//using General.Events;
//using General.MathCompartement;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Units;

//namespace CRFBase
//{
//    public class ComputeNewCombinations
//    {
//        public ComputeNewCombinations(int states)
//        {
//            PossibleStatesForVertices = states;
//        }
//        public Func<Combination, bool> CombinationFilter { get; set; }
//        public readonly MEvent<Combination> AnnounceNewCombination = new MEvent<Combination>();

//        public int PossibleStatesForVertices { get; set; }
//        public void Do(IList<Combination> Combinations, CRFNode chosenVertex, LinkedList<CRFNode> NewInnerVertices, Func<Combination, bool> combinationFilter, IEnumerable<CRFNode> border)
//        {
//            CombinationFilter = combinationFilter;
//            Do(Combinations, chosenVertex, NewInnerVertices, border);
//        }
//        public void Do(IList<Combination> Combinations, CRFNode chosenVertex, LinkedList<CRFNode> NewInnerVertices, IEnumerable<CRFNode> border)
//        {

//            bool hasNewInnerVertices = NewInnerVertices.Count > 0;
//            var validNewCombinations = new Dictionary<int, Combination>();
//            int borderSize = 0;

//            foreach (Combination combination in Combinations)
//            {
//                LinkedList<Combination> newCombinations = CreateNextGeneration(chosenVertex, combination);
                

//                foreach (Combination newCombination in newCombinations)
//                {
//                    if (!CombinationFilter(newCombination))
//                        continue;

//                    newCombination.BorderFingerPrint = 1;
//                    int indexer = 0;
//                    foreach (var node in border)
//                    {
//                        if (node.IsChosen && node.UnchosenNeighboursTemp > 0)
//                        {
//                            newCombination.BorderFingerPrint *= newCombination.Assignment[node.Ordinate] > 0 ? Primes.Numbers[indexer] : 1;
//                            indexer++;
//                        }
//                    }
//                    if (validNewCombinations.ContainsKey(newCombination.BorderFingerPrint))
//                    {
//                        if (newCombination.Score > validNewCombinations[newCombination.BorderFingerPrint].Score)
//                            validNewCombinations[newCombination.BorderFingerPrint] = newCombination;
//                    }
//                    else
//                        validNewCombinations.Add(newCombination.BorderFingerPrint, newCombination);
//                }

//            }
//            if (Combinations.Count == 0)
//            {

//                for (int state = 0; state < PossibleStatesForVertices; state++)
//                {
//                    Combination newCombination = new Combination();
//                    newCombination.AddScore(chosenVertex, state);

//                    newCombination.BorderFingerPrint = 1;
//                    int indexer = 0;
//                    foreach (var node in border)
//                    {
//                        if (node.IsChosen && node.UnchosenNeighboursTemp > 0)
//                        {
//                            newCombination.BorderFingerPrint *= newCombination.Assignment[node.Ordinate] > 0 ? Primes.Numbers[indexer] : 1;
//                            indexer++;
//                        }
//                    }

//                    if (validNewCombinations.ContainsKey(newCombination.BorderFingerPrint))
//                    {
//                        if (newCombination.Score > validNewCombinations[newCombination.BorderFingerPrint].Score)
//                            validNewCombinations[newCombination.BorderFingerPrint] = newCombination;
//                    }
//                    else
//                        validNewCombinations.Add(newCombination.BorderFingerPrint, newCombination);
//                }
//            }


//            foreach (var comb in validNewCombinations.Values)
//            {
//                AnnounceNewCombination.Enter(comb);
//            }
//        }
//        public LinkedList<Combination> CreateNextGeneration(CRFNode chosenVertex,
//               Combination oldCombination)
//        {

//            LinkedList<Combination> newCombinations = new LinkedList<Combination>();
//            for (int counter = 0; counter < PossibleStatesForVertices; counter++)
//            {
//                Combination newCombination = new Combination();
//                newCombination.Score = oldCombination.Score;
//                newCombination.Assignment = oldCombination.Assignment.ToArray();

//                newCombination.AddScore(chosenVertex, counter);
//                newCombinations.AddLast(newCombination);
//            }

//            return newCombinations;
//        }
//    }
//}
