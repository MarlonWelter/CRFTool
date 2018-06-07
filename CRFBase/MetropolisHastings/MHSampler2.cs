using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using CRFBase;
using CRFGraph = CodeBase.IGWGraph<CodeBase.SGLNodeData, CodeBase.SGLEdgeData, CodeBase.SGLGraphData>;

namespace CRFBase
{
    public class MHSampler2
    {

        public List<CRFGraph> GraphClones { get; private set; }

        public List<int[]> FinalSample { get; private set; }
        public List<int[]>[] GraphCloneSamples { get; private set; }


        private static int NumberLabels = 2;

        private static Random random;
        public static Random Random
        {
            get { return (random = random ?? new Random()); }
            set { random = value; }
        }

        public void Do(MHSampler2Parameters parameters)
        {
            Log.Post("MHSampler - Start");
            Build.Do();
            GraphClones = new List<IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>>();

            //get parameter values
            var numberChains = parameters.NumberChains;
            var graph = parameters.Graph;

            //create clones
            GraphClones.Add(graph);
            var catGraphCreator = new CategoryGraphCreator();
            for (int i = 1; i < numberChains; i++)
            {
                var clone = graph.Clone(GraphDataConverter.SGLNodeDataConvert, GraphDataConverter.SGLEdgeDataConvert, GraphDataConverter.SGLGraphDataConvert);
                //catGraphCreator.CreateCategoryGraph(clone);
                GraphClones.Add(clone);
            }

            //init testsamples
            GraphCloneSamples = new List<int[]>[GraphClones.Count];
            for (int i = 0; i < GraphCloneSamples.Length; i++)
            {
                GraphCloneSamples[i] = new List<int[]>();
            }

            //start chains
            for (int i = 0; i < GraphClones.Count; i++)
            {
                var chain = GraphClones[i];
                //start the first chain with viterbi
                switch (parameters.MHSampler2StartPoint)
                {
                    case MHSampler2StartPoint.Random:
                        FindStartPointRandom(chain);
                        break;
                    case MHSampler2StartPoint.Viterbi:
                        FindStartPointViterbi(chain);
                        break;
                    default: break;
                }
            }

            //iterate over prerun
            for (int i = 0; i < parameters.PreRunLength; i++)
            {
                foreach (var chain in GraphClones)
                {
                    ChangePosition(chain, 0);
                }
            }

            //iterate over prerun
            for (int i = 0; i < GraphClones.Count; i++)
            {
                var chain = GraphClones[i];
                for (int k = 0; k < parameters.RunLength; k++)
                {
                    ChangePosition(chain, 0);
                    var sample = new int[chain.Nodes.Count()];
                    foreach (var node in chain.Nodes)
                    {
                        sample[node.GraphId] = node.Data.TempAssign;
                    }
                    GraphCloneSamples[i].Add(sample);
                }
            }

            // shuffle all samples into one sample.
            FinalSample = GraphCloneSamples.SelectMany(s => s).RandomizeOrder().ToList();
        }


        private void FindStartPointNodeProb(CRFGraph graph)
        {

            foreach (var item in graph.Nodes)
            {
                item.Data.TempAssign = Math.Log(Random.NextDouble()) < item.Data.Score(0) ? 0 : 1;
            }
        }

        private void FindStartPointRandom(CRFGraph graph)
        {
            foreach (var item in graph.Nodes)
            {
                item.Data.TempAssign = Random.Next(2);
            }
        }


        private void FindStartPointAlternating(CRFGraph graph)
        {
            int state = 0;
            foreach (var item in graph.Nodes)
            {
                item.Data.TempAssign = state;
                state = (state + 1) % 2;
            }
        }



        private void FindStartPointViterbi(CRFGraph graph)
        {
            var request = new SolveInference(graph, NumberLabels);
            request.RequestInDefaultContext();
            var resultLabeling = request.Solution.Labeling;

            foreach (var item in graph.Nodes)
            {
                item.Data.TempAssign = resultLabeling[item.GraphId];
            }
        }
        

        private void ChangePosition(CRFGraph graph, int time)
        {
            //jedes proposal gleich wahrscheinlich
            var p = (double)graph.Nodes.Count() / (graph.Data.NumberCategories + 2 * graph.Nodes.Count());
            var random = Random.NextDouble();
            if (random <= p)
            {
                //CountTransitionNodeProposal++;
                ChangePositionNode(graph, time);
            }
            else if (random > p && random <= p * 2)
            {
                //CountTransitionNeighborhoodProposal++;
                ChangePositionNeighborhood(graph, time);
            }
            else
            {
                //CountTransitionCategoryProposal++;
                if (graph.Data.NumberCategories > 1)
                    ChangePositionCategory(graph, time);
                else
                    ChangePositionNode(graph, time);
            }


            //out of perRun , collect sample for tests
            if (time > 0)
            {
                //get list of samples for current graph
                var samples = GraphCloneSamples[GraphClones.IndexOf(graph)];

                int[] sample = new int[graph.Nodes.Count()];
                foreach (var node in graph.Nodes)
                {
                    sample[node.GraphId] = node.Data.TempAssign;
                }

                samples.Add(sample);
            }

        }



