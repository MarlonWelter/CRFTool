using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CRFGraph = CodeBase.IGWGraph<CodeBase.ICRFNodeData, CodeBase.ICRFEdgeData, CodeBase.ICRFGraphData>;

namespace CRFBase.InferenceHeuristik
{
    public class Viterbi_Plus
    {
        public readonly int MaximalCombinationsUnderConsideration;
        public readonly int NumberLabels;
        private readonly List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> ChosenVertices = new List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
        internal List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> BoundaryVertices = new List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
        public List<CRFLabelling> Combinations = new List<CRFLabelling>();
        private LinkedList<CRFLabelling> NextGeneration = new LinkedList<CRFLabelling>();
        public double Barrier { get; set; }
        private const int RunsToDetermineBarrier = 100;
        private Random Random = new Random();
        public CRFGraph Graph { get; set; }
        public Func<double, double> GlobalScoreAdjuster { get; set; }
        public CRFResult PreResult { get; set; }
        public int PreResultPatchSize { get; set; }

        private Func<IList<IGWNode>, IEnumerable<IGWNode>, LinkedList<IGWNode>> computeQueue = GreedyMinBorderQueueComputing.ComputeQueue;
        public Func<IList<IGWNode>, IEnumerable<IGWNode>, LinkedList<IGWNode>> ComputeQueue
        {
            get { return computeQueue; }
            set { computeQueue = value; }
        }


        public Viterbi_Plus(int maxCombinationsUnderConsideration, int labels)
        {
            NumberLabels = labels;
            MaximalCombinationsUnderConsideration = maxCombinationsUnderConsideration;

            AnnounceNewCombination.AddPath(newCombination => NextGeneration.AddLast(newCombination));
        }

