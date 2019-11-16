using CodeBase;
using CRFBase;
using CRFBase.InferenceHeuristik;
using CRFBase.OLM;
using CRFToolAppBase.Software_Graph;
//using EmbedBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CRFToolAppBase
{
    public class CRFToolContext
    {
        private readonly int numberOfLabels = 2;

        public CRFToolData Data { get; set; } = new CRFToolData();

        public void CommandIn(string commandLine)
        {
            var commands = commandLine.Split(' ');
            var command = commands[0];
            var methods = typeof(CRFToolContext).GetMethods();
            var commandRecognized = false;
            foreach (var method in methods)
            {
                if (method.Name.ToLower().Equals(command.ToLower()))
                {
                    var args = new string[commands.Length - 1];
                    for (int i = 0; i < args.Length; i++)
                    {
                        args[i] = commands[i + 1];
                    }

                    if (args.Length > 0)
                    {
                        method.Invoke(this, new object[] { args });
                    }
                    else
                    {
                        method.Invoke(this, null);
                    }

                    commandRecognized = true;
                }
            }
            if (commandRecognized)
            {
                Log.Post("command: " + command);
            }
            else
            {
                Log.Post("command " + command + " not recognized.");
            }
        }

        public GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> LoadCRFGraph()
        {
            var graph = default(GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>);
            var thread = new Thread(
                () =>
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Filter = "Graph Files|*.crf";
                    openFileDialog1.Title = "Select a CRF graph File";

                    if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        graph = JSONX.LoadFromJSON<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>(openFileDialog1.FileName);
                    }
                });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            Data.SelectedGraph = graph;
            return graph;
        }
        public List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> LoadCRFGraphs()
        {
            var list = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Graph Files|*.crfs";
            openFileDialog1.Title = "Select a CRF graph list File";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var graphsList = JSONX.LoadFromJSON<List<string>>(openFileDialog1.FileName);
                foreach (var item in graphsList)
                {
                    var graph = JSONX.LoadFromJSON<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>(item);
                    list.Add(graph);
                }
            }
            return list;
        }


        public void ShowSelectedGraph3D()
        {
            var graph3D = Data.SelectedGraph.Wrap3D();
            new ShowGraph3D(graph3D).Request();
        }

        public CRFResult FindMaxLabelingForSelectedGraph()
        {
            var graph = Data.SelectedGraph;
            var request = new SolveInference(graph, numberOfLabels);
            return request.Result;
        }

        public CRFResult TakeSampleLabelingOfSelectedGraph()
        {
            var graph = Data.SelectedGraph;
            throw new NotImplementedException();
        }

        #region Testing Purposes

        public void SoftwareGraph_1(params string[] args)
        {
            var request = new SoftwareGraphAnalysis_1(args[0], args[1], double.Parse(args[2]), double.Parse(args[3]), int.Parse(args[4]));
            request.Request();
        }

        public void CreateTestGraphs()
        {
            CreateTestData.Execute();
        }

        public GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> CreateTestGraphCRF()
        {
            var graph = GWGraphpackageOne.ThreeCoreElements<CRFNodeData, CRFEdgeData, CRFGraphData>(32);
            graph.Data = new CRFGraphData();
            foreach (var node in graph.Nodes)
            {
                node.Data = new CRFNodeData();
            }
            foreach (var edge in graph.Edges)
            {
                edge.Data = new CRFEdgeData();
            }

            var NumberOfSeedsForPatchCreation = 2;
            var MaximumTotalPatchSize = 12;
            var seedingMethodPatchCreation = new SeedingMethodPatchCreation(NumberOfSeedsForPatchCreation, MaximumTotalPatchSize);

            seedingMethodPatchCreation.CreatePatchAndSetAsReferenceLabel(graph);

            // add some simple example observation
            {
                var random = new Random();
                foreach (var node in graph.Nodes)
                {
                    var scores = new double[2];
                    var rdmValue = random.NextDouble();
                    scores[0] = node.Data.ReferenceLabel == 1 ? (rdmValue / (rdmValue + 1)) : (1 - (rdmValue / (rdmValue + 1)));
                    scores[1] = node.Data.ReferenceLabel == 0 ? (rdmValue / (rdmValue + 1)) : (1 - (rdmValue / (rdmValue + 1)));
                    node.Data.Scores = scores;
                }
                foreach (var edge in graph.Edges)
                {
                    var scores = new double[2, 2];
                    var rdmValue = random.NextDouble();
                    scores = new double[,] { { 0.01, 0.0 }, { 0.0, 0.01 } };
                    edge.Data.Scores = scores;
                }
            }

            graph.SaveAsJSON("testgraph.crf");

            return graph;
        }

        public void ShowAGraph()
        {
            //Data.SelectedGraph = CreateTestGraphCRF();
            //var bInterface = new BInterface();
            //bInterface.SimpleGraphEmbed(Data.SelectedGraph);

        }

        public void TestViterbi()
        {
            var graph = CreateTestGraphCRF();

            var request = new SolveInference(graph, 2, 100);
            request.Request();

            var result = request.Result;
            int tp = 0, tn = 0, fp = 0, fn = 0;
            var score = result.Score;

            foreach (var node in graph.Nodes)
            {
                if (node.Data.ReferenceLabel == 0)
                {
                    if (result.Labeling[node.GraphId] == 0)
                    {
                        tn++;
                    }
                    else
                    {
                        fp++;
                    }
                }
                else
                {
                    if (result.Labeling[node.GraphId] == 0)
                    {
                        fn++;
                    }
                    else
                    {
                        tp++;
                    }
                }
            }
            Console.WriteLine("score: " + score);
            Console.WriteLine("tp: " + tp + " fp: " + fp + " tn: " + tn + " fn: " + fn);

            var viterbi2 = new Viterbi_Plus(100, 2);
            var patchRatioFunction = new Func<double, double>((ratio) => { return -10000 * Math.Abs(0.25 - ratio); });
            var result2 = viterbi2.Run(graph, result, patchRatioFunction);
            tp = 0; tn = 0; fp = 0; fn = 0;
            score = result2.Score;

            foreach (var node in graph.Nodes)
            {
                if (node.Data.ReferenceLabel == 0)
                {
                    if (result2.Labeling[node.GraphId] == 0)
                    {
                        tn++;
                    }
                    else
                    {
                        fp++;
                    }
                }
                else
                {
                    if (result2.Labeling[node.GraphId] == 0)
                    {
                        fn++;
                    }
                    else
                    {
                        tp++;
                    }
                }
            }
            Console.WriteLine("score: " + score);
            Console.WriteLine("tp: " + tp + " fp: " + fp + " tn: " + tn + " fn: " + fn);
        }

        public void TestOlm()
        {
            var graph = CreateTestGraphCRF();

            var request = new OLMRequest(OLMVariant.Default, graph.ToIEnumerable());
            request.Request();
        }

        public void olmres()
        {
            var rdm = new Random();

            var olmresult = new OLMEvaluationResult();
            olmresult.GraphResults = new List<OLMEvaluationGraphResult>();
            for (int i = 0; i < 3; i++)
            {
                var graph = CreateTestGraphCRF();
                var prediction = new int[graph.NodeCounter];
                var observation = new int[graph.NodeCounter];
                var reference = new int[graph.NodeCounter];
                for (int k = 0; k < graph.NodeCounter; k++)
                {
                    prediction[k] = rdm.Next(2);
                    graph.Nodes.First(n => n.GraphId == i).Data.Observation = rdm.Next(2);
                    graph.Nodes.First(n => n.GraphId == i).Data.ReferenceLabel = rdm.Next(2);

                }
                //graph.Data.Pr
                var olmGraphResult = new OLMEvaluationGraphResult(graph, prediction, 0, 0, 0, 0, 0, 0, 0, 0);
                olmresult.GraphResults.Add(olmGraphResult);
            }

            var olmresult2 = new OLMEvaluationResult();
            olmresult2.GraphResults = new List<OLMEvaluationGraphResult>();
            for (int i = 0; i < 3; i++)
            {
                var graph = CreateTestGraphCRF();
                var prediction = new int[graph.NodeCounter];
                var observation = new int[graph.NodeCounter];
                var reference = new int[graph.NodeCounter];
                for (int k = 0; k < graph.NodeCounter; k++)
                {
                    prediction[k] = rdm.Next(2);
                    graph.Nodes.First(n => n.GraphId == i).Data.Observation = rdm.Next(2);
                    graph.Nodes.First(n => n.GraphId == i).Data.ReferenceLabel = rdm.Next(2);

                }
                //graph.Data.Pr
                var olmGraphResult = new OLMEvaluationGraphResult(graph, prediction, 0, 0, 0, 0, 0, 0, 0, 0);
                olmresult2.GraphResults.Add(olmGraphResult);
            }

            var request = new ShowOLMResult(olmresult);
            request.OlmResults.Add(olmresult2);
            request.Request();
        }
        #endregion
    }
}
