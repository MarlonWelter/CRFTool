
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CodeBase;
using System.Diagnostics;
using CRFGraph = CodeBase.IGWGraph<CRFBase.ICRFNodeDataBinary, CRFBase.ICRFEdgeDataBinary, CRFBase.ICRFGraphData>;
using System.Collections;

namespace CRFBase
{
    class InferenceHeuristikBinary
    {
        public readonly int MaximalCombinationsUnderConsideration;
        readonly List<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>> ChosenVertices = new List<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>>();
        internal List<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>> BoundaryVertices = new List<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>>();
        public List<Combination> Combinations = new List<Combination>();
        LinkedList<Combination> NextGeneration = new LinkedList<Combination>();
        readonly Dictionary<string, Combination> BorderCombinations = new Dictionary<string, Combination>();
        public double Barrier { get; set; }
        private const int RunsToDetermineBarrier = 100;
        private Random Random = new Random();
        public CRFGraph Graph { get; set; }

        ////if this field is true, then after one run of the algorithm - the interface assignments will be reprocessed to a weighted voting by the top results
        //public const bool ReprocessTopResults = true;
        public const bool KeepOnlyTopResult = true;
        //private ComputeNewCombinations ComputeNewCombination = new ComputeNewCombinations(PossibleStatesForVertices);
        private Func<IList<IGWNode>, IEnumerable<IGWNode>, LinkedList<IGWNode>> computeQueue = GreedyMinBorderQueueComputing.ComputeQueue;
        public Func<IList<IGWNode>, IEnumerable<IGWNode>, LinkedList<IGWNode>> ComputeQueue
        {
            get { return computeQueue; }
            set { computeQueue = value; }
        }

        public InferenceHeuristikBinary(int bufferSize)
        {
            MaximalCombinationsUnderConsideration = bufferSize;
            AnnounceNewCombination.AddPath(newCombination => NextGeneration.AddLast(newCombination));
        }

        public CRFResult Run(CRFGraph graph)
        {
            Graph = graph;
            graph.Nodes.Each(n => { n.Data.IsChosen = false; });
            var vertexQueue = computeQueue(graph.Nodes.Cast<IGWNode>().ToList(), null).Cast<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>>().ToList();

            //preparation
            int counterp = 0;
            foreach (var item in vertexQueue)
            {
                item.Data.Ordinate = counterp;
                counterp++;
                item.Data.UnchosenNeighboursTemp = item.Edges.Where(e => !item.Neighbour(e).Data.IsChosen).Count();
            }

            //Create starting Combination
            {
                Combination newCombination = new Combination();
                newCombination.Assignment = new int[Graph.Nodes.Count()];


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
                    NextGeneration = new LinkedList<Combination>(ng.Where(item => item.Score > barrier2 || (item.Score == barrier2 && Random.NextDouble() > 0.5)));
                }
                if (NextGeneration.NullOrEmpty())
                {

                }

                Combinations = NextGeneration.ToList();
                NextGeneration.Clear();

                counter2++;
                //Log.Post("Vertex: " + vertex.Data.Id + " " + vertex.Data.Ordinate + @"/" + vertexQueue.Count);
                //Log.Post("Barrier: " + Barrier + "   Combinations: " + Combinations.Count);
            }
            watch.Stop();
            //Log.Post("Time Used: " + watch.ElapsedMilliseconds);

            //return result
            var result = new CRFResult();
            result.Labeling = new int[vertexQueue.Count];

            var winner = Combinations[0];
            foreach (var node in vertexQueue)
            {
                result.Labeling[node.Data.Ordinate] = winner.Assignment[node.GraphId];
            }
            result.RunTime = watch.Elapsed;
            result.Score = winner.Score;

            return result;
        }

        private double DetermineBarrierStep2(List<Combination> Combinations, int MaximalCombinationsUnderConsideration, int RunsToDetermineBarrier)
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

