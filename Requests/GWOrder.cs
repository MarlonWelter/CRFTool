using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class GWCommand<DataType>
    {
        public GWCommand()
        {
        }
        public GWCommand(DataType data)
        {
            Data = data;
        }
        public DataType Data { get; set; }
    }
    public class GWCommand
    {
        public GWCommand()
        {
        }
    }
    public class GWCommand<InType, ReturnType>
    {
        public GWCommand(InType inData)
        {
            InData = inData;
        }
        public InType InData { get; set; }
        public ReturnType ReturnData { get; set; }

    }

    public interface GWCommandServant<Type, InType, ReturnType>
        where Type : GWCommand<InType, ReturnType>
    {
        ReturnType Work(InType inData);
    }

    public static class GWCommandX
    {
        public static ReturnType Order<Type, InType, ReturnType>(this GWCommand<InType, ReturnType> order)
        where Type : GWCommand<InType, ReturnType>
        {
            return DefaultGWCommandServant<Type, InType, ReturnType>.Servant(order.InData);
        }

        public static void Register<Type, InType, ReturnType>(this GWCommandServant<Type, InType, ReturnType> order)
        where Type : GWCommand<InType, ReturnType>
        {
            DefaultGWCommandServant<Type, InType, ReturnType>.Servant = order.Work;
        }
    }

    public static class DefaultGWCommandServant<Type, InType, ReturnType>
        where Type : GWCommand<InType, ReturnType>
    {
        public static Func<InType, ReturnType> Servant { get; set; }
    }
}
