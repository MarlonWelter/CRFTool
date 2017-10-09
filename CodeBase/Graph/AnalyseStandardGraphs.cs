using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase.Graph
{
    public class AnalyseStandardGraphs
    {
        public static void Do(int graphSizes)
        {
            //new FileLogger("GraphAnalysis(" + graphSizes + ").txt");

            var graphs = GWGraphpackageOne.DefaultPackage<object, object, object>(graphSizes);
            foreach (var graph in graphs)
            {
                var request = new GraphAnalyse(graph);
                request.RequestInDefaultContext();

                using (var writer = new StreamWriter(graph.Name + "-data.txt"))
                {
                    writer.WriteLine("AnalysisResult: " + graph.Name);

                    writer.Write(writer.NewLine);
                    writer.WriteLine("Nodes: " + graph.Nodes.Count);
                    writer.WriteLine("Edges: " + graph.Edges.Count);
                    writer.WriteLine("Diameter: " + request.Result.ShortestPaths.Max(l => l.NotNullOrEmpty() ? l.Max() : 0.0));
                    writer.WriteLine("Average MaxShortestPath: " + request.Result.ShortestPaths.Average(l => l.NotNullOrEmpty() ? l.Max() : 0.0));
                    writer.WriteLine("Average ClusterCoefficient: " + request.Result.ClusterCoefficients.Average());

                    writer.Write(writer.NewLine);
                    writer.WriteLine("ConnectionProbability By Distance: ");
                    for (int i1 = 0; i1 < request.Result.ConProbByDistance.Length; i1++)
                    {
                        writer.Write("[" + i1 + "]" + ":");
                        for (int i2 = 0; i2 < request.Result.ConProbByDistance[i1].Length; i2++)
                        {
                            writer.Write(" " + Math.Round(request.Result.ConProbByDistance[i1][i2], 2) + ",");
                        }
                        writer.WriteLine();
                    }

                    writer.Write(writer.NewLine);
                    writer.WriteLine("CluserCoefficients: ");
                    for (int i1 = 0; i1 < request.Result.ClusterCoefficients.Length; i1++)
                    {
                        writer.WriteLine("[" + i1 + "]" + ": " + request.Result.ClusterCoefficients[i1]);
                    }

                    writer.Write(writer.NewLine);
                    writer.WriteLine("ShortestPaths (Max, Average): ");
                    for (int i1 = 0; i1 < request.Result.ShortestPaths.Length; i1++)
                    {
                        writer.WriteLine("[" + i1 + "]" + ": (" + request.Result.ShortestPaths[i1].Max() + ", " + request.Result.ShortestPaths[i1].Average() + ")");
                    }

                    //writer.WriteLine("ShortestPaths: ");
                    //for (int i1 = 0; i1 < request.Result.ShortestPaths.GetLength(0); i1++)
                    //{
                    //    for (int i2 = 0; i2 < request.Result.ShortestPaths.GetLength(1); i2++)
                    //    {
                    //        writer.WriteLine("[" + i1 + "," + i2 + "]" + ": " + request.Result.ShortestPaths[i1, i2]);
                    //    }
                    //}
                    //writer.Flush();
                    //writer.WriteLine("AveragePaths: ");
                    //for (int i1 = 0; i1 < request.Result.AveragePaths.GetLength(0); i1++)
                    //{
                    //    for (int i2 = 0; i2 < request.Result.AveragePaths.GetLength(1); i2++)
                    //    {
                    //        writer.WriteLine("[" + i1 + "," + i2 + "]" + ": " + request.Result.AveragePaths[i1, i2]);
                    //    }
                    //}
                }

            }
        }
    }
}