        public CRFResult Run(CRFGraph graph, CRFResult preResult, Func<double, double> globalScoreAdjuster)
        {
            GlobalScoreAdjuster = globalScoreAdjuster;
            PreResult = preResult;
            Graph = graph;
            Combinations.Clear();
            graph.Nodes.Each(n => n.Data.IsChosen = false);
            // compute queue with random starting point

            var vertexQueue = RandomGrowing.ComputeQueue(graph.Nodes.Cast<IGWNode>().ToList()).Cast<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>().ToList();

            //preparation
            PreResultPatchSize = PreResult.Labeling.Sum();
            int counter = 0;
            foreach (var item in vertexQueue)
            {
                counter++;
                item.Data.UnchosenNeighboursTemp = item.Edges.Where(e => !item.Neighbour(e).Data.IsChosen).Count();
            }

            //Create starting Combination
            {
                CRFLabelling newCombination = new CRFLabelling();
                newCombination.AssignedLabels = new int[Graph.Nodes.Count()];

                newCombination.BorderFingerPrint = 0;
                Combinations.Add(newCombination);
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int counter2 = 0;
            foreach (var vertex in vertexQueue)
            {
                if (PreResult.Labeling[vertex.GraphId] == 1)
                {
                    PreResultPatchSize--;
                }

                vertex.Data.IsChosen = true;
                var NewInnerVertices = ComputeNewBoundaryVertices(vertex);

                Barrier = DetermineBarrier(Combinations, vertex, NewInnerVertices.Count == 0, MaximalCombinationsUnderConsideration, RunsToDetermineBarrier);

                Do(Combinations, vertex, NewInnerVertices, comb => (comb.LocalScore > Barrier || (comb.LocalScore == Barrier && Random.NextDouble() > 0.5)), BoundaryVertices);

                //filter step two
                //this is needed, because of the heuristik the average combinations still grow if(NewInnerVertices == 1)
                if (NextGeneration.Count > MaximalCombinationsUnderConsideration)
                {
                    var ng = NextGeneration.ToList();
                    var barrier2 = DetermineBarrierStep2(ng, MaximalCombinationsUnderConsideration, RunsToDetermineBarrier);
                    NextGeneration = new LinkedList<CRFLabelling>(ng.Where(item => item.Score > barrier2 || (item.Score == barrier2 && Random.NextDouble() > 0.5)));
                }

                Combinations = NextGeneration.ToList();
                NextGeneration.Clear();

                counter2++;
            }
            watch.Stop();

            //return result
            var result = new CRFResult();
            result.Labeling = new int[vertexQueue.Count];
            var winner = Combinations[0];
            foreach (var node in vertexQueue)
            {
                result.Labeling[node.GraphId] = winner.AssignedLabels[node.GraphId];
            }
            result.RunTime = watch.Elapsed;
            result.Score = winner.Score;

            return result;
        }

        private double DetermineBarrierStep2(List<CRFLabelling> Combinations, int MaximalCombinationsUnderConsideration, int RunsToDetermineBarrier)
        {

            var surviveRatio = ((double)MaximalCombinationsUnderConsideration) / (Combinations.Count);

            var sample = Combinations.RandomTake(RunsToDetermineBarrier, Random).ToList();

            double barrier = 0.0;
            int sampleCount = sample.Count;

            int survivors = (int)(surviveRatio * sampleCount) + 1;
            LinkedList<double> Scores = new LinkedList<double>();

            Random rand = new Random();


            foreach (var item in sample)
            {
                LinkedListHandler.SortedInsert(Scores, item.Score, survivors);
            }
            barrier = (Scores.Last.Value + Scores.Last.Previous.Value) / 2.0;

            return barrier;
        }

        private double DetermineBarrier(List<CRFLabelling> Combinations, IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> vertex, bool needBarrier, int MaximalCombinationsUnderConsideration, int RunsToDetermineBarrier)
        {
            if (!needBarrier)
            {
                return double.MinValue;
            }

            var surviveRatio = ((double)MaximalCombinationsUnderConsideration) / (Combinations.Count * NumberLabels);

            if (surviveRatio >= 1.0)
            {
                return double.MinValue;
            }

            var sample = Combinations.RandomTake(RunsToDetermineBarrier, Random).ToList();

            double barrier = 0.0;
            int sampleCount = sample.Count;

            int survivors = (int)(surviveRatio * sampleCount) + 1;
            LinkedList<double> Scores = new LinkedList<double>();

            Random rand = new Random();

            var scoringEdges = vertex.Edges.Where(e => vertex.Neighbour(e).Data.IsChosen).ToList();

            foreach (var item in sample)
            {
                int label = Random.Next(NumberLabels);
                var score = item.LocalScore + vertex.Data.Score(label);
                foreach (var edge in scoringEdges)
                {
                    if (edge.Foot.Equals(vertex))
                    {
                        score += edge.Score(item.AssignedLabels[edge.Head.GraphId], label);
                    }
                    else
                    {
                        score += edge.Score(label, item.AssignedLabels[edge.Foot.GraphId]);
                    }
                }
                // add GlobalScoreAdjuster 
                score += GlobalScoreAdjuster(((double)(PreResultPatchSize + item.AssignedLabels.Sum() + label)) / Graph.Nodes.Count());

                LinkedListHandler.SortedInsert(Scores, score, survivors);
            }
            barrier = (Scores.Last.Value + Scores.Last.Previous.Value) / 2.0;

            return barrier;
        }
        public void ClearInput()
        {
            ChosenVertices.Clear();
            BoundaryVertices.Clear();
            Combinations.Clear();
        }

        private LinkedList<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> ComputeNewBoundaryVertices(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> chosenVertex)
        {
            var NewInnerVertices = new LinkedList<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();

            foreach (var edge in chosenVertex.Edges)
            {
                var otherVertex = chosenVertex.Neighbour(edge);
                otherVertex.Data.UnchosenNeighboursTemp--;
                if (otherVertex.Data.UnchosenNeighboursTemp == 0)
                {
                    BoundaryVertices.Remove(otherVertex);
                    NewInnerVertices.AddLast(otherVertex);
                }
            }

            if (chosenVertex.Data.UnchosenNeighboursTemp > 0)
            {
                BoundaryVertices.Add(chosenVertex);
            }
            else
            {
                NewInnerVertices.AddLast(chosenVertex);
            }

            return NewInnerVertices;
        }

        public Func<CRFLabelling, bool> CombinationFilter { get; set; }
        public MEvent<CRFLabelling> AnnounceNewCombination { get; set; } = new MEvent<CRFLabelling>();

        public void Do(IList<CRFLabelling> Combinations, IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> chosenVertex, LinkedList<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> NewInnerVertices, Func<CRFLabelling, bool> combinationFilter, IList<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> border)
        {
            CombinationFilter = combinationFilter;
            Do(Combinations, chosenVertex, NewInnerVertices, border);
        }
        public void Do(IList<CRFLabelling> Combinations, IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> chosenVertex, LinkedList<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> NewInnerVertices, IList<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> border)
        {

            bool hasNewInnerVertices = NewInnerVertices.Count > 0;
            var validNewCombinations = new Dictionary<long, CRFLabelling>();

            foreach (CRFLabelling combination in Combinations)
            {
                LinkedList<CRFLabelling> newCombinations = CreateNextGeneration(chosenVertex, combination);


                foreach (CRFLabelling newCombination in newCombinations)
                {
                    if (!CombinationFilter(newCombination))
                    {
                        continue;
                    }

                    BitArray array = new BitArray(border.Count);
                    byte[] ar2 = new byte[2];

                    long fp = 0;
                    for (int index = 0; index < border.Count; index++)
                    {
                        fp = fp << 1;
                        fp += newCombination.AssignedLabels[border[index].GraphId];
                    }
                    newCombination.BorderFingerPrint = fp;

                    if (validNewCombinations.ContainsKey(fp))
                    {
                        if (newCombination.Score > validNewCombinations[fp].Score)
                        {
                            validNewCombinations[fp] = newCombination;
                        }
                    }
                    else
                    {
                        validNewCombinations.Add(fp, newCombination);
                    }
                }
            }

            foreach (var comb in validNewCombinations.Values)
            {
                AnnounceNewCombination.Enter(comb);
            }
        }
        public LinkedList<CRFLabelling> CreateNextGeneration(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> chosenVertex,
               CRFLabelling oldCombination)
        {

            LinkedList<CRFLabelling> newCombinations = new LinkedList<CRFLabelling>();
            for (int counter = 0; counter < NumberLabels; counter++)
            {
                CRFLabelling newCombination = default(CRFLabelling);
                if (counter == NumberLabels - 1)
                {
                    newCombination = new CRFLabelling();
                    newCombination.LocalScore = oldCombination.LocalScore;
                    newCombination.AssignedLabels = oldCombination.AssignedLabels;
                }
                else
                {
                    newCombination = new CRFLabelling(oldCombination);
                    newCombination.LocalScore = oldCombination.LocalScore;
                }
                newCombination.AssignLabel(chosenVertex, counter);
                newCombination.GlobalScore = GlobalScoreAdjuster(((double)(PreResultPatchSize + newCombination.AssignedLabels.Sum())) / Graph.Nodes.Count());
                newCombinations.AddLast(newCombination);
            }

            return newCombinations;
        }
    }
}
