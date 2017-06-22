using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeBase.Graph
{
    public class ClusterDetection<NodeData, EdgeData, GraphData>
    {
        //public void Do(GWGraph<Node3DWrap<NodeData>, EdgeData, GraphData> graph)
        //{
        //    var rdm = new Random();
        //    int[] clusterMap = new int[graph.Nodes.Count()];
        //    int[] clusterCounts = new int[clusterMap.Length];


        //    foreach (var node in graph.Nodes)
        //    {
        //        node.Data.ClusterId = node.GraphId;
        //    }

        //    for (int i = 0; i < clusterMap.Length; i++)
        //    {
        //        clusterMap[i] = i;
        //        clusterCounts[i] = 1;
        //    }

        //    var totalPossibleEdges = (clusterMap.Length * (clusterMap.Length - 1)) / 2;
        //    var ClusterEdges = 0.0;
        //    var clusterEdgesPossible = 0.0;
        //    var otherEdges = 0.0;
        //    var totalOtherEdges = 0.0;
        //    foreach (var edge in graph.Edges)
        //    {
        //        if (clusterMap[edge.Foot.GraphId] == clusterMap[edge.Head.GraphId])
        //        {
        //            ClusterEdges++;
        //        }
        //        else
        //            otherEdges++;
        //    }

        //    for (int i = 0; i < clusterCounts.Length; i++)
        //    {
        //        clusterEdgesPossible += (clusterCounts[i] * (clusterCounts[i] - 1)) / 2;
        //    }
        //    totalOtherEdges = totalPossibleEdges - clusterEdgesPossible;

        //    var clusterEdgeProb = clusterEdgesPossible == 0 ? 0 : ClusterEdges / clusterEdgesPossible;
        //    var interClusterEdgesProb = otherEdges / totalOtherEdges;

        //    var score = clusterEdgeProb / interClusterEdgesProb;
        //    var nodes = graph.Nodes.ToList().RemoveWhere(n => !n.Neighbours.Any()).ToList();

        //    for (int step = 0; step < 100000; step++)
        //    {
        //        var selectedNode = nodes[rdm.Next(nodes.Count)];

        //        var possibleClusters = selectedNode.Neighbours.Select(nb => clusterMap[nb.GraphId]).ToList();
        //        if (possibleClusters.Count() == 1 && clusterMap[selectedNode.GraphId] == possibleClusters.First())
        //        {
        //            step--;
        //            continue;
        //        }

        //        var selectedCluster = 0;// possibleClusters[rdm.Next(possibleClusters.Count)];

        //        var clusterMappingScore = new Dictionary<int, double>();
        //        var tempScore = 0.0;

        //        foreach (var cluster in possibleClusters)
        //        {
        //            selectedCluster = cluster;
        //            //Preview Scores

        //            if (clusterMap[selectedNode.GraphId] == selectedCluster)
        //            {
        //                clusterMappingScore.Add(selectedCluster, score);
        //                continue;
        //            }

        //            var scoreLocal = 1.0;

        //            var changeClusterEdges = 0;
        //            foreach (var nb in selectedNode.Neighbours)
        //            {
        //                if (clusterMap[nb.GraphId] == clusterMap[selectedNode.GraphId])
        //                {
        //                    changeClusterEdges--;
        //                    scoreLocal *= 0.3;
        //                }
        //                else if (clusterMap[nb.GraphId] == selectedCluster)
        //                {
        //                    changeClusterEdges++;
        //                    scoreLocal *= 3.0;
        //                }
        //            }
        //            var changePossibleClusterEdges = 0;

        //            changePossibleClusterEdges -= clusterCounts[selectedNode.GraphId] > 1 ?
        //                (clusterCounts[selectedNode.GraphId] * (clusterCounts[selectedNode.GraphId] - 1)) / 2 - ((clusterCounts[selectedNode.GraphId] - 1) * (clusterCounts[selectedNode.GraphId] - 2)) / 2 : 0;

        //            changePossibleClusterEdges -= clusterCounts[selectedCluster] > 0 ?
        //                (clusterCounts[selectedCluster] * (clusterCounts[selectedCluster] - 1)) / 2 - ((clusterCounts[selectedCluster] + 1) * (clusterCounts[selectedCluster])) / 2 : 0;

        //            var clusterEdgeProbLocal = clusterEdgesPossible + changePossibleClusterEdges == 0 ? 0 : (ClusterEdges + changeClusterEdges) / (clusterEdgesPossible + changePossibleClusterEdges);
        //            var interClusterEdgesProbLocal = (otherEdges - changeClusterEdges) / totalOtherEdges;

        //            //var scoreLocal = Math.Max(0, changeClusterEdges); //(clusterEdgeProbLocal / interClusterEdgesProbLocal, 2);

        //            clusterMappingScore.Add(selectedCluster, scoreLocal);
        //            tempScore += scoreLocal;
        //        }



        //        //execute best score
        //        selectedCluster = clusterMappingScore.ChooseByDistributionNormalize(); // clusterMappingScore.First(kvp => kvp.Value == clusterMappingScore.Values.Max()).Key;

        //        if (clusterMap[selectedNode.GraphId] == selectedCluster)
        //        {
        //            continue;
        //        }

        //        var temp = step;

        //        var changeClusterEdges2 = 0;
        //        foreach (var nb in selectedNode.Neighbours)
        //        {
        //            if (clusterMap[nb.GraphId] == clusterMap[selectedNode.GraphId])
        //                changeClusterEdges2--;
        //            else if (clusterMap[nb.GraphId] == selectedCluster)
        //                changeClusterEdges2++;
        //        }
        //        var changePossibleClusterEdges2 = 0;

        //        changePossibleClusterEdges2 -= clusterCounts[selectedNode.GraphId] > 1 ?
        //            (clusterCounts[selectedNode.GraphId] * (clusterCounts[selectedNode.GraphId] - 1)) / 2 - ((clusterCounts[selectedNode.GraphId] - 1) * (clusterCounts[selectedNode.GraphId] - 2)) / 2 : 0;

        //        changePossibleClusterEdges2 -= clusterCounts[selectedCluster] > 0 ?
        //            (clusterCounts[selectedCluster] * (clusterCounts[selectedCluster] - 1)) / 2 - ((clusterCounts[selectedCluster] + 1) * (clusterCounts[selectedCluster])) / 2 : 0;

        //        var clusterEdgeProbLocal2 = clusterEdgesPossible + changePossibleClusterEdges2 == 0 ? 0 : (ClusterEdges + changeClusterEdges2) / (clusterEdgesPossible + changePossibleClusterEdges2);
        //        var interClusterEdgesProbLocal2 = (otherEdges - changeClusterEdges2) / totalOtherEdges;

        //        var scoreLocal2 = clusterEdgeProbLocal2 / interClusterEdgesProbLocal2;

        //        score = scoreLocal2;
        //        ClusterEdges += changeClusterEdges2;
        //        clusterEdgesPossible += changePossibleClusterEdges2;
        //        otherEdges -= changeClusterEdges2;
        //        selectedNode.Data.ClusterId = selectedCluster;
        //        clusterCounts[clusterMap[selectedNode.GraphId]]--;
        //        clusterCounts[selectedCluster]++;
        //        clusterMap[selectedNode.GraphId] = selectedCluster;

        //        Thread.Sleep(10);
        //    }
        //}
        public void Do(GWGraph<Node3DWrap<NodeData>, Edge3DWrap<EdgeData>, GraphData> graph)
        {
            var rdm = new Random();
            int[] clusterMap = new int[graph.Nodes.Count()];
            int[] clusterCounts = new int[clusterMap.Length];


            foreach (var node in graph.Nodes)
            {
                node.Data.ClusterId = node.GraphId;
            }

            for (int i = 0; i < clusterMap.Length; i++)
            {
                clusterMap[i] = i;
                clusterCounts[i] = 1;
            }

            var totalPossibleEdges = (clusterMap.Length * (clusterMap.Length - 1)) / 2;
            var ClusterEdges = 0.0;
            var clusterEdgesPossible = 0.0;
            var otherEdges = 0.0;
            var totalOtherEdges = 0.0;
            foreach (var edge in graph.Edges)
            {
                if (clusterMap[edge.Foot.GraphId] == clusterMap[edge.Head.GraphId])
                {
                    ClusterEdges++;
                }
                else
                    otherEdges++;
            }

            for (int i = 0; i < clusterCounts.Length; i++)
            {
                clusterEdgesPossible += (clusterCounts[i] * (clusterCounts[i] - 1)) / 2;
            }
            totalOtherEdges = totalPossibleEdges - clusterEdgesPossible;

            var clusterEdgeProb = clusterEdgesPossible == 0 ? 0 : ClusterEdges / clusterEdgesPossible;
            var interClusterEdgesProb = otherEdges / totalOtherEdges;

            var score = clusterEdgeProb / interClusterEdgesProb;
            var nodes = graph.Nodes.ToList().RemoveWhere(n => !n.Neighbours.Any()).ToList();

            for (int step = 0; step < 100000; step++)
            {
                var selectedNode = nodes[rdm.Next(nodes.Count)];

                var possibleClusters = selectedNode.Neighbours.Select(nb => clusterMap[nb.GraphId]).Distinct().ToList();
                if (possibleClusters.Count() == 1 && clusterMap[selectedNode.GraphId] == possibleClusters.First())
                {
                    step--;
                    continue;
                }

                var selectedCluster = 0;// possibleClusters[rdm.Next(possibleClusters.Count)];

                var clusterMappingScore = new Dictionary<int, double>();
                var tempScore = 0.0;

                foreach (var cluster in possibleClusters)
                {
                    selectedCluster = cluster;
                    //Preview Scores

                    if (clusterMap[selectedNode.GraphId] == selectedCluster)
                    {
                        clusterMappingScore.Add(selectedCluster, score);
                        continue;
                    }

                    var scoreLocal = 1.0;

                    var changeClusterEdges = 0;
                    foreach (var nb in selectedNode.Neighbours)
                    {
                        if (clusterMap[nb.GraphId] == clusterMap[selectedNode.GraphId])
                        {
                            changeClusterEdges--;
                            scoreLocal *= 0.3;
                        }
                        else if (clusterMap[nb.GraphId] == selectedCluster)
                        {
                            changeClusterEdges++;
                            scoreLocal *= 3.0;
                        }
                    }
                    var changePossibleClusterEdges = 0;

                    changePossibleClusterEdges -= clusterCounts[selectedNode.GraphId] > 1 ?
                        (clusterCounts[selectedNode.GraphId] * (clusterCounts[selectedNode.GraphId] - 1)) / 2 - ((clusterCounts[selectedNode.GraphId] - 1) * (clusterCounts[selectedNode.GraphId] - 2)) / 2 : 0;

                    changePossibleClusterEdges -= clusterCounts[selectedCluster] > 0 ?
                        (clusterCounts[selectedCluster] * (clusterCounts[selectedCluster] - 1)) / 2 - ((clusterCounts[selectedCluster] + 1) * (clusterCounts[selectedCluster])) / 2 : 0;

                    var clusterEdgeProbLocal = clusterEdgesPossible + changePossibleClusterEdges == 0 ? 0 : (ClusterEdges + changeClusterEdges) / (clusterEdgesPossible + changePossibleClusterEdges);
                    var interClusterEdgesProbLocal = (otherEdges - changeClusterEdges) / totalOtherEdges;

                    //var scoreLocal = Math.Max(0, changeClusterEdges); //(clusterEdgeProbLocal / interClusterEdgesProbLocal, 2);

                    clusterMappingScore.Add(selectedCluster, scoreLocal);
                    tempScore += scoreLocal;
                }



                //execute best score
                selectedCluster = clusterMappingScore.ChooseByDistributionNormalize(); // clusterMappingScore.First(kvp => kvp.Value == clusterMappingScore.Values.Max()).Key;

                if (clusterMap[selectedNode.GraphId] == selectedCluster)
                {
                    continue;
                }

                var temp = step;

                var changeClusterEdges2 = 0;
                foreach (var nb in selectedNode.Neighbours)
                {
                    if (clusterMap[nb.GraphId] == clusterMap[selectedNode.GraphId])
                        changeClusterEdges2--;
                    else if (clusterMap[nb.GraphId] == selectedCluster)
                        changeClusterEdges2++;
                }
                var changePossibleClusterEdges2 = 0;

                changePossibleClusterEdges2 -= clusterCounts[selectedNode.GraphId] > 1 ?
                    (clusterCounts[selectedNode.GraphId] * (clusterCounts[selectedNode.GraphId] - 1)) / 2 - ((clusterCounts[selectedNode.GraphId] - 1) * (clusterCounts[selectedNode.GraphId] - 2)) / 2 : 0;

                changePossibleClusterEdges2 -= clusterCounts[selectedCluster] > 0 ?
                    (clusterCounts[selectedCluster] * (clusterCounts[selectedCluster] - 1)) / 2 - ((clusterCounts[selectedCluster] + 1) * (clusterCounts[selectedCluster])) / 2 : 0;

                var clusterEdgeProbLocal2 = clusterEdgesPossible + changePossibleClusterEdges2 == 0 ? 0 : (ClusterEdges + changeClusterEdges2) / (clusterEdgesPossible + changePossibleClusterEdges2);
                var interClusterEdgesProbLocal2 = (otherEdges - changeClusterEdges2) / totalOtherEdges;

                var scoreLocal2 = clusterEdgeProbLocal2 / interClusterEdgesProbLocal2;

                score = scoreLocal2;
                ClusterEdges += changeClusterEdges2;
                clusterEdgesPossible += changePossibleClusterEdges2;
                otherEdges -= changeClusterEdges2;
                selectedNode.Data.ClusterId = selectedCluster;
                clusterCounts[clusterMap[selectedNode.GraphId]]--;
                clusterCounts[selectedCluster]++;
                clusterMap[selectedNode.GraphId] = selectedCluster;

                Thread.Sleep(10);
            }
        }

        public void DoMCMCStyle(GWGraph<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo> graph, int steps, double ChangePenalty)
        {
            var rdm = new Random();
            int[] stateMap = new int[graph.Nodes.Count()];
            int[] stateCounts = new int[stateMap.Length];
            int numberStates = stateMap.Length;


            foreach (var node in graph.Nodes)
            {
                node.Data.ClusterId = node.GraphId;
                node.Data.CurrentState = node.GraphId;
                node.Data.LastUpdate = 0;
                node.Data.InStateCounts = new int[numberStates];
                node.Data.StateScores = new double[numberStates];
                for (int st = 0; st < numberStates; st++)
                {
                    node.Data.StateScores[st] = 1.0 / numberStates;
                }
            }
            foreach (var edge in graph.Edges)
            {
                edge.Data.StateScores = new double[numberStates, numberStates];
                for (int st1 = 0; st1 < numberStates; st1++)
                {
                    for (int st2 = 0; st2 < numberStates; st2++)
                    {
                        if (st1 == st2)
                        {
                            edge.Data.StateScores[st1, st2] = 1.0;
                        }
                        else
                        {
                            edge.Data.StateScores[st1, st2] = ChangePenalty;
                        }
                    }
                }
            }

            for (int i = 0; i < stateMap.Length; i++)
            {
                stateMap[i] = i;
                stateCounts[i] = 1;
            }

            var nodes = graph.Nodes.ToList().RemoveWhere(n => !n.Neighbours.Any()).ToList();

            for (int step = 1; step <= steps; step++)
            {
                var selectedNode = nodes[rdm.Next(nodes.Count)];

                var possibleNewState = rdm.Next(numberStates);
                var oldState = selectedNode.Data.CurrentState;
                if (possibleNewState == oldState)
                    continue;

                var oldScore = 1.0;
                oldScore *= selectedNode.Data.StateScores[selectedNode.Data.CurrentState];
                foreach (var edge in selectedNode.Edges)
                {
                    oldScore *= edge.Data.StateScores[edge.Head.Data.CurrentState, edge.Foot.Data.CurrentState];
                }
                selectedNode.Data.CurrentState = possibleNewState;
                var newScore = 1.0;
                oldScore *= selectedNode.Data.StateScores[selectedNode.Data.CurrentState];
                foreach (var edge in selectedNode.Edges)
                {
                    newScore *= edge.Data.StateScores[edge.Head.Data.CurrentState, edge.Foot.Data.CurrentState];
                }


                if (rdm.NextDouble() < oldScore / (newScore + oldScore))
                {
                    selectedNode.Data.CurrentState = oldState;
                }

                selectedNode.Data.InStateCounts[selectedNode.Data.CurrentState] += step - selectedNode.Data.LastUpdate;
                selectedNode.Data.LastUpdate = step;

                if (step % 250 == 0)
                {
                    foreach (var node in nodes)
                    {
                        node.Data.ClusterId = node.Data.InStateCounts.MaxEntry(st => st);
                    }
                    Thread.Sleep(100);
                }
            }

            foreach (var node in nodes)
            {
                node.Data.ClusterId = node.Data.InStateCounts.MaxEntry(st => st);
            }

        }
    }
}
