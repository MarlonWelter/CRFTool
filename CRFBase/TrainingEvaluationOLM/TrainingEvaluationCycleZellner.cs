using CodeBase;
using CRFBase.OLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class TrainingEvaluationCycleZellner
    {
        private Random random = new Random();
        // Graph Visualization: false = orignial, true = created
        private const bool GraphVisualization = false;
        private const bool UseIsingModel = true;
        private CreateObservationsUnit createObservationsUnit;
        private IsingModel isingModel;
        private PottsModelComplex pottsModel;

        /*
        *  Die mit Herrn Waack besprochene Version des Projektzyklus zum Testen der verschiedenen Trainingsvarianten von OLM 
        * 
        * 
        */
        public void RunCycle(TrainingEvaluationCycleInputParameters inputParameters)
        {
            #region Schritt 1: Vorbereiten der Daten
            
            var graphList = inputParameters.Graphs;
            int numberOfLabels = inputParameters.NumberOfLabels;
            int numberOfIntervals = inputParameters.NumberOfIntervals;
            int cloneFactor = inputParameters.CloneFactor;

            #endregion

            #region Schritt 2: Aufteilen der Daten in Evaluation und Training
            // Verhaeltnis: 80 20
            int separation = inputParameters.NumberOfGraphInstances - inputParameters.NumberOfGraphInstances / 5;
            // Verhältnis Leave-one-out
            //int separation = inputParameters.NumberOfGraphInstances - 1;

            var trainingGraphs = new List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>
                (new IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[separation]);
            var evaluationGraphs = new List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>
                (new IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[inputParameters.NumberOfGraphInstances - separation]);
            var randomizedGraphList = graphList.RandomizeOrder().ToList();

            for (int i = 0; i < separation; i++)
            {
                trainingGraphs[i] = randomizedGraphList[i];
                //trainingGraphs[i] = graphList[i];
            }
            int k = 0;
            for (int j = separation; j < inputParameters.NumberOfGraphInstances; j++, k++)
            {
                evaluationGraphs[k] = randomizedGraphList[j];
                //evaluationGraphs[i] = graphList[i];
            }

            //Log.Post("Evaluation Graph ID: " + evaluationGraphs[0].Data.Id);
            #endregion

            #region Schritt 3: Beobachtungen erzeugen (und Scores)

            // get Interface-Zellner scores and Non-Interface-Zellner scores of training data
            var interfaceScores = new List<double>();
            var noninterfaceScores = new List<double>();
            foreach(var graph in trainingGraphs)
            {
                var nodes = graph.Nodes;
                foreach(var node in nodes)
                {
                    if (node.Data.ReferenceLabel == 0 && node.Data.Characteristics[0] <= 0.5)
                        noninterfaceScores.Add(node.Data.Characteristics[0]);
                    else
                        interfaceScores.Add(node.Data.Characteristics[0]);
                }
            }

            //createObservationsUnit = new CreateObservationsUnit(inputParameters.Threshold);
            //createObservationsUnit = new CreateObservationsUnit(inputParameters.TransitionProbabilities);
            createObservationsUnit = new CreateObservationsUnit(new BetaDistribution(interfaceScores), new BetaDistribution(noninterfaceScores));

            // debug output
            if (UseIsingModel)
                Log.Post("Ising-Model");
            else
                Log.Post("Potts-Model with " + inputParameters.NumberOfIntervals + " Intervals");

            isingModel = new IsingModel(inputParameters.IsingConformityParameter, inputParameters.IsingCorrelationParameter);
            //var pottsModel = new PottsModel(inputParameters.PottsConformityParameters, inputParameters.IsingCorrelationParameter,
            //    inputParameters.AmplifierControlParameter, inputParameters.NumberOfLabels);
            pottsModel = new PottsModelComplex(inputParameters.PottsConformityParameters, inputParameters.PottsCorrelationParameters,
                inputParameters.AmplifierControlParameter, inputParameters.NumberOfLabels);

            // duplicate graph instances for several different observations with same GraphID
            //var trainingList = duplicateGraphs(trainingGraphs, cloneFactor);
            //var evaluationList = duplicateGraphs(evaluationGraphs, cloneFactor);

            //createObservationAndScores(trainingList);
            //createObservationAndScores(evaluationList);
            createObservationAndScores(graphList);
            #endregion

            #region Schritt 4: Die verschiedenen Varianten von OLM trainieren und evaluieren

            // object for evaluation
            var evaluationResults = new Dictionary<OLMVariant, OLMEvaluationResult>();
            var elapsedMs = 0.0;

            foreach (var trainingVariant in inputParameters.TrainingVariantsToTest)
            {
                evaluationResults.Add(trainingVariant, new OLMEvaluationResult());

                #region Schritt 4.1: Training der OLM-Variante
                {
                    //var request = new OLMRequest(trainingVariant, trainingList);
                    var request = new OLMRequest(trainingVariant, trainingGraphs);
                    if (UseIsingModel)
                        request.BasisMerkmale.AddRange(new IsingMerkmalNode(), new IsingMerkmalEdge());
                    else
                    {
                        request.BasisMerkmale.AddRange(pottsModel.AddNodeFeatures(graphList, numberOfIntervals));
                        //request.BasisMerkmale.Add(new IsingMerkmalEdge());
                        request.BasisMerkmale.AddRange(pottsModel.AddEdgeFeatures(graphList, numberOfIntervals));
                    }

                    // loss function
                    request.LossFunctionIteration = OLM.OLM.LossRatio;
                    request.LossFunctionValidation = OLM.OLM.LossRatio;

                    // execute training methods by calling OLMManager -> OLMBase
                    //var watch = System.Diagnostics.Stopwatch.StartNew();
                    request.Request();
                    var olmResult = request.Result;
                    //watch.Stop();
                    //elapsedMs = watch.ElapsedMilliseconds;

                    // update parameters
                    if (UseIsingModel)
                    {
                        isingModel.ConformityParameter = olmResult.ResultingWeights[0];
                        isingModel.CorrelationParameter = olmResult.ResultingWeights[1];
                    }
                    else
                    {
                        int i = 0;
                        for (i = 0; i < pottsModel.ConformityParameter.Length; i++)
                            pottsModel.ConformityParameter[i] = olmResult.ResultingWeights[i];
                        //pottsModel.CorrelationParameter = olmResult.ResultingWeights[numberOfIntervals * 2];
                        for (int j = 0; j < pottsModel.CorrelationParameter.Length; j++)
                            pottsModel.CorrelationParameter[j] = olmResult.ResultingWeights[i++];
                    }

                    // zugehörige Scores erzeugen für jeden Graphen (auch Evaluation)
                    //setScores(trainingList, request);
                    //setScores(evaluationList, request);
                    setScores(graphList, request);
                }
                #endregion

                #region Schritt 4.2: Evaluation der OLM-Variante

                var keys = new ComputeKeys();
                var results = new OLMEvaluationResult();
                if (UseIsingModel)
                {
                    results = new OLMEvaluationResult
                    {
                        ConformityParameter = isingModel.ConformityParameter,
                        CorrelationParameter = isingModel.CorrelationParameter
                    };
                } else
                {
                    results = new OLMEvaluationResult
                    {
                        ConformityParameters = pottsModel.ConformityParameter,
                        //CorrelationParameter = pottsModel.CorrelationParameter
                        CorrelationParameters = pottsModel.CorrelationParameter
                    };
                }

                if (UseIsingModel)
                    Log.Post("Conformity: " + results.ConformityParameter + "\t Correlation: " + results.CorrelationParameter);
                else
                {
                    for (int i = 0; i < results.ConformityParameters.Length; i++)
                        Log.Post("Conformity " + i + ": " + results.ConformityParameters[i] + "\t");
                    //Log.Post("Correlation: " + results.CorrelationParameter);
                    for (int i = 0; i < results.CorrelationParameters.Length; i++)
                        Log.Post("Correlation " + i + ": " + results.CorrelationParameters[i] + "\t");
                }

                // 1) Viterbi-Heuristik starten (request: SolveInference) + zusätzliche Parameter hinzufügen
                for (int graph = 0; graph < evaluationGraphs.Count; graph++)
                {
                    var request2 = new SolveInference(evaluationGraphs[graph], inputParameters.NumberOfLabels, inputParameters.BufferSizeViterbi);
                    request2.RequestInDefaultContext();
                    // 2) Ergebnis des request auswerten (request.Solution liefert ein Labeling)
                    int[] predictionLabeling = request2.Solution.Labeling;


                    //var graphID = evaluationGraphs[graph].Data.Id;
                    //var clones = new List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
                    //foreach(var clone in evaluationList)
                    //{
                    //    if (clone.Data.Id == graphID)
                    //        clones.Add(clone);
                    //}
                    //int[] predictionLabeling = new int[evaluationGraphs[graph].Nodes.Count()];
                    //foreach (var clone in clones)
                    //{
                    //    var request2 = new SolveInference(clone, inputParameters.NumberOfLabels, inputParameters.BufferSizeViterbi);
                    //    request2.RequestInDefaultContext();
                    //    // 2) Ergebnis des request auswerten (request.Solution liefert ein Labeling)
                    //    var labeling = request2.Solution.Labeling;
                    //    for (int i = 0; i < clone.Nodes.Count(); i++)
                    //        predictionLabeling[i] += labeling[i];
                    //}

                    //for (int i = 0; i < evaluationGraphs[graph].Nodes.Count(); i++)
                    //    predictionLabeling[i] = predictionLabeling[i] > cloneFactor / 2 ? 1 : 0;

                    // 3) Ergebnisse aller Evaluationsgraphen auswerten (TP, TN, FP, FN, MCC) und zwischenspeichern 
                    // neues Objekt, damit in Schritt 5 darauf zugegriffen werden kann.
                    var result = keys.computeEvalutionGraphResult(evaluationGraphs[graph], predictionLabeling);
                    // einfügen in Dictionary -> Liste
                    evaluationResults[trainingVariant].GraphResults.Add(result);
                }

                // Berechnen der Average-Werte
                foreach (OLMVariant variant in evaluationResults.Keys)
                    results.ComputeValues(evaluationResults[trainingVariant]);

                // debug output
                Log.Post("Average Values");
                Log.Post("Sensitivity: " + evaluationResults[trainingVariant].AverageSensitivity +
                    "\t Specificity: " + evaluationResults[trainingVariant].AverageSpecificity +
                    "\t MCC: " + evaluationResults[trainingVariant].AverageMCC +
                    "\t Accuracy: " + evaluationResults[trainingVariant].AverageAccuracy +
                    "\t F-Measure: " + evaluationResults[trainingVariant].AverageFmeasure +
                    "\t TotalTP: " + evaluationResults[trainingVariant].TotalTP +
                    "\t TotalFP: " + evaluationResults[trainingVariant].TotalFP +
                    "\t TotalTN: " + evaluationResults[trainingVariant].TotalTN +
                    "\t TotalFN: " + evaluationResults[trainingVariant].TotalFN);
                System.IO.File.AppendAllText("results.txt", evaluationResults[trainingVariant].AverageMCC + "&" + evaluationResults[trainingVariant].AverageFmeasure + 
                    "&" + evaluationResults[trainingVariant].AverageAccuracy + "&" + elapsedMs + "\r\n");
                #endregion

            }

            #endregion
        }

        private void setScores(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> list, OLMRequest request)
        {
            foreach (var graph in list)
            {
                if (UseIsingModel)
                    isingModel.CreateCRFScore(graph);
                else
                    pottsModel.CreateCRFScore(graph, request.BasisMerkmale);
            }
        }

        private void createObservationAndScores(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> list)
        {
            foreach (var graph in list)
            {
                //createObservationsUnit.CreateObservation(graph);
                //createObservationsUnit.CreateObservationThresholding(graph);
                createObservationsUnit.CreateObservationBetaDistribution(graph);

                // zugehörige Scores erzeugen
                if (UseIsingModel)
                    isingModel.CreateCRFScore(graph);

                else
                    pottsModel.InitCRFScore(graph);

                if (GraphVisualization)
                {
                    var graph3D = graph.Wrap3D();
                    new ShowGraph3D(graph3D).Request();
                }
            }
        }

        private List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> duplicateGraphs(List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> graphs, int cloneFactor)
        {
            var list = new List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
            foreach (var graph in graphs)
            {
                for (int i = 0; i < cloneFactor; i++)
                {
                    var newGraph = graph.Clone(nd => new CRFNodeData()
                    {
                        X = nd.Data.X,
                        Y = nd.Data.Y,
                        Z = nd.Data.Z,
                        ReferenceLabel = nd.Data.ReferenceLabel,
                        Characteristics = nd.Data.Characteristics
                    }, ed => new CRFEdgeData(), gd => new CRFGraphData() { ReferenceLabeling = gd.Data.ReferenceLabeling, Id = gd.Data.Id });
                    list.Add(newGraph);
                }
            }
            return list;
        }

        private void PresentResults()
        {
            #region Schritt 5: Ergebnisse präsentieren und speichern
            // output of the keys
            //outputKeys(evaluation, inputParameters, evaluationGraphs);

            // output of the labels
            //outputLabelingsScores(graphList, inputParameters);


            // TODO: Marlon
            // graphische Ausgabe

            //var olmPresentationRequest = new ShowOLMResult(evaluationResults.Values.ToList());
            //foreach (var variant in evaluationResults.Keys)
            //{

            //    foreach (var graphresult in evaluationResults[variant].GraphResults)
            //    {
            //        var graph = graphresult.Graph;
            //    }
            //}
            //olmPresentationRequest.Request();
            #endregion
        }

        private static void OutputKeys(double[][] keysForAllClones, TrainingEvaluationCycleInputParameters input,
            List<IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> evaluationGraphs)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter
               ("ergebnisse.txt");

            string[] s = new string[evaluationGraphs.Count];
            string head = "tp\ttn\tfp\tfn\tsen\tspe\tmcc";
            Console.WriteLine(head);
            file.WriteLine(head);

            for (int i = 0; i < evaluationGraphs.Count; i++)
            {
                for (int j = 0; j < keysForAllClones[i].Length; j++)
                {
                    Console.Write(Math.Round(keysForAllClones[i][j], 2) + "\t");
                    s[i] += Math.Round(keysForAllClones[i][j], 2);
                    s[i] += "\t";
                }
                s[i] += "\n";
                file.WriteLine(s[i]);
                Console.WriteLine();
            }
            file.Close();
        }

        private static void OutputLabelingsScores(List<IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> clone,
            TrainingEvaluationCycleInputParameters input)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter
               ("labelingAndScores.txt");

            string[] s = new string[input.NumberOfGraphInstances];
            string head = "ReferenceLabeling\tTrainingLabeling";
            Console.WriteLine(head);
            file.WriteLine(head);

            // for each graph print out the RL and AL
            for (int i = 0; i < input.NumberOfGraphInstances; i++)
            {
                // Reference Labeling
                for (int j = 0; j < clone[i].Data.ReferenceLabeling.Count(); j++)
                {
                    Console.Write(clone[i].Data.ReferenceLabeling[j] + " ");
                }
                Console.Write("\t");
                // AssignedLabeling
                //for (int j = 0; j < clone[i].Data.ReferenceLabeling.Count(); j++) {
                //    Console.Write(clone[i].Data.AssginedLabeling[j] + " ");
                //}
                //Console.Write("\t");
                Console.WriteLine();
            }
            file.Close();
        }
    }
}
