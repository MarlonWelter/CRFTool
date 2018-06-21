
using CodeBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.GibbsSampling
{
    class MCMCSamplingManager : GWManager<GiveProbableLabelings>
    {
        public int MaxCombinationsBuffer { get; set; }

        override protected void OnRequest(GiveProbableLabelings obj)
        {
            var watch = new Stopwatch();
            watch.Start();

            var occurences = BasicMCMCSampling.Do(obj.Graph, obj.StartingPoints, obj.PreRunLength, obj.RunLength);

            watch.Stop();

            var result = new Dictionary<IGWNode, double>();

            foreach (var node in occurences.Graph.Nodes)
            {
                result.Add(node, ((double)node.Data.TempCount) / occurences.CountedAssignmentsTotal);
            }

            obj.Result = result;
        }
    }
}
