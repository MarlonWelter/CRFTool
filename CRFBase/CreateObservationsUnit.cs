using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace CRFBase
{
    class CreateObservationsUnit
    {
        public CreateObservationsUnit(double[,] transitionProbabilities)
        {
            TransitionProbabilities = transitionProbabilities;
        }

        public CreateObservationsUnit(double threshold)
        {
            Threshold = threshold;
        }

        public CreateObservationsUnit(BetaDistribution interfaceDistribution, BetaDistribution noninterfaceDistribution)
        {
            InterfaceDistribution = interfaceDistribution;
            NoninterfaceDistribution = noninterfaceDistribution;
        }

        private Random Random = new Random();
        public double[,] TransitionProbabilities { get; set; }
        private double Threshold { get; set; }
        private BetaDistribution InterfaceDistribution { get; set; }
        private BetaDistribution NoninterfaceDistribution { get; set; }
        private int NumberOfSamples { get; set; }

        public void CreateObservationThresholding(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.Observation = node.Data.Characteristics[0] >= Threshold ? 1 : 0;
            }
        }

        public void CreateObservationBetaDistribution(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            foreach(var node in graph.Nodes)
            {
                node.Data.Observation = SampleBernoulli(ComputeProbability(node.Data.Characteristics[0]));
            }
        }

        private double ComputeProbability(double score)
        {
            // get values of beta distributions on position score
            var interfaceValue = InterfaceDistribution.getValue(score);
            var noninterfaceValue = NoninterfaceDistribution.getValue(score);
            // if the score=0 then the values are also 0
            return noninterfaceValue>0 && interfaceValue>0 ? interfaceValue / (interfaceValue + noninterfaceValue) : 0;
        }

        private int SampleBernoulli(double probability)
        {           
            return Bernoulli.Sample(probability);
        }

        public void CreateObservation(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            //var observation = new int[graph.Nodes.Count()];

            foreach (var node in graph.Nodes)
            {
                node.Data.Observation = CreateObservation(node.Data, graph.Data.ReferenceLabeling[node.GraphId]);
            }

            //return observation;
        }

        public int CreateObservation(ICRFNodeData node, int label)
        {
            var index = TransitionProbabilities.ChooseTransition(label, Random);
            return index;
        }
    }
}
