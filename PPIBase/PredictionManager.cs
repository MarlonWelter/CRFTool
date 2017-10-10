using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace PPIBase
{
    public class PredictorManager : IRequestListener
    {
        public const string RasaAverageHydrophobicityPredictorFileEnding = ".rahp";
        public const string RasaAverageHydrophobicityCRFPredictorFileEnding = ".rahcp";

        public PredictorManager(string baseLocation)
        {
            BaseDirectory = baseLocation;
            init();
            Register();
        }

        private void init()
        {
            //Load Predictors
            foreach (var file in Directory.GetFiles(BaseDirectory))
            {
                try
                {
                    using (var reader = new StreamReader(file))
                    {
                        if (file.EndsWith(RasaAverageHydrophobicityPredictorFileEnding))
                        {
                            var deserializer = new XmlSerializer(typeof(RasaAverageHydrophobicityPredictor));
                            var predictor = deserializer.Deserialize(reader) as RasaAverageHydrophobicityPredictor;
                            predictors.Add(predictor);
                        }
                        else if (file.EndsWith(RasaAverageHydrophobicityCRFPredictorFileEnding))
                        {
                            var deserializer = new XmlSerializer(typeof(RasaAverageHydrophobicityCRFPredictor));
                            var predictor = deserializer.Deserialize(reader) as RasaAverageHydrophobicityCRFPredictor;
                            predictors.Add(predictor);
                        }
                    }
                }
                catch
                {
                    Log.Post("error loading cref predictors.", LogCategory.Inconsistency);
                }
            }
        }
        public string BaseDirectory { get; set; }
        public IGWContext Context { get; set; }

        private LinkedList<IHas<IPredictionLogic>> predictors = new LinkedList<IHas<IPredictionLogic>>();

        public void Register()
        {
            this.DoRegister<CreatePredictor>(OnCreatePredictor);
            this.DoRegister<Get<IHas<IPredictionLogic>>>(OnGetPredictors);
        }

        private void OnGetPredictors(Get<IHas<IPredictionLogic>> request)
        {
            request.Elements = predictors.ToList();
        }

        public void Unregister()
        {
            this.DoUnregister<CreatePredictor>(OnCreatePredictor);
            this.DoUnregister<Get<IHas<IPredictionLogic>>>(OnGetPredictors);
        }

        private void OnCreatePredictor(CreatePredictor request)
        {

            var req = new GetPDBs(ReadFileLines.Do(request.ViewModel.TrainingPDBsFile));
            req.RequestInDefaultContext();
            var files = req.Files;

            var graphs = CreateProteinGraph.Do(files, request.ViewModel.InterfaceDist, request.ViewModel.GraphNodeDefinition);

            var requestI = new RequestInterface(files, request.ViewModel.InterfaceDist, request.ViewModel.UseVanDerWaalsRadiiIface);
            requestI.RequestInDefaultContext();
            var interfaces = requestI.Interface;

            switch (request.ViewModel.PredictionType)
            {
                case PredictorTypes.RasaAverageHydrophobicityPredictor:
                    var predictor = new RasaAverageHydrophobicityPredictor(request.ViewModel);
                    predictor.Train(request.ViewModel, files, interfaces, graphs);

                    //store Predictor
                    using (var writer = new StreamWriter(BaseDirectory + predictor.Name + RasaAverageHydrophobicityPredictorFileEnding))
                    {
                        var serializer = new XmlSerializer(typeof(RasaAverageHydrophobicityPredictor));
                        serializer.Serialize(writer, predictor);
                    }

                    predictors.Add(predictor);
                    this.DoRequest(new Created<IHas<IPredictionLogic>>(predictor));
                    break;
                case PredictorTypes.RasaAverageHydrophobicityCRFPredictor:
                    var predictor2 = new RasaAverageHydrophobicityCRFPredictor(request.ViewModel);
                    predictor2.Train(request.ViewModel, files, interfaces, graphs);

                    //store Predictor
                    using (var writer = new StreamWriter(BaseDirectory + predictor2.Name + RasaAverageHydrophobicityCRFPredictorFileEnding))
                    {
                        var serializer = new XmlSerializer(typeof(RasaAverageHydrophobicityCRFPredictor));
                        serializer.Serialize(writer, predictor2);
                    }

                    predictors.Add(predictor2);
                    this.DoRequest(new Created<IHas<IPredictionLogic>>(predictor2));
                    break;
                default:
                    break;
            }
        }
    }
}
