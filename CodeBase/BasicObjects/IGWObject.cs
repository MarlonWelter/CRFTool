
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Collections;
using System.Reflection;

namespace CodeBase
{
    public interface IGWObject
    {
        Guid GWId { get; set; }
    }

    public static class GWObjectX
    {
        public static readonly Type[] SimpleTypes = new Type[] { typeof(double), typeof(string), typeof(int), typeof(bool), typeof(Guid) };

        public static object ConvertFromString(Type type, string value)
        {
            try
            {
                switch (type.Name)
                {
                    case "Double":
                        return double.Parse(value, CultureInfo.InvariantCulture);
                    case "String":
                        return value;
                    case "Int32":
                        return int.Parse(value, CultureInfo.InvariantCulture);
                    case "Int64":
                        return int.Parse(value, CultureInfo.InvariantCulture);
                    case "Bool":
                        return bool.Parse(value);
                    case "Guid":
                        return Guid.Parse(value);
                    default:
                        return null;
                }
            }
            catch
            {
                Log.Post("Couldn't convert this value.");
            }
            return null;
        }
        public static string ConvertToString(Type type, object value)
        {
            try
            {
                switch (type.Name)
                {
                    case "Double":
                        var dlb = (double)value;
                        return dlb.ToString(CultureInfo.InvariantCulture);
                    case "String":
                        return (string)value;
                    case "Int32":
                        return ((int)value).ToString(CultureInfo.InvariantCulture);
                    case "Int64":
                        return ((int)value).ToString(CultureInfo.InvariantCulture);
                    case "Bool":
                        return ((bool)value).ToString(CultureInfo.InvariantCulture);
                    case "Guid":
                        return ((Guid)value).ToString();
                    default:
                        return null;
                }
            }
            catch
            {
                Log.Post("Couldn't convert this value.");
            }
            return null;
        }
        public static void TakeSimpleValues<T>(this T taker, T model)
        {
            if (taker == null || model == null)
                return;

            taker.TakeSimpleValues(model, typeof(T));
        }
        public static void TakeSimpleValues(this object taker, object model, Type DataType)
        {
            if (taker == null || model == null)
                return;
            var interfaceprops = DataType.GetInterfaces().SelectMany(i => i.GetProperties()).ToList();
            foreach (var prop in DataType.GetProperties().Union(interfaceprops))
            {
                var proptype = prop.PropertyType;
                if (prop.CanRead)
                {
                    var value = prop.GetValue(taker, new object[0]);
                    if (SimpleTypes.Contains(proptype))
                    {
                        if (prop.CanWrite)
                        {
                            prop.SetValue(taker, prop.GetValue(model), null);
                        }
                    }
                    else if (value is ILogic)
                    {
                        value.TakeSimpleValues(prop.GetValue(model), proptype);
                    }
                }
            }
        }
        public static void TakeSimpleValues<T>(this IHas<T> taker, IHas<T> model) where T : ILogic
        {
            if (taker != null && model != null)
                taker.Logic.TakeSimpleValues<T>(model.Logic);
        }
        public static bool SimpleValueEquals<T>(this IHas<T> self, IHas<T> other) where T : ILogic
        {
            return SimpleValueEquals<T>(self.Logic, other.Logic);
        }
        public static bool SimpleValueEquals<T>(this T self, T other)
        {
            var interfaceprops = typeof(T).GetInterfaces().SelectMany(i => i.GetProperties()).ToList();
            foreach (var prop in typeof(T).GetProperties().Union(interfaceprops))
            {
                if ((SimpleTypes.Contains(prop.PropertyType) && prop.CanRead))
                {
                    var selfval = prop.GetValue(self);
                    if (selfval != null && !selfval.Equals(prop.GetValue(other)))
                        return false;
                }
            }
            return true;
        }
        public static bool ValueEqualsFlat<T>(this IHas<T> self, IHas<T> other) where T : ILogic
        {
            return ValueEqualsFlat<T>(self.Logic, other.Logic);
        }
        public static bool ValueEqualsFlat<T>(this T self, T other)
        {
            var interfaceprops = typeof(T).GetInterfaces().SelectMany(i => i.GetProperties()).ToList();
            foreach (var prop in typeof(T).GetProperties().Union(interfaceprops))
            {
                if (prop.CanRead)
                {
                    if (!prop.GetValue(self).Equals(prop.GetValue(other)))
                        return false;
                }
            }
            return true;
        }

