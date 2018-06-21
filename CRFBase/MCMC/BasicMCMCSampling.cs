using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using CRFGraph = CodeBase.IGWGraph<CodeBase.ICRFNodeData, CodeBase.ICRFEdgeData, CodeBase.ICRFGraphData>;

namespace CRFBase
{
    public class BasicMCMCSampling
    {
        public const int DefaultBufferFindStartpointViterbi = 25;

        public static void Do(CRFGraph graph, Action<CRFGraph> findStartpoint, Action<CRFGraph, int> changePosition, Action<CRFGraph, int> evaluatePosition, int startpoints, int preRunLength, int runLength)
        {
            graph.Nodes.Each(n => { n.Data.TempCount = 0; });

            for (int startpointcount = 0; startpointcount < startpoints; startpointcount++)
            {
                graph.Nodes.Each(n => { n.Data.LastUpdate = 0; });
                findStartpoint(graph);
                for (int iteration = 1; iteration <= preRunLength; iteration++)
                {
                    changePosition(graph, 0);
                }
                for (int iteration = 1; iteration <= runLength; iteration++)
                {
                    changePosition(graph, iteration);
                }
                evaluatePosition(graph, runLength);
            }
        }

        public static CountOccurences Do(CRFGraph graph, int startpoints, int preRunLength, int runLength)
        {

            var occurences = new CountOccurences(graph);
            Do(graph, FindStartPointNodeProb, ChangePositionDefault, occurences.Count, startpoints, preRunLength, runLength);
            return occurences;
        }

        private static void FindStartPointNodeProb(CRFGraph graph)
        {

            foreach (var item in graph.Nodes)
            {
                item.Data.TempAssign = Math.Log(Random.NextDouble()) < item.Data.Score(0) ? 0 : 1;
            }
        }

        private static Random random;
        public static Random Random
        {
            get { return (random = random ?? new Random()); }
            set { random = value; }
        }

        public static void ChangePositionDefault(CRFGraph graph, int time)
        {
            var node = graph.Nodes.ToList().RandomElement(Random);

            //compute relative change
            var relChange = 0.0;
            relChange += node.Data.Score((node.Data.TempAssign + 1) % 2) - node.Data.Score(node.Data.TempAssign);

            //old edges Score
            foreach (var edge in node.Edges)
            {
                relChange -= edge.TempScore();
            }

            //new edges Score
            node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
            foreach (var edge in node.Edges)
            {
                relChange += edge.TempScore();
            }

            if (Random.NextDouble() <= Math.Exp(relChange))
            {
                //state changed
                //TODO check for correct counting
                node.Data.TempCount += (1 - node.Data.TempAssign) * (time - node.Data.LastUpdate);
                node.Data.LastUpdate = time;
            }
            else
            {
                //stay in state
                node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
            }
        }
    }

    public class CountOccurences
    {
        public CountOccurences(CRFGraph graph)
        {
            Graph = graph;
        }
        public int CountedAssignmentsTotal { get; set; }

        public void Count(CRFNode node, int value)
        {
            node.TempCount += node.TempAssign;
        }

        public CRFGraph Graph { get; set; }

        public void Count(CRFGraph graph, int time)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.TempCount += node.Data.TempAssign * (time - node.Data.LastUpdate);
            }
            CountedAssignmentsTotal += time;
        }
    }

}
