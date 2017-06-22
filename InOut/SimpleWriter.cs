
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace CodeBase
{
    public static class JSONX
    {
        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
        public static void SaveAsJSON<T>(this T data, string file, JsonSerializerSettings jsonSerializerSettings = null)
        {
            using (var writer = new StreamWriter(file))
            {
                var jsonString = JsonConvert.SerializeObject(data, jsonSerializerSettings ?? new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.Objects, Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });
                writer.Write(jsonString);
            }
        }
        public static T LoadFromJSON<T>(string file)
        {
            var data = default(T);
            using (var reader = new StreamReader(file))
            {
                data = JsonConvert.DeserializeObject<T>(reader.ReadToEnd(), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Error, PreserveReferencesHandling = PreserveReferencesHandling.Objects, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            return data;
        }

        //public static void Do(string file, IEnumerable<AgO<string, string>> entries)
        //{
        //    using (var writer = new StreamWriter(file))
        //    {
        //        foreach (var entry in entries)
        //        {
        //            writer.WriteLine(entry.Data1 + ": " + entry.Data2);
        //        }
        //    }
        //}
        //public static void Do(string file, params string[] entries)
        //{
        //    using (var writer = new StreamWriter(file))
        //    {
        //        for (int i = 0; i < entries.Length + 1; i += 2)
        //        {
        //            writer.WriteLine(entries[i] + ": " + entries[i + 1]);
        //        }
        //    }
        //}
    }
    public static class StoreX
    {
        public const string Delimiter = ":";
        public static void StoreProb(object obj, PropertyInfo prop, StreamWriter writer)
        {
            //check if null
            if (prop.GetValue(obj) == null)
            {
                writer.WriteLine("\"" + prop.Name + "\"" + Delimiter + " " + "\"" + "null" + "\"" + ",");
                return;
            }
            if (prop.PropertyType is IEnumerable)
            {

            }

            switch (prop.PropertyType.Name)
            {
                case "String":
                    writer.WriteLine("\"" + prop.Name + "\"" + Delimiter + " " + "\"" + prop.GetValue(obj).ToString() + "\"" + ",");
                    break;
                case "Int32":
                    writer.WriteLine("\"" + prop.Name + "\"" + Delimiter + " " + prop.GetValue(obj).ToString() + ",");
                    break;
                case "Int64":
                    writer.WriteLine("\"" + prop.Name + "\"" + Delimiter + " " + prop.GetValue(obj).ToString() + ",");
                    break;
                case "Double":
                    writer.WriteLine("\"" + prop.Name + "\"" + Delimiter + " " + prop.GetValue(obj).ToString() + ",");
                    break;
                case "Bool":
                    writer.WriteLine("\"" + prop.Name + "\"" + Delimiter + " " + prop.GetValue(obj).ToString() + ",");
                    break;
                default:
                    writer.WriteLine("\"" + prop.Name + "\"" + Delimiter + " ");
                    StoreObj(prop.GetValue(obj), writer);
                    break;
            }

        }
        public static void StoreObj<Type>(Type obj, StreamWriter writer)
        {
            writer.WriteLine("{");
            foreach (var prop in typeof(Type).GetProperties())
            {
                if (prop.CustomAttributes.Any(at => at.AttributeType == typeof(GWStoreAttribute)))
                {
                    StoreProb(obj, prop, writer);
                }
            }
            writer.WriteLine("}");
        }
    }

    public class GWStoreAttribute : Attribute
    {

    }

    public interface GWStoreable
    {
        void Store(StreamWriter writer);
    }

}
