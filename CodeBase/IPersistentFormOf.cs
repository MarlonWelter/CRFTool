using BasicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace General.BasicObject
{
    public interface IPersistentFormOf<LightType> : IMITObject
    {
        void TakeValuesOf(LightType element);
        LightType ToLightElementFlat();
        LightType ToLightElementComplex(ICollection<IMITObject> buffer);
        bool SolveReferences(LightType lightObject);
    }
    public static class PersistentFormExtension
    {

        //public static bool SolveReferences<LightType>(IEnumerable<IPersistentFormOf<LightType>> en)
        //{
        //    bool success = true;
        //    foreach (var item in en)
        //    {
        //        success = success && item.SolveReferences();
        //    }
        //    return success;
        //}

        public static ILightType ToLightElementFlat<ILightType, LightClass>(this IPersistentFormOf<ILightType> persistentObject)
            where ILightType : IMITObject
            where LightClass : ILightType, new()
        {
            var lightObject = new LightClass();

            foreach (PropertyInfo pi in typeof(ILightType).GetProperties())
            {
                if (pi.CanWrite)
                {
                    if (pi.GetType().IsSubclassOf(typeof(IMITObject)))
                    {

                    }
                    //var value = typeof(T).GetProperty(pi.Name).GetValue(objectModel, null);
                    //if (!value.Equals(typeof(T).GetProperty(pi.Name).GetValue(objectToFill, null)))
                    //    pi.SetValue(objectToFill, value, null);
                }
            }
            return lightObject;
        }
        public static IEnumerable<LightType> ToLightElementsFlat<LightType, PersistentType>(this IEnumerable<PersistentType> persistentObjects)
            where LightType : IMITObject, new()
            where PersistentType : IPersistentFormOf<LightType>
        {
            var ll = new LinkedList<LightType>();
            foreach (var item in persistentObjects)
            {
                ll.AddLast(item.ToLightElementFlat());
            }
            return ll;
        }
        public static IEnumerable<LightType> ToLightElementsComplex<LightType, PersistentType>(this IEnumerable<PersistentType> persistentObjects)
            where LightType : class, IMITObject, new()
            where PersistentType : IPersistentFormOf<LightType>
        {
            var ll = new LinkedList<LightType>();
            var buffer = new LinkedList<IMITObject>();
            foreach (var item in persistentObjects)
            {
                ll.AddLast(item.ToLightElementComplex(buffer));
            }
            return ll;
        }

        public static LightType Find<LightType>(this IEnumerable<IMITObject> buffer, Guid mitId)
            where LightType : class, IMITObject
        {
            var obj = buffer.FirstOrDefault(m => m.MitId.Equals(mitId));

            var refr = obj as LightType;
            if (refr != null)
                return refr;

            return default(LightType);
        }

        public static LightType FindOrCreateAndUpdateBuffer<LightType>(this ICollection<IMITObject> buffer, Guid mitId)
            where LightType : class, IMITObject, new()
        {
            var obj = buffer.FirstOrDefault(m => m.MitId.Equals(mitId));

            var refr = obj as LightType;
            if (refr != null)
                return refr;

            var lightType = new LightType();
            buffer.Add(lightType);
            return lightType;
        }
        public static LightType FindOrCreate<LightType>(this ICollection<IMITObject> buffer, Guid mitId)
            where LightType : class, IMITObject, new()
        {
            var obj = buffer.FirstOrDefault(m => m.MitId.Equals(mitId));

            var refr = obj as LightType;
            if (refr != null)
                return refr;

            var lightType = new LightType();
            return lightType;
        }



    }
}