        public static void CreateXML<ObjectType>(ObjectType data, string path, LinkedList<IGWObject> buffer = null) where ObjectType : IGWObject
        {
            using (var writer = XmlWriter.Create(path))
            {
                writer.WriteStartDocument();
                CreateXML(data, writer);
                writer.WriteEndDocument();
            }
        }

        public static void CreateXML(IGWObject data, XmlWriter writer, LinkedList<IGWObject> buffer = null)
        {
            var ObjectType = data.GetType();

            writer.WriteStartElement(ObjectType.Name);

            CreateXMLInner(data, writer, buffer);

            writer.WriteEndElement();
        }

        private static void CreateXMLInner(IGWObject data, XmlWriter writer, LinkedList<IGWObject> buffer = null)
        {
            buffer = buffer ?? new LinkedList<IGWObject>();
            if (!buffer.Contains(data))
            {
                buffer.Add(data);
            }

            var ObjectType = data.GetType();

            //var interfaceprops = ObjectType.GetInterfaces().SelectMany(i => i.GetProperties()).ToList();
            foreach (var prop in ObjectType.GetProperties())//.Union(interfaceprops))
            {
                if (!prop.CanRead)
                    continue;
                var value = prop.GetValue(data);
                if (value == null)
                    continue;
                var proptype = prop.PropertyType;
                if (SimpleTypes.Contains(proptype))
                {
                    writer.WriteElementString(prop.Name, ConvertToString(proptype, value));
                }
                else if (value is ILogic)
                {
                    writer.WriteStartElement(prop.Name);
                    writer.WriteAttributeString("MitId", (value as IGWObject).GWId.ToString());
                    CreateXMLInner(value as ILogic, writer, buffer);
                    writer.WriteEndElement();

                }
                else if (value is IGWContext)
                {
                    continue;
                }
                else if (value is IGWObject)
                {
                    if (buffer.Contains(value))
                    {
                        writer.WriteStartElement(prop.Name);
                        writer.WriteAttributeString("MitId", (value as IGWObject).GWId.ToString());
                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteStartElement(prop.Name);
                        CreateXMLInner(prop.GetValue(data) as IGWObject, writer, buffer);
                        writer.WriteEndElement();
                    }
                }
                else if (value is IEnumerable<IGWObject>)
                {
                    var elements = value as IEnumerable<IGWObject>;
                    writer.WriteStartElement(prop.Name);

                    foreach (var element in elements)
                    {
                        writer.WriteStartElement("Item");
                        writer.WriteAttributeString("MitId", (element).GWId.ToString());
                        if (!buffer.Contains(element))
                        { CreateXMLInner(element, writer, buffer); }
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
            }

        }

        public static T ParseXml<T>(string path) where T : class, IGWObject
        {
            var t = default(T);
            DebugDepth = 0;
            using (var reader = XmlReader.Create(path))
            {
                t = ParseXml(reader, typeof(T), new MemoryDo()) as T;
            }
            return t;
        }
        public static object ParseXml(XmlReader reader, Type ObjectType, MemoryDo memoryDo, LinkedList<IGWObject> buffer = null)
        {
            buffer = buffer ?? new LinkedList<IGWObject>();
            var constr = ObjectType.GetConstructor(new Type[0]);
            var t = constr.Invoke(new object[0]) as IGWObject;

            //while (reader.Read())
            //{
            //    // prüfen, ob es sich aktuell um ein Element handelt
            //    if (reader.NodeType == XmlNodeType.Element)
            //    {
            //        break;
            //    }
            //}

            var obj = ParseXml(t, reader, null, null, null, memoryDo, buffer);
            buffer.Add(t);
            memoryDo.Trigger(t.GWId.ToString());

            return t;
        }
        public static int DebugDepth = 0;
        public static object ParseXml(object t, XmlReader reader, Type innerType, MethodInfo addToCurrentCollection, IEnumerable<IGWObject> tempCollection, MemoryDo memoryDo, LinkedList<IGWObject> buffer = null)
        {
            DebugDepth++;



            var ObjectType = t.GetType();
            var props = (ObjectType.GetProperties()).ToList();
            while (reader.Read())
            {
                // prüfen, ob es sich aktuell um ein Element handelt
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var prop = props.FirstOrDefault(p => p.Name.Equals(reader.Name));

                    if (DebugDepth != reader.Depth)
                    {

                    }

                    if (reader.IsEmptyElement)
                    {
                        if (reader.HasAttributes)
                        {
                            if (prop != null && prop.CanWrite)
                            {
                                reader.MoveToNextAttribute();
                                var id = reader.Value;
                                var propdata = buffer.FirstOrDefault(obj => obj.GWId.ToString().Equals(id));
                                prop.SetValue(t, propdata);

                                if (propdata == null)
                                    memoryDo.Add(reader.Value, prop, id, (propI, idI) => propI.SetValue(t, buffer.FirstOrDefault(obj => obj.GWId.ToString().Equals(idI))));
                            }
                            else if (reader.Name.StartsWith("Item") && addToCurrentCollection != null)
                            {
                                reader.MoveToNextAttribute();
                                var propdata = buffer.FirstOrDefault(obj => obj.GWId.ToString().Equals(reader.Value));
                                addToCurrentCollection.Invoke(tempCollection, new object[] { propdata });

                                if (propdata == null)
                                    memoryDo.Add(reader.Value, () => addToCurrentCollection.Invoke(tempCollection, new object[] { propdata }));
                            }
                            continue;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (prop != null)
                    {
                        var value = prop.GetValue(t);
                        var proptype = prop.PropertyType;
                        if (SimpleTypes.Contains(proptype))
                        {
                            reader.Read();
                            if (prop.CanWrite)
                            {
                                prop.SetValue(t, ConvertFromString(proptype, reader.Value));// reader.ReadContentAs(proptype, null));
                            }
                            reader.Read();
                        }
                        else if (value is ILogic)
                        {
                            if (reader.HasAttributes)
                            {
                                reader.MoveToNextAttribute();
                                (value as IGWObject).GWId = Guid.Parse(reader.Value);
                            }
                            ParseXml(value, reader, null, null, null, memoryDo, buffer);

                        }
                        else if (proptype.GetInterfaces().Contains(typeof(IGWObject)) && (prop.CanWrite || value != null))
                        {
                            if (prop.CanWrite && value == null)
                            {
                                if (reader.HasAttributes)
                                {
                                    reader.MoveToNextAttribute();
                                    var propdata = buffer.FirstOrDefault(obj => obj.GWId.ToString().Equals(reader.Value));
                                    prop.SetValue(t, propdata);
                                }
                                else
                                {
                                    prop.SetValue(t, ParseXml(reader, proptype, memoryDo, buffer));
                                }
                            }
                            else
                            {
                                //ParseXml(value, reader, value.GetType(), 
                                ParseXml(value, reader, null, null, null, memoryDo, buffer);
                                //value.TakeSimpleValues(ParseXml(reader, proptype, buffer), proptype);
                            }
                        }
                        else if (value is IEnumerable<IGWObject>)
                        {
                            innerType = value.GetType().GenericTypeArguments.FirstOrDefault();
                            addToCurrentCollection = value.GetType().GetMethod("Add");
                            tempCollection = value as IEnumerable<IGWObject>;
                            ParseXml(t, reader, innerType, addToCurrentCollection, tempCollection, memoryDo, buffer);
                        }

                    }
                    else if (reader.Name.StartsWith("Item") && addToCurrentCollection != null)
                    {
                        if (reader.HasAttributes)
                        {
                            reader.MoveToNextAttribute();
                            var propdata = buffer.FirstOrDefault(obj => obj.GWId.ToString().Equals(reader.Value));
                            if (propdata == null)
                            {
                                var constr2 = innerType.GetConstructor(new Type[0]);
                                var t2 = constr2.Invoke(new object[0]) as IGWObject;
                                t2.GWId = Guid.Parse(reader.Value);
                                var item = ParseXml(t2, reader, null, null, null, memoryDo, buffer);
                                buffer.Add(t2);
                                memoryDo.Trigger(t2.GWId.ToString());
                                addToCurrentCollection.Invoke(tempCollection, new object[] { item });
                            }
                            else
                            {
                                addToCurrentCollection.Invoke(tempCollection, new object[] { propdata });
                            }
                        }
                        else
                        {
                            var constr2 = innerType.GetConstructor(new Type[0]);
                            var t2 = constr2.Invoke(new object[0]) as IGWObject;
                            var item = ParseXml(t2, reader, null, null, null, memoryDo, buffer);
                            buffer.Add(t2);
                            memoryDo.Trigger(t2.GWId.ToString());
                            addToCurrentCollection.Invoke(tempCollection, new object[] { item });
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    var nm = reader.Name;
                    DebugDepth--;
                    return t;
                }

            }
            return t;
        }

    }
}
