
   using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;

namespace CRFBase
{
    public class OLM_IV_RandomChange<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ProteinCRFGraphData
    {
        public OLM_IV_RandomChange(double alpha, int labels, int bufferSizeInference, BasisMerkmal<NodeData, EdgeData, GraphData>[] basisMerkmale, Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, double sensitivityFactor, string name)
        {
            Name = name;
            Alpha = alpha;
            Labels = labels;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale;
            SensitivityFactor = sensitivityFactor;
            WeightObservationUnit_II = new WeightObservationUnit_II(basisMerkmale.Length, 0.1);
        }
        public double SensitivityFactor { get; set; }
        public double Alpha { get; set; }
        public double[] Impulses { get; set; }
        public double[] LastChanges { get; set; }
        public List<double> Changes { get; set; }
        
        //public double[] Do(int weights, IEnumerable<IGWGraph<NodeData, EdgeData, GraphData>> graphs, double epsilon)
        //{
        //    int iteration = 0;
        //    MaxIterations = 1000;
        //    Changes = new List<double>();

        //    var rdm = new Random();
        //    double[] weightOpt = new double[weights];
        //    double[] weightCurrent = new double[weights];
        //    LastChanges = new double[weights];
        //    Impulses = new double[weights];
        //    for (int i = 0; i < weights; i++)
        //    {
        //        weightCurrent[i] = 0.0;
        //        weightOpt[i] = weightCurrent[i];
        //    }
        //    //this.WeightObservationUnit.Init(weightCurrent);
        //    var lossOpt = double.MaxValue;
        //    var lossOptOld = 0.0;
        //    var lossCurrent = 0.0;

        //    OLMTracker = new OLMTracking(weights, new int[] { 1, 3, 5, 8, 12, 20, 50 }, weightCurrent, Name + "_OLMTracking.txt");
        //    TrainingGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>();
        //    ValidationGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>();

        //    //divide graphs in training / validation
        //    foreach (var graph in graphs)
        //    {
        //        //if (rdm.NextDouble() > 0.5)
        //        {
        //            TrainingGraphs.Add(graph);
        //        }
        //        //else
        //        ValidationGraphs.Add(graph);
        //    }


        //    var interfaceValid = 0;
        //    var noninterfaceValid = 0;

        //    foreach (var graph in ValidationGraphs)
        //    {
        //        interfaceValid += graph.Data.ReferenceLabeling.Sum();
        //        noninterfaceValid += graph.Nodes.Count() - interfaceValid;
        //    }

        //    var sitesValid = interfaceValid + Alpha * noninterfaceValid;

        //    while (!(iteration >= MaxIterations))
        //    {
        //        iteration++;

        //        var oldWVector = weightCurrent.ToArray();
        //        var startTime = DateTime.Now;
        //        weightCurrent = Doiteration(TrainingGraphs, weightCurrent, iteration);
        //        var timeused = DateTime.Now - startTime;

        //        tp = 0; tn = CoreResidues; fp = 0; fn = 0;
        //        lossCurrent = 0.0;
        //        foreach (var graph in ValidationGraphs)
        //        {
        //            //set scores according to weights
        //            SetWeightsCRF(weightCurrent, graph);

        //            var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, null, Labels, BufferSizeInference);
        //            request.RequestInDefaultContext();

        //            var labeling = request.Solution.Labeling;
        //            lossCurrent += LossFunctionValidation(graph.Data.ReferenceLabeling, labeling);

        //            TrackResults(graph.Data.ReferenceLabeling, labeling);

        //        }

        //        WriterResults();
        //        lossCurrent /= sitesValid;

        //        if (lossCurrent < lossOpt)
        //        {
        //            lossOptOld = lossOpt;
        //            lossOpt = lossCurrent;
        //            weightOpt = weightCurrent;
        //        }

        //        OLMTracker.Track(weightCurrent, lossCurrent);
        //    }

        //    OLMTracker.WriteWeights();

        //    return weightOpt;
        //}

      
        //double tp = 0, tn = 0, fp = 0, fn = 0;
        double lastMCC = 0.0;
       
        private void WriterResults()
        {
            using (var writer = new StreamWriter(Name + "_Results.txt", true))
            {
                var sensitivity = tp / (tp + fn);
                var specificity = tn / (tn + fp);
                var mcc = (tp * tn + fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));

                writer.WriteLine("" + tp + "_" + tn + "_" + fp + "_" + fn);
                writer.WriteLine(sensitivity);
                writer.WriteLine(specificity);
                writer.WriteLine(mcc);
                writer.WriteLine();
            }
        }

        private double mccMax = 0.0;
        private double[] lastWeights;
        //private Random random = new Random();
        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {
            var weights = weightCurrent.ToArray();
            var weightsSum = new double[weightCurrent.Length];
            int iteration = 0;

            double tp = 0.001, tn = 0.001 + 0, fp = 0.001, fn = 0.001;

            int[] countsRefMinusPred = new int[weightCurrent.Length];

            lastWeights = weights.ToArray();

            var totalChange = random.NextDouble();
            //change weights
            for (int i = 0; i < weights.Length; i++)
			{
                var localChange = random.NextDouble();
                weights[i] += (random.NextDouble() * 0.2 - 0.1) * totalChange * localChange;
			}

            for (int i = 0; i < TrainingGraphs.Count; i++)
            {
                var graph = TrainingGraphs[i];
                //set scores according to weights
                SetWeightsCRF(weights, graph);

                //compute labeling with viterbi algorithm
                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, null, Labels, BufferSizeInference);
                request.RequestInDefaultContext();
                int[] labeling = request.Solution.Labeling;
                //check nonequality

                iteration++;

                for (int k = 0; k < labeling.Length; k++)
                {
                    if (graph.Data.ReferenceLabeling[k] > 0)
                    {
                        if (labeling[k] > 0)
                            tp += 1;
                        else
                            fn += 1;
                    }

                    else
                    {
                        if (labeling[k] > 0)
                            fp += 1;
                        else
                            tn += 1;
                    }
                }

                //var loss = LossFunctionIteration(labeling, graph.Data.ReferenceLabeling);
                int[] countsPred = CountPred(graph, labeling);
                int[] countsRef = CountPred(graph, graph.Data.ReferenceLabeling);
                for (int k = 0; k < countsPred.Length; k++)
                {
                    countsRefMinusPred[k] += countsRef[k] - countsPred[k];
                }

            }

            var mcc = (tp * tn + fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
            if(mcc < mccMax)
            {
                weights = lastWeights;
                Log.Post("Weight unchanged.");
            }
            else
                Log.Post("Weight changed.");

            mccMax = Math.Max(mccMax, mcc);

            //this.WeightObservationUnit.Feedback(mcc - lastMCC);
            lastMCC = mcc;
            return weights;
        }

        protected override bool CheckCancelCriteria()
        {
            return Iteration >= MaxIterations;
        }

        internal override void SetStartingWeights()
        {
            throw new NotImplementedException();
        }
    }
}

