using CodeBase;
using CRFBase.OLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.MetropolisHastings
{
    class ExampleRunSoftwareGraphLearning
    {
        public static void Run(SoftwareGraphLearningParameters parameters)
        {

            Build.Do();

            var graphs = new List<IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>>();

            #region Graphen erzeugen

            var erdösGraphCreator = new ErdösGraphCreator();
            var categoryGraphCreator = new CategoryGraphCreator();

            for (int i = 0; i < parameters.NumberOfGraphs; i++)
            {
                var newGraph = erdösGraphCreator.CreateGraph(new ErdösGraphCreationParameter(parameters));
                // Kategoriengraph erzeugen
                categoryGraphCreator.CreateCategoryGraph(newGraph);

                graphs.Add(newGraph);

                //var graph3D = newGraph.Wrap3D();
                //new ShowGraph3D(graph3D).Request();
            }



            #endregion

            #region Beobachtungen erzeugen
            // TODO: Nach welchem Vorgehen Beobachtungen erzeugen?
            // erstmal: für jede Kategorie: zufällig x [0,1): Prob(0) = x, Prob(1) = 1-x

            for (int i = 0; i < parameters.NumberOfGraphs; i++)
            {
                var graph = graphs[i];
                // Beobachtung für graph erzeugen
                foreach (var node in graph.Nodes)
                {
                    var categoryGraph = graph.Data.CategoryGraph;
                    var category = categoryGraph.Nodes.ToList().Find(catNode => catNode.Data.Category == node.Data.Category);

                    double probability = CodeBase.BaseProgram.Random.NextDouble();
                    if (probability <= category.Data.ObservationProbabilty)
                    {
                        node.Data.Observation = 0;
                    }
                    else
                    {
                        node.Data.Observation = 1;

                    }
                }

            }

            //information about graph
            /*
            var graphView = new GraphView();
            foreach (var graph in graphs)
            {
                graphView.GetGraphInfo(graph);

            } */




            #endregion

            #region Training

            // zwei Parameter: Korrelationsparameter || Konformitätsparameter
            // zusätzliche parameter (a (negativ), b) (vorgegeben in SoftwareGraphLearningParameters)
            // Zielfunktion setzt sich aus zwei Zielen zusammen:
            // Ziel 1: Homogenität: erstmal: Für jede Kategorie: Score := a * (Math.Abs(0.95 - homogenityRatio))
            // Ziel 2: Unabhängigkeit: erstmal: Für jede Kategorie: Score += b, falls
            // Ziel 3: Korrelation mit lokalen Beobachtungen
            // Math.Sign([Mittelwert Knotenscore] - 0.5) == Math.Sign([Mittelwert Knotenlabeling] - 0.5)  
            // Ziel 3: Korrelation



            //in results sind die durchschnittl homogenitäten der graphen für die 10 * 10 datenpunkte gespeichert
            var results = new List<LocalResult>();
            for (int k = 1; k <= 10; k++)
            {
                for (int l = 1; l <= 10; l++)
                {
                    var conformity = k * 0.1;
                    var correlation = l * 0.1;
                    var isingModel = new IsingModel(conformity, correlation);
                    //in resultstemp sind homogenitäten für die graphen bei gleicher correlation + conformity
                    var resultsTemp = new List<LocalResult>();
                    int counter = 0;
                    foreach (var graph in graphs)
                    {
                        var localResult = new LocalResult(parameters.NumberCategories, conformity, correlation);

                        #region homogenities normal

                        // anhand der Observation & Korrelation/Konformität CRF-Scores berechnen
                        // Berechne Scores für nodes und edges
                        isingModel.CreateCRFScore(graph);

                        // Viterbiheuristik starten
                        var request = new SolveInference(graph, parameters.NumberLabels);
                        request.RequestInDefaultContext();

                        // sammeln der ergebnisse
                        var resultingLabeling = request.Solution.Labeling;

                        //labeling auf nodes mappen
                        var nodes = graph.Nodes.ToList();
                        foreach (var node in nodes)
                        {
                            node.Data.AssignedLabel = resultingLabeling[node.GraphId];
                        }

                        //homogenität berechnen -> in localResult speichern
                        var categoryGraph = graph.Data.CategoryGraph;
                        //durch jede kategorie gehen
                        foreach (var catNode in categoryGraph.Nodes)
                        {
                            int amountZeroLabeled = 0;
                            //für jeden knoten in aktueller kategorie, anzahl 0 labels zählen
                            foreach (var node in catNode.Data.Nodes)
                            {
                                if (node.Data.AssignedLabel == 0)
                                {
                                    amountZeroLabeled++;
                                }
                            }
                            //homogenität = max(a; 1-a) a = anteil mit 0 gelabelt
                            var homogenityRatio = Math.Max((amountZeroLabeled * 1.0) / catNode.Data.NumberNodes,
                                1 - (amountZeroLabeled * 1.0) / catNode.Data.NumberNodes);
                            //homgenität speichern in homogenity array
                            localResult.Homs[catNode.Data.Category] = homogenityRatio;
                        }
                        #endregion

                        #region distinction isolated

                        //distinction isoliert berechnen 

                        foreach (var edge in graph.Edges)
                        {   //set score of all inter edges to 0
                            if (edge.Data.Type == EdgeType.Inter)
                            {
                                edge.Data.Scores = new double[2, 2] { { 0, 0 }, { 0, 0 } };
                            }
                        }
                        // Viterbiheuristik starten
                        request = new SolveInference(graph, parameters.NumberLabels);
                        request.RequestInDefaultContext();

                        // sammeln der ergebnisse
                        resultingLabeling = request.Solution.Labeling;

                        //labeling auf nodes mappen
                        nodes = graph.Nodes.ToList();
                        foreach (var node in nodes)
                        {
                            node.Data.LabelTemp = resultingLabeling[node.GraphId];
                        }

                        categoryGraph = graph.Data.CategoryGraph;
                        //durch jede kategorie gehen
                        foreach (var catNode in categoryGraph.Nodes)
                        {
                            int amountDifferentLabeled = 0;
                            //für jede kategorie, anzahl unterschiedlich gelabelter knoten zählen
                            foreach (var node in catNode.Data.Nodes)
                            {
                                if (node.Data.AssignedLabel != node.Data.LabelTemp)
                                {
                                    amountDifferentLabeled++;
                                }
                            }
                            //distinctRatio = anteil der ungleich gelabelten nodes
                            var distinctRatio = (amountDifferentLabeled * 1.0) / catNode.Data.NumberNodes;
                            //distinctRatio in distinction array speichern
                            localResult.Distincts[catNode.Data.Category] = distinctRatio;
                        }

                        #endregion

                        resultsTemp.Add(localResult);

                        if (counter == 0)
                        {
                            graph.SaveAsJSON("exampleGraph_" + k + "_" + l + ".txt");
                            //var graph3D = graph.Wrap3D();
                            //new ShowGraph3D(graph3D).Request();
                        }
                        counter++;
                    }//end of foreach graph in graphs

                    //jetzt durchschnittswerte für alle in resultsTemp berechnen 
                    //und diese durchschnitte in results speichern
                    var averageResult = new LocalResult(parameters.NumberCategories, conformity, correlation);

                    //values aufaddieren
                    foreach (var localresult in resultsTemp)
                    {
                        averageResult.AddValues(localresult);
                    }
                    //durchschnitte für kategorien
                    for (int i = 0; i < averageResult.Homs.Length; i++)
                    {
                        averageResult.Homs[i] /= resultsTemp.Count;
                        averageResult.Distincts[i] /= resultsTemp.Count;
                    }
                    //gesamtdurchschnitte
                    double avgHomogenity = 0;
                    double avgDistinction = 0;
                    for (int i = 0; i < averageResult.Homs.Length; i++)
                    {
                        avgHomogenity += averageResult.Homs[i];
                        avgDistinction += averageResult.Distincts[i];
                    }
                    avgHomogenity /= averageResult.Homs.Length;
                    avgDistinction /= averageResult.Distincts.Length;
                    averageResult.AvgHomogenity = avgHomogenity;
                    averageResult.AvgDistinction = avgDistinction;
                    averageResult.ResultValue = avgHomogenity + (1 - avgDistinction);

                    //ausgabe
                    Log.Post("Konformität: " + conformity + " - Korrelation: " + correlation + "   ", LogCategory.Result);
                    Log.Post("Avg Homogenity: " + averageResult.AvgHomogenity, LogCategory.Result);
                    Log.Post("Avg Distinction: " + averageResult.AvgDistinction, LogCategory.Result);
                    Log.Post(Environment.NewLine, LogCategory.Result);

                    results.Add(averageResult);

                }
            }
            /*  Erwartungen: 
             *  
             *  1) gleiche Werte in allen Communities
             *  2) Homogenität ansteigend in Correlation
             *  
             *   
             * */

            // Auswertung der Homogenität und Unabhängigkeit

            var bestResult = results.MaxEntry(r => r.ResultValue);

            Log.Post("Exhaustive Search Result:");
            Log.Post("conformity: " + bestResult.Conformity);
            Log.Post("correlation: " + bestResult.Correlation);


            bestResult.SaveAsJSON(@"..\..\bestResult.txt");
            results.SaveAsJSON(@"..\..\results.txt");

            #endregion


            #region OLM


            var con = bestResult.Conformity;
            var cor = bestResult.Correlation;

            //create referenceLabeling for best parameters
            var isingModell = new IsingModel(con, cor);

            foreach (var graph in graphs)
            {
                isingModell.CreateCRFScore(graph);
                var request = new SolveInference(graph, parameters.NumberLabels);
                request.RequestInDefaultContext();
                graph.Data.ReferenceLabeling = request.Solution.Labeling;
            }

            var req = new OLMRequest(OLMVariant.Default, graphs);
            req.BasisMerkmale.AddRange(new IsingMerkmalNode(), new IsingMerkmalEdge());
            req.LossFunctionValidation = LossFunction;
            req.MaxIterations = 100;

            req.RequestInDefaultContext();

            double[] olmWeights = req.Result.ResultingWeights;


            Log.Post("OLM Result: ");
            for (int i = 0; i < olmWeights.Length; i++)
            {
                Log.Post(olmWeights[i] + "");
            }
            #endregion


        }//end of Do Method


        private static double LossFunction(int[] referenceLabeling, int[] MAPlabeling)
        {
            double res = 0;

            if (referenceLabeling.Length != MAPlabeling.Length)
            {
                throw new Exception("Labelings are not comparable");
            }

            for (int i = 0; i < referenceLabeling.Length; i++)
            {
                if (referenceLabeling[i] != MAPlabeling[i])
                {
                    res += 1;
                }
            }

            return res;
        }
    }
}
