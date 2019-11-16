using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRFBase
{
    public class RandomGrowing
    {
        public static string MaxQueueFile = "";
        public static int maximumBorder;
        public static int workingMSA = 0;
        public static int problemMSA = 0;
        private static LinkedList<IGWEdge> OutsideEdges = new LinkedList<IGWEdge>();
        private static bool[] isInQueue;
        public static Random Random { get; set; } = new Random();

        public static LinkedList<IGWNode> ComputeQueue(IList<IGWNode> vertices)
        {
            var startTime = DateTime.Now;

            isInQueue = new bool[vertices.Count];

            OutsideEdges.Clear();

            LinkedList<IGWNode> queue = new LinkedList<IGWNode>();
            maximumBorder = 0;

            var startpoint = vertices.RandomElement(Random);
            queue.AddRange(startpoint);
            OutsideEdges.AddRange(startpoint.Edges);

            while (queue.Count < vertices.Count())
            {
                var randomEdge = OutsideEdges.RandomElement(Random);
                var selectedNode = default(IGWNode);
                if (!queue.Contains(randomEdge.Head))
                {
                    selectedNode = randomEdge.Head;
                }
                else
                {
                    selectedNode = randomEdge.Foot;
                }
                queue.Add(selectedNode);
                OutsideEdges.RemoveWhere(edge => edge.Head == selectedNode || edge.Foot == selectedNode);
                foreach (var edge in selectedNode.Edges)
                {
                    var other = edge.Head == selectedNode ? edge.Foot : edge.Head;
                    if (!queue.Contains(other))
                    {
                        OutsideEdges.Add(edge);
                    }
                }
            }

            return queue;
        }
    }
}