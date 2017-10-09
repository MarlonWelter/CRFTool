//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using General.General;
//using System.Threading.Tasks;
//using General.BasicOperation;
//using General.BasicObject;
//using General.Logging;
//using General;

//namespace CRFBase
//{
//    public class MarginalizeByResampling
//    {
//        public static double[] Do(IEnumerable<CRFNode> nodes, CRFNode nodeToMarginalize, MarginalizeByResamplingOptions options)
//        {
//            if (nodes.Count() == 1)
//                return nodes.First().Scores;

//            LinkedList<CRFNode> nodeQueue = GreedyMinBorderQueueComputing.ComputeQueue(nodes.ToArray(), nodeToMarginalize.ToIEnumerable());

//            //nodeQueue.AddFirst(nodeToMarginalize);

//            nodeQueue.Each(n => n.IsChosen = false);
//            nodeToMarginalize.IsChosen = false;
//            var sumProbs = new double[options.NumberOfLabels];
//            var rdm = new Random();

//            try
//            {
//                for (int run = 0; run < options.ResamplingRuns; run++)
//                {
//                    //for (int labelLastNode = 0; labelLastNode < options.NumberOfLabels; labelLastNode++)
//                    {
//                        var currentCombination = new Combination();
//                        var currentBorder = new LinkedList<CRFNode>();
//                        var currentNodeToChoose = nodeQueue.Last;

//                        while (true)
//                        {
//                            var currentNode = currentNodeToChoose.Value;
//                            currentNode.IsChosen = true;
//                            currentBorder.AddLast(currentNode);

//                            var scores = new double[options.NumberOfLabels];
//                            for (int label = 0; label < options.NumberOfLabels; label++)
//                            {
//                                scores[label] = currentNode.Scores[label];
//                                foreach (var edge in currentNode.Edges())
//                                {
//                                    var nb = currentNode.Neighbour(edge);
//                                    if (nb.IsChosen)
//                                        scores[label] *= edge.Score(currentNode, label, nb, currentCombination.Assignment[nb]);
//                                }
//                            }

//                            var chosenIndex = scores.ChooseIndexWeighted(d => d, rdm);

//                            currentCombination.AddScore(currentNode, chosenIndex);

//                            currentNodeToChoose = currentNodeToChoose.Previous;
//                            if (currentNodeToChoose == null)
//                                break;
//                        }

//                        var scores2 = new double[options.NumberOfLabels];
//                        for (int label = 0; label < options.NumberOfLabels; label++)
//                        {
//                            scores2[label] = nodeToMarginalize.Scores[label];
//                            foreach (var edge in nodeToMarginalize.Edges())
//                            {
//                                var nb = nodeToMarginalize.Neighbour(edge);
//                                scores2[label] *= edge.Score(nodeToMarginalize, label, nb, currentCombination.Assignment[nb]);
//                            }
//                        }

//                        var sum = scores2.Sum();
//                        //var relativeWeight = scores2[labelLastNode] / sum;

//                        //sumProbs[labelLastNode] += relativeWeight;

//                        for (int i = 0; i < options.NumberOfLabels; i++)
//                        {
//                            sumProbs[i] += scores2[i] / sum;
//                        }

//                        nodeQueue.Each(n => n.IsChosen = false);

//                    }
//                }

//                var newsum = sumProbs.Sum();
//                for (int labelLastNode = 0; labelLastNode < options.NumberOfLabels; labelLastNode++)
//                {
//                    sumProbs[labelLastNode] = sumProbs[labelLastNode] / newsum;
//                }
//            }
//            catch
//            {

//            }

//            return sumProbs;
//        }
//    }

//    public class MarginalizeByResamplingOptions
//    {
//        public MarginalizeByResamplingOptions(int numberOfLabels, int resamplings)
//        {
//            NumberOfLabels = numberOfLabels;
//            ResamplingRuns = resamplings;
//        }
//        public int ResamplingRuns { get; set; }
//        public int NumberOfLabels { get; set; }
//    }


//    /*
//     * 
//     * Notice that this class doesn't give correct samples, but only samples pointwise correct regarding the marginal distribution
//     * 
//     * */
//    public class MarginalizeMethodDistribution : RandomVariable<Dictionary<CRFNode, int>>
//    {
//        public MarginalizeMethodDistribution(CRFGraph graph, Random random, MarginalizeByResamplingOptions options)
//        {
//            Graph = graph;
//            Options = options;
//            this.random = random;
//            Init();
//        }

//        private void Init()
//        {
//            foreach (var node in Graph.Nodes)
//            {
//                var result = MarginalizeByResampling.Do(Graph.Nodes, node, Options);
//                MarginalProbabilities.Add(node, result);
//                Log.Post(node.Id + ": " + Math.Round(result[0], 2));
//            }
//        }

//        private Dictionary<CRFNode, double[]> MarginalProbabilities = new Dictionary<CRFNode, double[]>();

//        private Random random;
//        public CRFGraph Graph { get; set; }
//        public MarginalizeByResamplingOptions Options { get; set; }

//        public IEnumerable<Dictionary<CRFNode, int>> DrawSample(int sampleSize)
//        {
//            for (int i = 0; i < sampleSize; i++)
//            {
//                var assigns = new Dictionary<CRFNode, int>();
//                foreach (var node in Graph.Nodes)
//                {
//                    assigns.Add(node, random.NextDouble() <= MarginalProbabilities[node][0] ? 0 : 1);
//                }
//                yield return assigns;
//            }
//        }

//        public Dictionary<CRFNode, int> Draw()
//        {
//            return DrawSample(1).First();
//        }

//        public double MarginalProbability(Dictionary<CRFNode, int> element)
//        {
//            var score = 0.0;

//            foreach (var assignment in element)
//            {
//                score += Math.Log(MarginalProbabilities[assignment.Key][assignment.Value]);
//            }
//            score = Math.Exp(score);

//            return score;
//        }
//    }
//}
