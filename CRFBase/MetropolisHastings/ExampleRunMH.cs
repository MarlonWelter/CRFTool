using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.MetropolisHastings
{
    class ExampleRunMH
    {
        public static void Run()
        {
            new FileLogger("Log/");
            new ConsoleLoggerSafe();

            SoftwareGraphLearningParameters parameters = new SoftwareGraphLearningParameters();
            parameters.NumberOfGraphs = 60;
            parameters.NumberNodes = 50;
            parameters.NumberLabels = 2;
            parameters.NumberCategories = 4;
            parameters.IntraConnectivityDegree = 0.15;
            parameters.InterConnectivityDegree = 0.01;



            //new WorkCycle().Do(parameters);



            var graph = new ErdösGraphCreator().CreateGraph(new ErdösGraphCreationParameter(parameters));
            new CategoryGraphCreator().CreateCategoryGraph(graph);

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

            //scores erzeugen
            var isingModel = new IsingModel(1.0, 0.7);
            isingModel.CreateCRFScore(graph);




            //sample parameters
            var samplerParameters = new MHSamplerParameters();
            samplerParameters.Graph = graph;
            samplerParameters.NumberChains = 3;






            //sampler starten
            var gibbsSampler = new MHSampler();
            gibbsSampler.Do(samplerParameters);





            Console.Read();


            BaseProgram.Exit.Enter();
        }
    }
}
