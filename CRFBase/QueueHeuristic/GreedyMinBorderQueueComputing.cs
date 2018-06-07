using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;

namespace CRFBase
{
    class GreedyMinBorderQueueComputing
    {
        public static string MaxQueueFile = "";
        public static int maximumBorder;
        public static int workingMSA = 0;
        public static int problemMSA = 0;
        private static Dictionary<IGWNode, ICollection<IGWEdge>> OutsideEdges = new Dictionary<IGWNode, ICollection<IGWEdge>>();
        private static bool[] isInQueue;

        public static LinkedList<IGWNode> ComputeQueue(IList<IGWNode> vertices, IEnumerable<IGWNode> startPatch)
        {
            var startTime = DateTime.Now;

            if (startPatch == null)
                startPatch = new LinkedList<IGWNode>();

            var isChosen = new bool[vertices.Count];
            isInQueue = new bool[vertices.Count];

            startPatch.Each(n => isChosen[n.GraphId] = true);
            OutsideEdges.Clear();

            foreach (var item in vertices)
            {
                OutsideEdges.Add(item, item.Edges.Where<IGWEdge>(e => !isChosen[item.Neighbour(e).GraphId]).ToList());

            }

            LinkedList<IGWNode> queue = new LinkedList<IGWNode>();
            maximumBorder = 0;


            while (queue.Count < vertices.Count() - startPatch.Count())
            {

                int minimumOutsideEdges = vertices.Count();
                int minimumIndex = -1;

                for (int counter = 0; counter < vertices.Count(); counter++)
                {

                    var vertexInFocus = vertices[counter];
                    if (isChosen[vertexInFocus.GraphId])
                        continue;
                    if (OutsideEdges[vertexInFocus].Count > 0 || (!isInQueue[vertexInFocus.GraphId]))
                    {
                        if (OutsideEdges[vertexInFocus].Count < minimumOutsideEdges)
                        {
                            minimumIndex = counter;
                            minimumOutsideEdges = OutsideEdges[vertexInFocus].Count;
                        }
                    }
                }

                var minimumOutsideEdgesVertex = vertices[minimumIndex];


                if (!queue.Contains(minimumOutsideEdgesVertex))
                {
                    queue.AddLast(minimumOutsideEdgesVertex);
                    isInQueue[minimumOutsideEdgesVertex.GraphId] = true;
                    RemoveInnerEdges(queue);
                    countBorder(queue);
                }

                //Make the list of the vertices you want to add
                LinkedList<IGWNode> verticesToAdd = new LinkedList<IGWNode>();
                foreach (var edge in OutsideEdges[minimumOutsideEdgesVertex])
                {
                    verticesToAdd.AddLast(minimumOutsideEdgesVertex.Neighbour(edge));
                }

                foreach (var vertexToAdd in verticesToAdd)
                {

                    if (!queue.Contains(vertexToAdd))
                    {

                        queue.AddLast(vertexToAdd);
                        isInQueue[vertexToAdd.GraphId] = true;

                        //remove edges that are now inside
                        RemoveInnerEdges(queue);

                        countBorder(queue);
                    }
                    else
                    {
                        Console.WriteLine("Inkonsistenz in Queue-Berechnung!");
                    }
                }
            }
            if (maximumBorder < 22)
            {
                if (MaxQueueFile != string.Empty)
                {
                    using (var writer = File.AppendText(MaxQueueFile))
                    {
                        writer.WriteLine(maximumBorder);
                    }
                }
                workingMSA++;
            }
            else
            {
                if (MaxQueueFile != string.Empty)
                {
                    using (var writer = File.AppendText(MaxQueueFile))
                    {
                        writer.WriteLine(maximumBorder);
                    }
                }
                problemMSA++;
            }

            return queue;
        }

        private static void countBorder(LinkedList<IGWNode> queue)
        {

            int bordercount = 0;
            int checksum = 0;

            foreach (var vertex in queue)
            {
                if (OutsideEdges[vertex].Count > 0)
                    bordercount++;
                if (OutsideEdges[vertex].Count == 0)
                    checksum++;
            }
            if (bordercount + checksum != queue.Count)
                Console.WriteLine("Inkonsistenz in Queue-Berechnung!");

            if (bordercount > maximumBorder)
                maximumBorder = bordercount;

        }

        private static void RemoveInnerEdges(LinkedList<IGWNode> queue)
        {
            LinkedList<IGWEdge> rememberToRemove = new LinkedList<IGWEdge>();
            foreach (var item in queue)
            {
                foreach (IGWEdge edge in OutsideEdges[item])
                {
                    if (isInQueue[item.Neighbour(edge).GraphId])
                    {
                        if (!rememberToRemove.Contains(edge))
                            rememberToRemove.AddLast(edge);
                    }
                }
            }

            foreach (IGWEdge edge in rememberToRemove)
            {
                OutsideEdges[edge.Head].Remove(edge);
                OutsideEdges[edge.Foot].Remove(edge);
            }
        }

    }
}