        private double DetermineBarrier(List<Combination> Combinations, IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> vertex, bool needBarrier, int MaximalCombinationsUnderConsideration, int RunsToDetermineBarrier)
        {
            if (!needBarrier)
                return double.MinValue;

            var surviveRatio = ((double)MaximalCombinationsUnderConsideration) / (Combinations.Count * 2);

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
                var label = Random.Next(2) > 0;
                var score = item.Score + vertex.Data.Score(label);
                foreach (var edge in scoringEdges)
                {
                    //if (edge.Foot.Equals(vertex))
                    //    score += edge.Score(item.Assignment[edge.Head.Data.Ordinate], label);
                    //else
                    //    score += edge.Score(label, item.Assignment[edge.Foot.Data.Ordinate]);
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

        private LinkedList<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>> ComputeNewBoundaryVertices(IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> chosenVertex)
        {
            var NewInnerVertices = new LinkedList<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>>();

            foreach (var edge in chosenVertex.Edges)
            {
                var otherVertex = chosenVertex.Neighbour(edge);
                //if (otherVertex.IsChosen)
                //    continue;
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

        public Func<Combination, bool> CombinationFilter { get; set; }
        public readonly MEvent<Combination> AnnounceNewCombination = new MEvent<Combination>();

        public void Do(IList<Combination> Combinations, IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> chosenVertex, LinkedList<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>> NewInnerVertices, Func<Combination, bool> combinationFilter, IList<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>> border)
        {
            CombinationFilter = combinationFilter;
            Do(Combinations, chosenVertex, NewInnerVertices, border);
        }
        public void Do(IList<Combination> Combinations, IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> chosenVertex, LinkedList<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>> NewInnerVertices, IList<IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData>> border)
        {

            bool hasNewInnerVertices = NewInnerVertices.Count > 0;
            var validNewCombinations = new Dictionary<long, Combination>();

            foreach (Combination combination in Combinations)
            {
                LinkedList<Combination> newCombinations = CreateNextGeneration(chosenVertex, combination);


                foreach (Combination newCombination in newCombinations)
                {
                    if (!CombinationFilter(newCombination))
                        continue;

                    BitArray array = new BitArray(border.Count);
                    byte[] ar2 = new byte[2];

                    long fp = 0;
                    for (int index = 0; index < border.Count; index++)
                    {
                        fp = fp << 1;
                        fp += newCombination.Assignment[border[index].GraphId];
                    }
                    newCombination.BorderFingerPrint = fp;

                    //newCombination.BorderFingerPrint = "";
                    //int indexer = 0;
                    //foreach (var node in border)
                    //{
                    //    if (node.Data.IsChosen && node.Data.UnchosenNeighboursTemp > 0)
                    //    {
                    //        newCombination.BorderFingerPrint += newCombination.Assignment[node.Data.Ordinate] > 0 ? "1" : "0";
                    //        indexer++;
                    //    }
                    //}
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
        public LinkedList<Combination> CreateNextGeneration(IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> chosenVertex,
               Combination oldCombination)
        {

            LinkedList<Combination> newCombinations = new LinkedList<Combination>();
            //for (int counter = 0; counter < PossibleStatesForVertices; counter++)
            {
                Combination newCombination = default(Combination);
                //if (counter == PossibleStatesForVertices - 1)
                {
                    newCombination = new Combination();
                    newCombination.Score = oldCombination.Score;
                    newCombination.Assignment = oldCombination.Assignment;
                    //newCombination.AddScore(chosenVertex, 1);
                    newCombinations.AddLast(newCombination);
                }
                //else
                {
                    newCombination = new Combination(oldCombination);
                    newCombination.Score = oldCombination.Score;
                    //newCombination.AddScore(chosenVertex, 0);
                    newCombinations.AddLast(newCombination);
                    //Buffer.BlockCopy(oldCombination.Assignment, 0, newCombination.Assignment, 0, oldCombination.Assignment.Length * 4);

                    //newCombination = new Combination();
                    //newCombination.Score = oldCombination.Score;
                    //newCombination.Assignment = oldCombination.Assignment.ToArray();
                }

            }

            return newCombinations;
        }
    }
}
