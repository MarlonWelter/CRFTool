using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public static class MethodExtension
    {


        public static void TryMethodMultipleTimes(this Action action, int triesToGo)
        {
            triesToGo--;
            try
            {
                if(triesToGo >= 0)
                    action();
            }
            catch (Exception e)
            {
                if (triesToGo > 0)
                {
                    action.TryMethodMultipleTimes(triesToGo);
                }
                else
                    throw e;
            }
        }




        public static ReturnValue TryMethodMultipleTimes<ReturnValue>(this Func<ReturnValue> action, int triesToGo)
        {
            triesToGo--;
            try
            {
                if (triesToGo >= 0)
                    return action();
            }
            catch (Exception e)
            {
                if (triesToGo > 0)
                {
                    return action.TryMethodMultipleTimes(triesToGo);
                }
                else
                    throw e;
            }
            return default(ReturnValue);
        }


        public static ReturnValue TryMethodMultipleTimes<Param1, ReturnValue>(this Func<Param1, ReturnValue> action, int triesToGo, Param1 param1)
        {
            triesToGo--;
            try
            {
                if (triesToGo >= 0)
                    return action(param1);
            }
            catch (Exception e)
            {
                if (triesToGo > 0)
                {
                    return action.TryMethodMultipleTimes(triesToGo, param1);
                }
                else
                    throw e;
            }
            return default(ReturnValue);
        }


        public static ReturnValue TryMethodMultipleTimes<Param1, Param2, ReturnValue>(this Func<Param1, Param2, ReturnValue> action, int triesToGo, Param1 param1, Param2 param2)
        {
            triesToGo--;
            try
            {
                if (triesToGo >= 0)
                    return action(param1, param2);
            }
            catch (Exception e)
            {
                if (triesToGo > 0)
                {
                    return action.TryMethodMultipleTimes(triesToGo, param1, param2);
                }
                else
                    throw e;
            }
            return default(ReturnValue);
        }


        public static ReturnValue TryMethodMultipleTimes<Param1, Param2, Param3, ReturnValue>(this Func<Param1, Param2, Param3, ReturnValue> action, int triesToGo, Param1 param1, Param2 param2, Param3 param3)
        {
            triesToGo--;
            try
            {
                if (triesToGo >= 0)
                    return action(param1, param2, param3);
            }
            catch (Exception e)
            {
                if (triesToGo > 0)
                {
                    return action.TryMethodMultipleTimes(triesToGo, param1, param2, param3);
                }
                else
                    throw e;
            }
            return default(ReturnValue);
        }
    }
}
