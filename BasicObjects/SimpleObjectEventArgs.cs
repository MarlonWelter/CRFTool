using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{

    /// <summary>
    /// SimpleObjectEventArgs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimO<T> : EventArgs
    {
        public readonly T Data;
        public SimO(T data)
            : base()
        {
            Data = data;
        }
    }
    /// <summary>
    /// SimpleObjectEventArgs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="S"></typeparam>
    public class SimOArgs<T, S> : EventArgs
    {
        public readonly T Data1;
        public readonly S Data2;
        public SimOArgs(T data1, S data2)
            : base()
        {
            Data1 = data1;
            Data2 = data2;
        }
    }

    public static class SimOX
    {
        public static void Fire<T>(this EventHandler<SimO<T>> eh, T data)
        {
            if (eh != null)
                eh(data, new SimO<T>(data));
        }
        public static void Fire<T>(this EventHandler<SimO<T>> eh, object sender, T data)
        {
            if (eh != null)
                eh(sender, new SimO<T>(data));
        }
    }
}
