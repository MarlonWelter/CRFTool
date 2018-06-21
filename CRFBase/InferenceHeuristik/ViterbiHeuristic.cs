
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CodeBase;
using System.Diagnostics;
using CRFGraph = CodeBase.IGWGraph<CodeBase.ICRFNodeData, CodeBase.ICRFEdgeData, CodeBase.ICRFGraphData>;
using System.Collections;

namespace CRFBase
{
    class ViterbiHeuristic
    {
        public readonly int MaximalCombinationsUnderConsideration;
        public readonly int NumberLabels;
        readonly List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> ChosenVertices = new List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
        internal List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> BoundaryVertices = new List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
        public List<CRFLabelling> Combinations = new List<CRFLabelling>();
        LinkedList<CRFLabelling> NextGeneration = new LinkedList<CRFLabelling>();
        public double Barrier { get; set; }
        private const int RunsToDetermineBarrier = 100;
        private Random Random = new Random();
        public CRFGraph Graph { get; set; }
        
        private Func<IList<IGWNode>, IEnumerable<IGWNode>, LinkedList<IGWNode>> computeQueue = GreedyMinBorderQueueComputing.ComputeQueue;
        public Func<IList<IGWNode>, IEnumerable<IGWNode>, LinkedList<IGWNode>> ComputeQueue
        {
            get { return computeQueue; }
            set { computeQueue = value; }
        }


        public ViterbiHeuristic(int maxCombinationsUnderConsideration, int labels)
        {
            NumberLabels = labels;
            MaximalCombinationsUnderConsideration = maxCombinationsUnderConsideration;

            AnnounceNewCombination.AddPath(newCombination => NextGeneration.AddLast(newCombination));
        }

        public CRFResult Run(CRFGraph graph, IDictionary<IGWNode, int> startLabeling)
        {
            Graph = graph;
            graph.Nodes.Each(n => n.Data.IsChosen = false);
            var startPatch = (startLabeling != null) ? startLabeling.Keys.Cast<IGWNode>() : new List<IGWNode>();
            var vertexQueue = computeQueue(graph.Nodes.Cast<IGWNode>().ToList(), startPatch).Cast<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>().ToList();

            //preparation
            int counter = 0;
            if (startLabeling != null)
            {
                foreach (IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> item in startLabeling.Keys)
                {
                    item.Data.IsChosen = true;
                }
            }
            foreach (var item in vertexQueue)
            {
                counter++;
                item.Data.UnchosenNeighboursTemp = item.Edges.Where(e => !item.Neighbour(e).Data.IsChosen).Count();
            }

            //Create starting Combination
            {
                CRFLabelling newCombination = new CRFLabelling();
                newCombination.AssignedLabels = new int[Graph.Nodes.Count() - startPatch.Count()];

                var score = 0.0;
                if (startLabeling != null)
                {
                    foreach (var item in startLabeling)
                    {
                        var key = item.Key as IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>;
                        score += key.Data.Score(item.Value);
                    }

                    foreach (var edge in graph.Edges)
                    {
                        if (edge.Head.Data.IsChosen && edge.Foot.Data.IsChosen)
                        {
                            score += edge.Score(edge.Head.GraphId, startLabeling[edge.Head], edge.Foot.GraphId, startLabeling[edge.Foot]);
                        }
                    }
                }
                newCombination.BorderFingerPrint = 0;
                Combinations.Add(newCombination);
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int counter2 = 0;
            foreach (var vertex in vertexQueue)
            {

                vertex.Data.IsChosen = true;
                var NewInnerVertices = ComputeNewBoundaryVertices(vertex);

                Barrier = DetermineBarrier(Combinations, vertex, NewInnerVertices.Count == 0, MaximalCombinationsUnderConsideration, RunsToDetermineBarrier);

                Do(Combinations, vertex, NewInnerVertices, comb => (comb.Score > Barrier || (comb.Score == Barrier && Random.NextDouble() > 0.5)), BoundaryVertices);

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
            if (startLabeling != null)
            {
                foreach (var node in startLabeling)
                {
                    result.Labeling[node.Key.GraphId] = node.Value;
                }
            }
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
                return double.MinValue;

            var surviveRatio = ((double)MaximalCombinationsUnderConsideration) / (Combinations.Count * NumberLabels);

            if (surviveRatio >= 1.0)
                return double.MinValue;

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
                var score = item.Score + vertex.Data.Score(label);
                foreach (var edge in scoringEdges)
                {
                    if (edge.Foot.Equals(vertex))
                        score += edge.Score(item.AssignedLabels[edge.Head.GraphId], label);
                    else
                        score += edge.Score(label, item.AssignedLabels[edge.Foot.GraphId]);
                }
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
                BoundaryVertices.Add(chosenVertex);
            else
                NewInnerVertices.AddLast(chosenVertex);
            
            return NewInnerVertices;
        }

        public Func<CRFLabelling, bool> CombinationFilter { get; set; }
        public readonly MEvent<CRFLabelling> AnnounceNewCombination = new MEvent<CRFLabelling>();

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
                        continue;

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
                            validNewCombinations[fp] = newCombination;
                    }
                    else
                        validNewCombinations.Add(fp, newCombination);
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
                    newCombination.Score = oldCombination.Score;
                    newCombination.AssignedLabels = oldCombination.AssignedLabels;
                }
                else
                {
                    newCombination = new CRFLabelling(oldCombination);
                    newCombination.Score = oldCombination.Score;
                }
                newCombination.AddScore(chosenVertex, counter);
                newCombinations.AddLast(newCombination);
            }

            return newCombinations;
        }
    }
}