        private void ChangePositionNode(CRFGraph graph, int time)
        {

            var node = graph.Nodes.ToList().RandomElement(Random);
            //var score = assignment.TakeScore();

            //compute relative change
            var relChange = 0.0;
            var oldScore = 0.0;
            var newScore = 0.0;

            oldScore += node.Data.Score(node.Data.TempAssign);
            foreach (var edge in node.Edges)
            {
                oldScore += edge.TempScore();
            }


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

            newScore += node.Data.Score(node.Data.TempAssign);
            foreach (var edge in node.Edges)
            {
                newScore += edge.TempScore();
            }


            if (Random.NextDouble() <= (Math.Exp(newScore)) / (Math.Exp(oldScore)))
            {
                //state changed
                //TODO check for correct counting
                node.Data.TempCount += (1 - node.Data.TempAssign) * (time - node.Data.LastUpdate);
                node.Data.LastUpdate = time;
                //CountTransitionNodeAccepted++;
            }
            else
            {
                //stay in state
                node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
            }
        }



        private void ChangePositionCategory(CRFGraph graph, int time)
        {

            //pick random category
            var category = Random.Next(graph.Data.NumberCategories);
            var catGraph = graph.Data.CategoryGraph;
            //node of categoryGraph contains all information
            var catNode = catGraph.Nodes.ToList().Find(n => (n.Data.Category == category));

            var relChange = 0.0;
            var oldScore = 0.0;
            var newScore = 0.0;

            //oldScore berechnen
            foreach (var node in catNode.Data.Nodes)
            {
                oldScore += node.Data.Score(node.Data.TempAssign);
            }

            foreach (var edge in catNode.Data.InterEdges)
            {
                oldScore += edge.TempScore();
            }


            //flip each node
            foreach (var node in catNode.Data.Nodes)
            {
                node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
            }


            //newScore berechnen
            foreach (var node in catNode.Data.Nodes)
            {
                newScore += node.Data.Score(node.Data.TempAssign);
            }

            foreach (var edge in catNode.Data.InterEdges)
            {
                newScore += edge.TempScore();
            }


            relChange = newScore - oldScore;
            if (Random.NextDouble() <= (Math.Exp(newScore)) / (Math.Exp(oldScore)))
            {
                //state changed
                //TODO check for correct counting
                foreach (var node in catNode.Data.Nodes)
                {
                    node.Data.TempCount += (1 - node.Data.TempAssign) * (time - node.Data.LastUpdate);
                    node.Data.LastUpdate = time;

                }
                //CountTransitionCategoryAccepted++;
            }
            else
            {
                //stay in state
                foreach (var node in catNode.Data.Nodes)
                {
                    node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
                }

            }


        }



        private void ChangePositionNeighborhood(CRFGraph graph, int time)
        {
            //knoten auswürfeln
            var node = graph.Nodes.ToList().RandomElement(Random);
            var neighbors = node.Neighbours;

            var relChange = 0.0;
            var oldScore = 0.0;
            var newScore = 0.0;

            //oldscore berechnen
            oldScore += node.Data.Score(node.Data.TempAssign);
            foreach (var neighbor in neighbors)
            {
                oldScore += neighbor.Data.Score(neighbor.Data.TempAssign);
                foreach (var edge in neighbor.Edges)
                {
                    //if (!neighbor.Neighbour(edge).Equals(node))
                    oldScore += edge.TempScore();
                }
            }


            //knoten + nachbarschaft flippen
            node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
            foreach (var neighbor in neighbors)
            {
                neighbor.Data.TempAssign = ((neighbor.Data.TempAssign + 1) % 2);
            }


            //newscore berechnen
            newScore += node.Data.Score(node.Data.TempAssign);
            foreach (var neighbor in neighbors)
            {
                newScore += neighbor.Data.Score(neighbor.Data.TempAssign);
                foreach (var edge in neighbor.Edges)
                {
                    //if (!neighbor.Neighbour(edge).Equals(node))
                    newScore += edge.TempScore();
                }
            }


            relChange = newScore - oldScore;

            if (Random.NextDouble() <= (Math.Exp(newScore)) / (Math.Exp(oldScore)))
            {
                //state changed
                //TODO check for correct counting
                node.Data.TempCount += (1 - node.Data.TempAssign) * (time - node.Data.LastUpdate);
                node.Data.LastUpdate = time;
                foreach (var neighbor in neighbors)
                {
                    neighbor.Data.TempCount += (1 - neighbor.Data.TempAssign) * (time - neighbor.Data.LastUpdate);
                    neighbor.Data.LastUpdate = time;

                }
                //CountTransitionNeighborhoodAccepted++;
            }
            else
            {
                //stay in state
                node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
                foreach (var neighbor in neighbors)
                {
                    neighbor.Data.TempAssign = ((neighbor.Data.TempAssign + 1) % 2);
                }

            }


        }
    }

}
