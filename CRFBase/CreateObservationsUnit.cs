using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private Random Random = new Random();
        public double[,] TransitionProbabilities { get; set; }
        private double Threshold { get; set; }

        public void CreateObservationThresholding(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.Observation = node.Data.Characteristics[0] >= Threshold ? 1 : 0;
            }
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
