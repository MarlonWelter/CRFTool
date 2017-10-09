using CodeBase;
using CRFBase.OLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class TrainingEvaluationCycle
    {
        private SeedingMethodPatchCreation seedingMethodPatchCreation;
        private Random random = new Random();
        // Graph Visualization: false = orignial, true = created
        private const bool GraphVisalization = false;

        /*
*  Die mit Herrn Waack besprochene Version des Projektzyklus zum Testen der verschiedenen Trainingsvarianten von OLM 
* 
* 
*/
        public void RunCycle(TrainingEvaluationCycleInputParameters inputParameters)
        {
            #region Schritt 0: Vorbereiten der Daten

            // Zwischenspeichern von viel genutzten Variablen zur Übersichtlichkeit:
            var inputGraph = inputParameters.Graph;
            var graphList = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();

            // Graphen erzeugen
            for (int i = 0; i < inputParameters.NumberOfGraphInstances; i++)
            {
                var newGraph = inputGraph.Clone(nd => new CRFNodeData() { X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z }, ed => new CRFEdgeData(), gd => new CRFGraphData());
                graphList.Add(newGraph);
            }

            // Erzeugung der benötigten Objekte:
            seedingMethodPatchCreation = new SeedingMethodPatchCreation(inputParameters.NumberOfSeedsForPatchCreation, inputParameters.MaximumTotalPatchSize);

            #endregion


            #region Schritt 1: Referenzlabelings erzeugen.
            
            int[][] referenceLabelings = new int[inputParameters.NumberOfGraphInstances][];
            for (int i = 0; i < inputParameters.NumberOfGraphInstances; i++)
            {
                seedingMethodPatchCreation.CreatePatchAndSetAsReferenceLabel(graphList[i]);

                if (i == 0 && GraphVisalization == true)
                {
                    var graph3D = graphList[i].Wrap3D(nd => new Node3DWrap<CRFNodeData>(nd.Data) { ReferenceLabel = nd.Data.ReferenceLabel, X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z }, (ed) => new Edge3DWrap<CRFEdgeData>(ed.Data) { Weight = 1.0 });
                    new ShowGraph3D(graph3D).Request();
                }
            }


            #endregion

            #region Schritt 2: Beobachtungen erzeugen (und Scores)

            var createObservationsUnit = new CreateObservationsUnit(inputParameters.TransitionProbabilities);
            var isingModel = new IsingModel(inputParameters.IsingConformityParameter, inputParameters.IsingCorrelationParameter);
            for (int i = 0; i < inputParameters.NumberOfGraphInstances; i++)
            {
                var graph = graphList[i];
                createObservationsUnit.CreateObservation(graph);
                //graph.Data.Observations = observation;

                // zugehörige Scores erzeugen
                isingModel.CreateCRFScore(graph);

                if (i == 0)
                {
                    var graph3D = graph.Wrap3D();
                    new ShowGraph3D(graph3D).Request();
                }
            }
            #endregion

            #region Schritt 3: Aufteilen der Daten in Evaluation und Training
            // Verhaeltnis: 50 50
            int separation = inputParameters.NumberOfGraphInstances / 2;

            var testGraphs = new List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>
                (new IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[separation]);
            var evaluationGraphs = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>
                (new GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>[inputParameters.NumberOfGraphInstances - separation]);

            for (int i = 0; i < separation; i++)
            {
                testGraphs[i] = graphList[i];
            }
            int k = 0;
            for (int j = separation; j < inputParameters.NumberOfGraphInstances; j++)
            {
                evaluationGraphs[k++] = graphList[j];
            }

            #endregion

            #region Schritt 4: Die verschiedenen Varianten von OLM trainieren und evaluieren

            // object for evaluation
            var evaluationResults = new Dictionary<OLMVariant, OLMEvaluationResult>();

            foreach (var trainingVariant in inputParameters.TrainingVariantsToTest)
            {
                evaluationResults.Add(trainingVariant, new OLMEvaluationResult());

                #region Schritt 4.1: Training der OLM-Variante
                {
                    var request = new OLMRequest(trainingVariant, testGraphs);
                    request.BasisMerkmale = new BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[]
                        { new IsingMerkmalNode(), new IsingMerkmalEdge() };
                    //TODO: loss function auslagern
                    request.LossFunctionValidation = (a, b) =>
                    {
                        var loss = 0.0;
                        for (int i = 0; i < a.Length; i++)
                        {
                            loss += a[i] != b[i] ? 1 : 0;
                        }
                        return loss / a.Length;
                    };

                    request.Request();

                    var olmResult = request.Result;


                    // update Ising parameters in IsingModel
                    isingModel.ConformityParameter = olmResult.ResultingWeights[0];
                    isingModel.CorrelationParameter = olmResult.ResultingWeights[1];

                    // zugehörige Scores erzeugen für jeden Graphen (auch Evaluation)
                    foreach (var graph in graphList)
                    {
                        isingModel.CreateCRFScore(graph);
                    }
                }
                #endregion

                #region Schritt 4.2: Evaluation der OLM-Variante

                var keys = new ComputeKeys();
                var results = new OLMEvaluationResult();
                results.ConformityParameter = isingModel.ConformityParameter;
                results.CorrelationParameter = isingModel.CorrelationParameter;

                // 1) Viterbi-Heuristik starten (request: SolveInference) + zusätzliche Parameter hinzufügen
                for (int graph = 0; graph < evaluationGraphs.Count; graph++)
                {
                    var request2 = new SolveInference(evaluationGraphs[graph], null, inputParameters.NumberOfLabels,
                    inputParameters.BufferSizeViterbi);

                    request2.RequestInDefaultContext();

                    // 2) Ergebnis des request auswerten (request.Solution liefert ein Labeling)
                    int[] predictionLabeling = request2.Solution.Labeling;

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
                Log.Post("Sensitivity: "+evaluationResults[trainingVariant].AverageSensitivity + 
                    "\t Specificy: " + evaluationResults[trainingVariant].AverageSpecificity + 
                    "\t MCC: " + evaluationResults[trainingVariant].AverageMCC +
                    //"\t Accuracy: " + evaluationResults[trainingVariant].AverageAccuracy +
                    "\t TotalTP: " + evaluationResults[trainingVariant].TotalTP + "\n");

                #endregion

            }

            #endregion

            #region Schritt 5: Ergebnisse präsentieren und speichern
            // output of the keys
            //outputKeys(evaluation, inputParameters, evaluationGraphs);

            // output of the labels
            //outputLabelingsScores(graphList, inputParameters);


            // TODO: Marlon
            // graphische Ausgabe

            var olmPresentationRequest = new ShowOLMResult(evaluationResults.Values.ToList());
            //foreach (var variant in evaluationResults.Keys)
            //{
                
            //    //foreach (var graphresult in evaluationResults[variant].GraphResults)
            //    //{
            //    //    //var graph = graphresult.Graph;
            //    //}
            //}
            olmPresentationRequest.Request();
            #endregion
        }

        //private int[] CreateReferenceLabeling(TrainingEvaluationCycleInputParameters input)
        //{
        //    // Die ReferenzLabelings werden über die Seeding-Methode erzeugt.
        //    seedingMethodPatchCreation.NumberOfSeeds = input.NumberOfSeedsForPatchCreation;
        //    seedingMethodPatchCreation.TotalPatchesSize = input.MaximumTotalPatchSize;
        //    var referenceLabelingNodes = seedingMethodPatchCreation.CreatePatches(input.Graph);
        //    var referenceLabeling = new int[input.Graph.Nodes.Count()];

        //    // Laura: Jeder Knoten aus referenceLabelingNodes muss nun den Eintrag "1" im referenceLabeling-Array bekommen.
        //    foreach (var node in referenceLabelingNodes)
        //    {
        //        referenceLabeling[node.GraphId] = 1;
        //    }

        //    return referenceLabeling;
        //}

        private static void outputKeys(double[][] keysForAllClones, TrainingEvaluationCycleInputParameters input,
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

        private static void outputLabelingsScores(List<IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> clone,
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
