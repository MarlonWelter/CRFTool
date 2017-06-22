using System;
using System.Collections.Generic;
using System.Linq;
using AgOType = CodeBase.AgO<System.Type, System.Type>;

namespace CodeBase
{
    public interface IAgO<S, T>
    {
        S Data1 { get; set; }
        T Data2 { get; set; }
    }
    public class AgO<S, T> : IAgO<S, T>
    {
        public S Data1 { get; set; }

        public T Data2 { get; set; }
        public AgO() { }
        public AgO(S data1, T data2)
        {
            Data1 = data1;
            Data2 = data2;
        }
        public override string ToString()
        {
            return Data1 == null ? " " : Data1.ToString() + "->" + Data2 == null ? " " : Data2.ToString();
        }
    }
    public class AgO<S, T, U>
    {
        public S Data1 { get; set; }
        public T Data2 { get; set; }
        public U Data3 { get; set; }
        public AgO() { }
        public AgO(S data1, T data2, U data3)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
        }
    }

    public class AgO<S, T, U, V>
    {
        public S Data1 { get; set; }
        public T Data2 { get; set; }
        public U Data3 { get; set; }
        public V Data4 { get; set; }
        public AgO() { }
        public AgO(S data1, T data2, U data3, V data4)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;
        }
    }
    public class AgO<S, T, U, V, W>
    {
        public S Data1 { get; set; }
        public T Data2 { get; set; }
        public U Data3 { get; set; }
        public V Data4 { get; set; }
        public W Data5 { get; set; }
        public AgO() { }
        public AgO(S data1, T data2, U data3, V data4, W data5)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;
            Data5 = data5;
        }
    }

    public class AgO<S, T, U, V, W, X>
    {
        public S Data1 { get; set; }
        public T Data2 { get; set; }
        public U Data3 { get; set; }
        public V Data4 { get; set; }
        public W Data5 { get; set; }
        public X Data6 { get; set; }
        public AgO() { }
        public AgO(S data1, T data2, U data3, V data4, W data5, X data6)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;
            Data5 = data5;
            Data6 = data6;
        }
    }


    public class AgO<S, T, U, V, W, X, Y>
    {
        public S Data1 { get; set; }
        public T Data2 { get; set; }
        public U Data3 { get; set; }
        public V Data4 { get; set; }
        public W Data5 { get; set; }
        public X Data6 { get; set; }
        public Y Data7 { get; set; }
        public AgO() { }
        public AgO(S data1, T data2, U data3, V data4, W data5, X data6, Y data7)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;
            Data5 = data5;
            Data6 = data6;
            Data7 = data7;
        }
    }


    public static class AgOExtension
    {
        /// <summary>
        /// pair.first := parametername, pair.second := parametervalue
        /// </summary>
        /// <param name="pairArray"></param>
        /// <param name="parameterKey"></param>
        /// <returns></returns>
        public static void Add<Type1, Type2>(this ICollection<IAgO<Type1, Type2>> agoCol, Type1 par1, Type2 par2)
        {
            agoCol.Add(new AgO<Type1, Type2>(par1, par2));
        }
    }

    public class Pair<S>
    {
        public S First { get; set; }
        public S Second { get; set; }
        public Pair(S first, S second)
        {
            First = first;
            Second = second;
        }
        public override bool Equals(object obj)
        {
            var other = obj as Pair<S>;
            if (other == null)
                return false;
            return other.First.Equals(First) && other.Second.Equals(Second);
        }
        public override int GetHashCode()
        {
            return First.GetHashCode() + Second.GetHashCode();
        }
    }
    public static class PairExtension
    {
        /// <summary>
        /// pair.first := parametername, pair.second := parametervalue
        /// </summary>
        /// <param name="pairArray"></param>
        /// <param name="parameterKey"></param>
        /// <returns></returns>
        public static TYPE GetParameter<TYPE>(this IEnumerable<Pair<TYPE>> pairArray, TYPE parameterKey)
        {
            if (pairArray != null)
            {
                return pairArray.FirstOrDefault(x => x.First.Equals(parameterKey)).Second;
            }
            else
            {
                return default(TYPE);
            }

        }
    }
}
