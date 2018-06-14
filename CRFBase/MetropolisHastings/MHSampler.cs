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
    public class MHSampler
    {

        public List<CRFGraph> Chains { get; private set; }

        public List<int[]> FinalSample { get; private set; }
        public List<int[]>[] TestSamples { get; private set; }

        private List<int[]> Combinations;

        private int TestInterval;

        private double ToleranceVariance;

        private double ToleranceAutoCorrelation;

        private double ToleranceMarginalDistribution;

        private static int NumberLabels = 2;

        private static Random random;
        public static Random Random
        {
            get { return (random = random ?? new Random()); }
            set { random = value; }
        }

        //counter variables
        public ulong CountTransitionCategoryProposal { get; set; } 
        public ulong CountTransitionCategoryAccepted { get; set; } 
        public ulong CountTransitionNeighborhoodProposal { get; set; } 
        public ulong CountTransitionNeighborhoodAccepted { get; set; } 
        public ulong CountTransitionNodeProposal { get; set; } 
        public ulong CountTransitionNodeAccepted { get; set; } 
        public int CountTestAutoCorrelationFailed { get; set; }
        public int CountTestVarianceFailed { get; set; }
        public int CountTestMarginalDistributionFailed { get; set; }


        public void Do(MHSamplerParameters parameters)
        {
            Log.Post("MHSampler - Start");
            Build.Do();
            Chains = new List<IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>>();

            //get parameter values
            var numberChains = parameters.NumberChains;
            var graph = parameters.Graph;
            TestInterval = parameters.TestInterval;
            ToleranceVariance = parameters.ToleranceVariance;
            ToleranceMarginalDistribution = parameters.ToleranceMarginalDistribution;
            ToleranceAutoCorrelation = parameters.ToleranceAutoCorrelation;


            //don't allow less than 2 chains
            if (numberChains < 2)
            {
                numberChains = 2;
            }
            //combinations for marginal distribution test
            Combinations = new List<int[]>();
            int[] arr = new int[numberChains];
            for(int i = 0; i < numberChains; i++)
            {
                arr[i] = i;
            }
            CalcCombinations(arr, 2, 0 , new int[2]);


            //create clones
            Chains.Add(graph);
            var catGraphCreator = new CategoryGraphCreator();
            for (int i = 1; i < numberChains; i++)
            {
                var clone = graph.Clone(GraphDataConverter.SGLNodeDataConvert, GraphDataConverter.SGLEdgeDataConvert, GraphDataConverter.SGLGraphDataConvert);
                catGraphCreator.CreateCategoryGraph(clone);
                Chains.Add(clone);
            }

            //init testsamples
            TestSamples = new List<int[]>[Chains.Count];
            for(int i = 0; i < TestSamples.Length; i++)
            {
                TestSamples[i] = new List<int[]>();
            }


            //start chains
            for(int i = 0; i < Chains.Count; i++)
            {
                var chain = Chains[i];
                //start the first chain with viterbi
                if(i == 0)
                {
                    FindStartPointViterbi(chain);
                }
                //rest with random for now
                else
                {
                    FindStartPointRandom(chain);
                }
            }

            //iterate over prerun
            for(int i = 0; i < parameters.PreRunLength; i++)
            {
                foreach(var chain in Chains)
                {
                    ChangePosition(chain, 0);
                }
            }

            //test if chains are mixed
            bool chainsHaveMixed = false;
            int time = 1;
            int test = 1;
            while (!chainsHaveMixed)
            {
                foreach (var chain in Chains)
                {
                    ChangePosition(chain, time);
                }

                //make statistic tests every xth iteration
                if (time % TestInterval == 0)
                {
                    Console.WriteLine("test: " + test++);
                    chainsHaveMixed = StatisticTests(time);

                    //constant window
                    if (!chainsHaveMixed)
                    {
                        //chains aren't mixed -> set everything back to 0
                        time = 0;
                        foreach (var chain in Chains)
                        {
                            chain.Nodes.Each(n => { n.Data.TempCount = 0; });
                            chain.Nodes.Each(n => { n.Data.LastUpdate = 0; });
                        }

                        //clear testsamples
                        foreach(var samples in TestSamples)
                        {
                            samples.Clear();
                        }
                    }

                }
                time++;
            }

            // shuffle all samples into one sample.
            FinalSample = TestSamples.SelectMany(s => s).RandomizeOrder().ToList();

            Log.Post("Chains may have mixed. ");
            Log.Post("Number of Chains: " + numberChains);
            Log.Post("Total Transitions: " + (CountTransitionNodeProposal + CountTransitionNeighborhoodProposal + CountTransitionCategoryProposal));
            Log.Post("Node Transistion Proposals: " + CountTransitionNodeProposal);
            Log.Post("Node Transistions Accepted: " + CountTransitionNodeAccepted);
            Log.Post("Neighborhood Transistion Proposals: " + CountTransitionNeighborhoodProposal);
            Log.Post("Neighborhood Transistions Accepted: " + CountTransitionNeighborhoodAccepted);
            Log.Post("Category Transition Proposals: " + CountTransitionCategoryProposal);
            Log.Post("Category Transitions Accepted: " + CountTransitionCategoryAccepted);

            Log.Post("Tests: " + (test - 1));
            Log.Post("AutoCorrelation Test Failed: " + CountTestAutoCorrelationFailed);
            Log.Post("Variance Test Failed: " + CountTestVarianceFailed);
            Log.Post("Marginal Distribution Test Failed: " + CountTestMarginalDistributionFailed);
            Log.Post(Environment.NewLine);


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
            foreach(var item in graph.Nodes)
            {
                item.Data.TempAssign = Random.Next(2);
            }
        }


        private void FindStartPointAlternating(CRFGraph graph)
        {
            int state = 0;
            foreach(var item in graph.Nodes)
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

            foreach(var item in graph.Nodes)
            {
                item.Data.TempAssign = resultLabeling[item.GraphId];
            }
        }



        
        private void ChangePosition(CRFGraph graph, int time)
        {
            //jedes proposal gleich wahrscheinlich
            var p = (double) graph.Nodes.Count() / (graph.Data.NumberCategories + 2 * graph.Nodes.Count());
            var random = Random.NextDouble();
            if (random <= p)
            {
                CountTransitionNodeProposal++;
                ChangePositionNode(graph, time);
            }
            else if (random > p && random <= p*2)
            {
                CountTransitionNeighborhoodProposal++;
                ChangePositionNeighborhood(graph, time);
            }
            else
            {
                CountTransitionCategoryProposal++;
                ChangePositionCategory(graph, time);
            }


            //out of perRun , collect sample for tests
            if (time > 0)
            {   
                //get list of samples for current graph
                var samples = TestSamples[Chains.IndexOf(graph)];

                int[] sample = new int[graph.Nodes.Count()];
                foreach(var node in graph.Nodes)
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
            foreach(var edge in node.Edges)
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
            foreach(var edge in node.Edges)
            {
                newScore += edge.TempScore();
            }


            if (Random.NextDouble() <= (Math.Exp(newScore)) / (Math.Exp(oldScore)))
            {
                //state changed
                //TODO check for correct counting
                node.Data.TempCount += (1 - node.Data.TempAssign) * (time - node.Data.LastUpdate);
                node.Data.LastUpdate = time;
                CountTransitionNodeAccepted++;
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
            foreach(var node in catNode.Data.Nodes)
            {
                oldScore += node.Data.Score(node.Data.TempAssign);
            }

            foreach(var edge in catNode.Data.InterEdges)
            {
                oldScore += edge.TempScore();
            }


            //flip each node
            foreach(var node in catNode.Data.Nodes)
            {
                node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
            }


            //newScore berechnen
            foreach (var node in catNode.Data.Nodes)
            {
                newScore += node.Data.Score(node.Data.TempAssign);
            }

            foreach(var edge in catNode.Data.InterEdges)
            {
                newScore += edge.TempScore();
            }


            relChange = newScore - oldScore;
            if (Random.NextDouble() <= (Math.Exp(newScore))/(Math.Exp(oldScore)))
            {
                //state changed
                //TODO check for correct counting
                foreach(var node in catNode.Data.Nodes)
                {
                    node.Data.TempCount += (1 - node.Data.TempAssign) * (time - node.Data.LastUpdate);
                    node.Data.LastUpdate = time;
                    
                }
                CountTransitionCategoryAccepted++;
            }
            else
            {
                //stay in state
                foreach(var node in catNode.Data.Nodes)
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
            foreach(var neighbor in neighbors)
            {
                oldScore += neighbor.Data.Score(neighbor.Data.TempAssign);
                foreach(var edge in neighbor.Edges)
                {   
                   //if (!neighbor.Neighbour(edge).Equals(node))
                   oldScore += edge.TempScore();
                }
            }


            //knoten + nachbarschaft flippen
            node.Data.TempAssign = ((node.Data.TempAssign + 1) % 2);
            foreach(var neighbor in neighbors)
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
                CountTransitionNeighborhoodAccepted++;
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




        private bool StatisticTests(int window)
        {
            return TestAutoCorrelation(window, 1000, TestFunction.Average) & TestVariance(window, TestFunction.Sum) & TestMarginalDistribution(window);
            //return TestAutoCorrelation(window, 1000, TestFunction.Average) && TestVariance(window, TestFunction.Sum) && TestMarginalDistribution(window);
        }


        private bool TestVariance(int window, Func<int[], double> testFunction)
        {
            double[] f_k = new double[TestSamples.Length];
            double f_bar, B, W, V, R;

            //average for each chain k, f_k
            for(int i = 0; i < f_k.Length; i++)
            {
                double res = 0.0;
                var samples = TestSamples[i];
                foreach(var sample in samples)
                {
                    res += testFunction(sample);
                }
                res = res / window;
                f_k[i] = res;
            }

            //average between chains f_bar
            f_bar = f_k.Sum() / f_k.Length;


            //between-chains variance B
            B = 0.0;
            for (int i = 0; i < f_k.Length; i++)
            {
                B += Math.Pow((f_k[i] - f_bar), 2);
            }
            B = B * ((double) window / (f_k.Length - 1));


            //within-chain variance W
            W = 0.0;
            for(int i = 0; i < f_k.Length; i++)
            {
                var samples = TestSamples[i];
                foreach(var sample in samples)
                {
                    W += Math.Pow((testFunction(sample) - f_k[i]), 2);
                }
            }
            W = W * (1.0 / f_k.Length) * (1.0 / (window - 1));


            //overestimate of variance V
            V = (((double)(window - 1) / window) * W) + ((1.0 / window) * B);


            //disagreement R
            //if all chains have not converged to stat. distrib., this estimate will be high
            //-> should be close to 1, then chains may have mixed
            R = Math.Sqrt(V / W);

            //Console.WriteLine("disagreement: " + R);

            if (Math.Abs(R - 1) > ToleranceVariance)
            {
                CountTestVarianceFailed++;
                return false;
            }


            return true;
        }

        private bool TestAutoCorrelation(int window, int lag, Func<int[], double> testFunction)
        {
            for(int i = 0; i < TestSamples.Length; i++)
            {

                var samples = TestSamples[i];

                var estimator = 0.0;
                foreach (var sample in samples)
                {
                    estimator += testFunction(sample);
                }
                estimator = estimator * (1.0 / (window));


                var variance = 0.0;
                foreach (var sample in samples)
                {
                    variance += Math.Pow((testFunction(sample) - estimator), 2);
                }
                variance = variance * (1.0 / (window - 1));


                var autocovariance = 0.0;
                for (int m = 0; m < samples.Count - lag; m++)
                {
                    autocovariance += (testFunction(samples[m]) - estimator) * (testFunction(samples[m + lag]) - estimator);
                }
                autocovariance = autocovariance * (1.0 / (window - lag));


                //one way to diagnose a poorly mixing chain is to observe high autocorrelation at distant lags
                var autocorrelation = autocovariance / variance;

                //Console.WriteLine("autocorrelation of chain " + i + " at lag " + lag + " : " + autocorrelation);

               
                if (autocorrelation >= ToleranceAutoCorrelation)
                {
                    CountTestAutoCorrelationFailed++;
                    return false;
                }

            }

            return true;
        }


        private bool TestMarginalDistribution(int window)
        {
            //count 1s at end
            foreach (var chain in Chains)
            {
                foreach (var node in chain.Nodes)
                {
                    node.Data.TempCount += node.Data.TempAssign * (window - node.Data.LastUpdate);
                    node.Data.LastUpdate = window;
                }
            }


            //iterate over combinations ( NumberChains choose 2) combinations are compared
            foreach (var comb in Combinations)
            {

                var chain1 = Chains[comb[0]];
                var chain2 = Chains[comb[1]];

                double[] dist1 = new double[chain1.Nodes.Count()];
                double[] dist2 = new double[chain2.Nodes.Count()];


                foreach (var node in chain1.Nodes)
                {
                    dist1[node.GraphId] = (double)node.Data.TempCount / window;
                }

                foreach (var node in chain2.Nodes)
                {
                    dist2[node.GraphId] = (double)node.Data.TempCount / window;
                }



                //if not inside tolerance return false
                for (int i = 0; i < dist1.Length; i++)
                {
                    if (Math.Abs(dist1[i] - dist2[i]) > ToleranceMarginalDistribution)
                    {
                        CountTestMarginalDistributionFailed++;
                        return false;
                    }

                }
            }

            return true;
        }



        private void CalcCombinations(int[] arr, int len, int start, int[] result)
        {
            if (len == 0)
            {
                //need array with res values but new reference
                int[] realres = new int[result.Length];
                for(int i = 0; i < result.Length; i++)
                {
                    realres[i] = result[i];
                }
                Combinations.Add(realres);
                return;
           }
            for(int i = start; i <= arr.Length - len; i++)
            {
                result[result.Length - len] = arr[i];
                CalcCombinations(arr, len - 1, i + 1, result);
            }
        }



    }





    class TestFunction
    {
        public static double Sum(int[] sample)
        {
            return sample.Sum();
        }

        public static double FirstNodeLabeledOne(int[] sample)
        {
            return sample[0];
        }

        public static double Average(int[] sample)
        {
            return sample.Average();
        }

    }


}
