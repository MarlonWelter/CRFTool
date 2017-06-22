using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class GraphAnalyse : GWRequest<GraphAnalyse>
    {
        public GraphAnalyse(IGWGraph graph)
        {
            Graph = graph;
            //Technique = technique;
        }
        public IGWGraph Graph { get; set; }
        public GraphAnalysisResult Result { get; set; }
        //public GraphAnalysisTechnique Technique { get; set; }
    }

    public class GraphAnalysisResult
    {
        public double[][] ShortestPaths { get; set; }
        public double[] ClusterCoefficients { get; set; }

        //public GraphAnalysisResult(double[,] shortestPaths, double[,] averagePaths)
        //{
        //    ShortestPaths = shortestPaths;
        //    AveragePaths = averagePaths;
        //}

        public GraphAnalysisResult(double[][] maxshortestPaths, double[] clusterCoefficients)
        {
            this.ShortestPaths = maxshortestPaths;
            this.ClusterCoefficients = clusterCoefficients;
        }
        //public double[,] ShortestPaths { get; set; }
        public double[,] AveragePaths { get; set; }

        public double[][] ConProbByDistance { get; set; }
    }
    //public enum GraphAnalysisTechnique
    //{
    //    ShortestPaths,
    //    AveragePaths
    //}
    public static class GraphAnalysisTechniques
    {

        public static double[][] ConnectionProbabilityByDistance(IGWGraph graph, double[][] shortestPaths, int Maxdistance)
        {
            var conProbs = new double[graph.Nodes.Count()][];
            var nodes = graph.Nodes.OrderBy(n => n.GraphId).ToList();

            for (int i1 = 0; i1 < nodes.Count; i1++)
            {
                var node = nodes[i1];
                conProbs[i1] = new double[Maxdistance];
                for (int i2 = 1; i2 <= Maxdistance; i2++)
                {
                    var possibleCons = 0.0;
                    var actualCons = 0.0;
                    var nodesInDist = new LinkedList<IGWNode>();
                    for (int i3 = 0; i3 < nodes.Count; i3++)
                    {
                        if (shortestPaths[i1][i3] == i2)
                            nodesInDist.Add(nodes[i3]);
                    }
                    if (nodesInDist.NotNullOrEmpty())
                    {
                        foreach (var nd in nodesInDist)
                        {
                            foreach (var nd2 in nodesInDist)
                            {
                                if (nd.GraphId >= nd2.GraphId)
                                    continue;
                                possibleCons++;
                                if (nd.Neighbours.Contains(nd2))
                                    actualCons++;
                            }
                        }
                        conProbs[i1][i2 - 1] = actualCons / possibleCons;
                    }
                }
            }
            return conProbs;
        }

        public static double[] AnalyseClusterCoefficient(IGWGraph graph)
        {
            var coeffcients = new double[graph.Nodes.Count()];

            foreach (var node in graph.Nodes)
            {
                var coefficient = 0.0;
                double allCons = 0;
                double actualCons = 0;
                var nbs = node.Neighbours.ToList();
                for (int i1 = 0; i1 < nbs.Count; i1++)
                {
                    for (int i2 = i1 + 1; i2 < nbs.Count; i2++)
                    {
                        allCons++;
                        if (nbs[i1].Neighbours.Contains(nbs[i2]))
                            actualCons++;
                    }
                }
                if (allCons > 0)
                    coefficient = actualCons / allCons;
                else
                    coefficient = 0;

                coeffcients[node.GraphId] = coefficient;
            }

            return coeffcients;
        }
        public static double[][] ComputeShortestPaths(IGWGraph graph)
        {
            var nodes = graph.Nodes.ToList();
            var marks = new bool[nodes.Count];

            var distances = new double[nodes.Count][];
            foreach (var node in nodes)
            {
                distances[node.GraphId] = new double[nodes.Count];
            }
            foreach (var node in nodes)
            {
                foreach (var otherNode in nodes)
                {
                    if (node.GraphId > otherNode.GraphId)
                        continue;


                    int distance = 0;
                    marks = new bool[nodes.Count];
                    if (otherNode.GraphId == node.GraphId)
                    {
                        distances[otherNode.GraphId][node.GraphId] = distance;
                        continue;
                    }
                    var currentBorder = new LinkedList<IGWNode>();
                    currentBorder.Add(node);
                    marks[node.GraphId] = true;
                    var foundPath = false;
                    while (currentBorder.NotNullOrEmpty() && !foundPath)
                    {
                        distance++;
                        var newBorder = new LinkedList<IGWNode>();
                        foreach (var borderNode in currentBorder)
                        {
                            if (foundPath)
                                break;
                            foreach (var nb in borderNode.Neighbours)
                            {
                                if (!marks[nb.GraphId])
                                {
                                    if (otherNode.GraphId == nb.GraphId)
                                    {
                                        distances[otherNode.GraphId][node.GraphId] = distance;
                                        distances[node.GraphId][otherNode.GraphId] = distance;
                                        foundPath = true;
                                        break;
                                    }
                                    else
                                    {
                                        marks[nb.GraphId] = true;
                                        newBorder.Add(nb);
                                    }
                                }
                            }
                        }
                        currentBorder = newBorder;
                    }
                    if (!foundPath)
                    {
                        distances[otherNode.GraphId][node.GraphId] = double.PositiveInfinity;
                        distances[node.GraphId][otherNode.GraphId] = double.PositiveInfinity;
                    }
                }
            }
            return distances;
        }

        //public static List<double>[] AnalyseShortestPaths(IGWGraph graph)
        //{
        //    var nodes = graph.Nodes.ToList();
        //    var marks = new bool[nodes.Count];

        //    var distances = new List<double>[nodes.Count];

        //    foreach (var node in nodes)
        //    {
        //        distances[node.GraphId] = new List<double>();
        //        foreach (var otherNode in nodes)
        //        {
        //            //if (node.GraphId > otherNode.GraphId)
        //            //    continue;
        //            int distance = 0;
        //            marks = new bool[nodes.Count];
        //            if (otherNode.GraphId == node.GraphId)
        //            {
        //                continue;
        //            }
        //            var currentBorder = new LinkedList<IGWNode>();
        //            currentBorder.Add(node);
        //            marks[node.GraphId] = true;
        //            var foundPath = false;
        //            while (currentBorder.NotNullOrEmpty() && !foundPath)
        //            {
        //                distance++;
        //                var newBorder = new LinkedList<IGWNode>();
        //                foreach (var borderNode in currentBorder)
        //                {
        //                    if (foundPath)
        //                        break;
        //                    foreach (var nb in borderNode.Neighbours)
        //                    {
        //                        if (!marks[nb.GraphId])
        //                        {
        //                            if (otherNode.GraphId == nb.GraphId)
        //                            {
        //                                distances[node.GraphId].Add(distance);
        //                                //distances[otherNode.GraphId].Add(distance);
        //                                foundPath = true;
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                marks[nb.GraphId] = true;
        //                                newBorder.Add(nb);
        //                            }
        //                        }
        //                    }
        //                }
        //                currentBorder = newBorder;
        //            }
        //            if (!foundPath)
        //            {
        //                //distances[otherNode.GraphId, node.GraphId] = double.PositiveInfinity;
        //                //distances[node.GraphId, otherNode.GraphId] = double.PositiveInfinity;
        //            }
        //        }
        //    }
        //    return distances;
        //}

        private static Random Random = new Random();
        public static double[,] AnalyseAveragePaths(IGWGraph graph)
        {
            var nodes = graph.Nodes.ToList();
            var marks = new bool[nodes.Count];

            var distances = new double[nodes.Count, nodes.Count];

            int runs = 100;
            int maxSearchLength = 1000;

            foreach (var node in nodes)
            {
                foreach (var otherNode in nodes)
                {
                    if (node.GraphId > otherNode.GraphId)
                        continue;

                    var distancesTemp = new LinkedList<double>();
                    for (int run = 0; run < runs; run++)
                    {
                        int distance = 0;
                        var currentNode = node;
                        while (otherNode.GraphId != currentNode.GraphId && distance <= maxSearchLength)
                        {
                            distance++;
                            var nbCount = currentNode.Neighbours.Count();
                            if (nbCount == 0)
                            {
                                distance = int.MaxValue;
                                break;
                            }
                            var rdm = Random.Next(nbCount);
                            currentNode = currentNode.Neighbours.Skip(rdm).First();
                        }
                        if (distance == int.MaxValue)
                            distancesTemp.Add(double.PositiveInfinity);
                        else
                            distancesTemp.Add(distance);
                    }
                    distances[node.GraphId, otherNode.GraphId] = distancesTemp.Average();
                    distances[otherNode.GraphId, node.GraphId] = distancesTemp.Average();
                }
            }
            return distances;
        }
    }

    public class GraphAnalysisManager : IRequestListener
    {
        public GraphAnalysisManager()
        {
            this.Register();
        }
        public IGWContext Context { get; set; }
        private void OnGraphAnalysis(GraphAnalyse obj)
        {
            //var shortestPaths = GraphAnalysisTechniques.AnalyseShortestPaths(obj.Graph);
            var ShortestPaths = GraphAnalysisTechniques.ComputeShortestPaths(obj.Graph);
            var ConByDistances = GraphAnalysisTechniques.ConnectionProbabilityByDistance(obj.Graph, ShortestPaths, 12);
            //var averagePaths = GraphAnalysisTechniques.AnalyseAveragePaths(obj.Graph);
            var clusterCoefficients = GraphAnalysisTechniques.AnalyseClusterCoefficient(obj.Graph);
            var results = new GraphAnalysisResult(ShortestPaths, clusterCoefficients);
            results.ConProbByDistance = ConByDistances;
            obj.Result = results;
        }
        public void Register()
        {
            this.DoRegister<GraphAnalyse>(OnGraphAnalysis);
        }

        public void Unregister()
        {
            this.DoUnregister<GraphAnalyse>(OnGraphAnalysis);
        }
    }
}
