using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface IClassificationResult : ILogic
    {
        int TruePositives { get; }
        int TrueNegatives { get; }
        int FalsePositives { get; }
        int FalseNegatives { get; }

    }

    public class ClassificationResultLogic : IClassificationResult
    {
        public Guid GWId { get; set; }
        public int TruePositives { get; set; }
        public int TrueNegatives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }

    }

    public static class ClassificationResultX
    {
        //public static int TruePositives(this IHas<IClassificationResult> logicHolder)
        //{
        //    return logicHolder.Logic.TruePositives;
        //}
        //public static int TrueNegatives(this IHas<IClassificationResult> logicHolder)
        //{
        //    return logicHolder.Logic.TrueNegatives;
        //}
        //public static int FalsePositives(this IHas<IClassificationResult> logicHolder)
        //{
        //    return logicHolder.Logic.FalsePositives;
        //}
        //public static int FalseNegatives(this IHas<IClassificationResult> logicHolder)
        //{
        //    return logicHolder.Logic.FalseNegatives;
        //}
    }

}
