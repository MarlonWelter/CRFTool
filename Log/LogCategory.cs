using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public enum LogCategory
    {
        Detail,
        Overview,
        Inconsistency,
        Critical,
        UserInput,
        Technical,
        Result
    }

    public static class LogCategoryExtension
    {
        public static string AsString(this LogCategory category)
        {
            switch (category)
            {
                case LogCategory.Detail:
                    return "Detail";
                case LogCategory.Overview:
                    return "Overview";
                case LogCategory.Inconsistency:
                    return "Inconsistency";
                case LogCategory.Critical:
                    return "Critical";
                case LogCategory.UserInput:
                    return "UserInput";
                case LogCategory.Technical:
                    return "Technical";
                default:
                    return "Unknown Category";
            }
        }
    }
}
