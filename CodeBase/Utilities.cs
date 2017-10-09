using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public static class Utilities
    {
        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static Type MaxEntry<Type, CompareType>(IEnumerable<Type> elements, Func<Type, CompareType> keySelector)
                where CompareType : IComparable<CompareType>
        {
            var maxEl = default(Type);
            var maxKey = default(CompareType);
            foreach (var entry in elements)
            {
                if (maxEl == null)
                {
                    maxEl = entry;
                    maxKey = keySelector(entry);
                }
                else
                {
                    var entryKey = keySelector(entry);
                    if (maxKey.CompareTo(entryKey) < 0)
                    {
                        maxEl = entry;
                        maxKey = entryKey;
                    }
                }
            }
            return maxEl;
        }
    }
}
