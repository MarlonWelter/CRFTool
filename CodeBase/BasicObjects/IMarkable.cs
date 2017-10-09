using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface IMarkable
    {
        bool IsMarked { get; set; }
    }
    public interface ITempSCore
    {
        double Temp { get; set; }
    }
    public static class MarkableExtensions
    {
        public static void Mark(this IMarkable markable)
        {
            if (markable.IsMarked == true)
                throw new Exception("markable object must not be marked twice.");
            markable.IsMarked = true;
        }
        public static void Unmark(this IMarkable markable)
        {
            markable.IsMarked = false;
        }
    }
}
