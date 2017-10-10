using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PPIBase
{
    public class RasaAverageHydrophobicityPredictor : IHas<IPredictionLogic>
    {
        public RasaAverageHydrophobicityPredictor()
        {
            logic = new PredictionLogic(doPrediction, doTraining);
            this.Name("RasaAverageHydrophobicityPredictor");
        }
        public RasaAverageHydrophobicityPredictor(CreatePredictorVM vm)
        {
            Threshold = vm.Threshold;
            logic = new PredictionLogic(doPrediction, doTraining);
            this.Name(vm.Name);
        }

        public string Name
        {
            get { return logic.Name; }
            set { logic.Name = value; }
        }

        public Guid GWId { get; set; }

        public double Threshold { get; set; }

        public double Max { get; set; }

        private void doTraining(CreatePredictorVM vm, IEnumerable<PDBFile> trainingFiles, IDictionary<string, IDictionary<Residue, bool>> referenceInterface, IDictionary<string, IDictionary<string, ProteinGraph>> graphs)
        {
            var Feature = new AverageHydrophobicityFeature();

            var values = new LinkedList<AgO<double, ResidueNode>>();
            foreach (var trainingFile in trainingFiles)
            {
                var req = new RequestRasa(trainingFile);
                req.RequestInDefaultContext();
                var rasa = req.Rasavalues;
                foreach (var entry in graphs[trainingFile.Name])
                {
                    Feature.Compute(entry.Value);
                    //values.AddRange(entry.Value.Nodes.Select(node => new AgO<double, ResidueNode>(Feature.ValueOf(node.Data.Residue) * rasa[node.Data.Residue], node.D
//)));
                }
            }

            Max = values.Max(v => v.Data1);

            //var intervals = values.DivideByScoreAequidistant(val => val.Data1, vm.DivisionIntervals);
            //var pts = new LinkedList<AgO<double, double>>();
            //for (int i = 0; i < intervals.Length; i++)
            //{
            //    var interval = intervals[i];
            //    var pt = new AgO<double, double>();
            //    pt.Data1 = ((double)i + 1) / intervals.Length;
            //    var iface = (interval.Sum(val => referenceInterface[val.Data2.Residue.PDB.Name][val.Data2.Residue] ? 1.0 : 0.0));
            //    pt.Data2 = iface / interval.Count();
            //    pts.Add(pt);
            //}
            //ValueFunction = new StepFunction(pts);


        }

        private Dictionary<string, Dictionary<Residue, bool>> doPrediction(PDBFile file, IDictionary<string, ProteinGraph> graphs)
        {
            var dict = new Dictionary<string, Dictionary<Residue, bool>>();
            var Feature = new AverageHydrophobicityFeature();
            var req = new RequestRasa(file);
            req.RequestInDefaultContext();
            var rasa = req.Rasavalues;
            foreach (var graph in graphs)
            {
                Feature.Compute(graph.Value);

                var predDict = new Dictionary<Residue, bool>();
                dict.Add(graph.Key, predDict);

                foreach (var node in graph.Value.Nodes)
                {
                    predDict.Add(node.Data.Residue, (Feature.ValueOf(node.Data.Residue) * rasa[node.Data.Residue]) / Max >= Threshold);
                }
            }
            return dict;
        }


        [XmlIgnore]
        private IPredictionLogic logic;

        [XmlIgnore]
        IPredictionLogic IHas<IPredictionLogic>.Logic
        {
            get { return logic; }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class RasaAverageHydrophobicityCRFPredictor : IHas<IPredictionLogic>
    {
        public Guid GWId { get; set; }
        public RasaAverageHydrophobicityCRFPredictor()
        {
            logic = new PredictionLogic(doPrediction, doTraining);
            this.Name("RasaAverageHydrophobicityPredictor");
        }
        public RasaAverageHydrophobicityCRFPredictor(CreatePredictorVM vm)
        {
            Threshold = vm.Threshold;
            logic = new PredictionLogic(doPrediction, doTraining);
            this.Name(vm.Name);
        }

        public string Name
        {
            get { return logic.Name; }
            set { logic.Name = value; }
        }


        public double Threshold { get; set; }

        public double Max { get; set; }

        private void doTraining(CreatePredictorVM vm, IEnumerable<PDBFile> trainingFiles, IDictionary<string, IDictionary<Residue, bool>> referenceInterface, IDictionary<string, IDictionary<string, ProteinGraph>> graphs)
        {
            var Feature = new AverageHydrophobicityFeature();

            var values = new LinkedList<AgO<double, ResidueNode>>();
            foreach (var trainingFile in trainingFiles)
            {
                var req = new RequestRasa(trainingFile);
                req.RequestInDefaultContext();
                var rasa = req.Rasavalues;
                foreach (var entry in graphs[trainingFile.Name])
                {
                    Feature.Compute(entry.Value);
                    //values.AddRange(entry.Value.Nodes.Select(node => new AgO<double, ResidueNode>(Feature.ValueOf(node.Data.Residue) * rasa[node.Data.Residue], node)));
                }
            }

            Max = values.Max(v => v.Data1);
        }

        private Dictionary<string, Dictionary<Residue, bool>> doPrediction(PDBFile file, IDictionary<string, ProteinGraph> graphs)
        {
            var dict = new Dictionary<string, Dictionary<Residue, bool>>();
            var Feature = new AverageHydrophobicityFeature();
            var req = new RequestRasa(file);
            req.RequestInDefaultContext();
            var rasa = req.Rasavalues;
            //Threshold = 0.5;
            //foreach (var graph in graphs)
            //{
            //    Feature.Compute(graph.Value);
            //    var valdict = new Dictionary<ResidueNode, double[]>();
            //    var edgeScores = new double[2, 2] { { 1.2, 0.6 }, { 0.6, 1.0 } };
            //    foreach (var node in graph.Value.Nodes)
            //    {
            //        var val = (Feature.ValueOf(node.Data.Residue) * rasa[node.Data.Residue]) / Max;
            //        valdict.Add(node, new double[2] { val, 1 - val });
            //    }
            //    var edgedict = new Dictionary<SimpleEdge<ResidueNode>, double[,]>();
            //    foreach (var edge in graph.Value.Edges)
            //    {
            //        edgedict.Add(edge, edgeScores);
            //    }

            //    var predDict = new Dictionary<Residue, bool>();
            //    dict.Add(graph.Key, predDict);

            //    var crfGraph = graph.Value.CreateGraph(valdict, edgedict);

            //    //var options = new MarginalizeByResamplingOptions(2, 200);
            //    //foreach (var node in crfNodes)
            //    //{
            //    //    var probs = MarginalizeByResampling.Do(crfNodes, node, options);

            //    //    predDict.Add(graph.Value.Nodes.First(nd => nd.Residue.Id.Equals(node.Id)).Residue, probs[0] >= Threshold);
            //    //}
            //    var result = SimpleSampling.Do(crfGraph, 2000);

            //    foreach (var node in crfGraph.Nodes)
            //    {
            //        var score1 = result[new Assign() { Node = node, Label = 0 }];
            //        var score2 = result[new Assign() { Node = node, Label = 1 }];
            //        var prob = score2 / (score1 + score2);
            //        predDict.Add(graph.Value.Nodes.First(nd => nd.Data.Residue.Id.Equals(node.Data.Id)).Residue, prob >= Threshold);
            //    }

            //}
            return dict;
        }


        [XmlIgnore]
        private IPredictionLogic logic;

        [XmlIgnore]
        IPredictionLogic IHas<IPredictionLogic>.Logic
        {
            get { return logic; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
