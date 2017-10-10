using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public interface IPredictionLogic : ILogic
    {
        void Train(CreatePredictorVM vm, IEnumerable<PDBFile> trainingFiles, IDictionary<string, IDictionary<Residue, bool>> referenceInterface, IDictionary<string, IDictionary<string, ProteinGraph>> graphs);
        string Name { get; set; }
        IEnumerable<string> TrainingPDBs { get; }
        Dictionary<string, Dictionary<Residue, bool>> Predict(PDBFile file, IDictionary<string, ProteinGraph> graphs);
    }

    public class PredictionLogic : IPredictionLogic
    {
        public Guid GWId { get; set; }
        public void Train(CreatePredictorVM vm, IEnumerable<PDBFile> trainingFiles, IDictionary<string, IDictionary<Residue, bool>> referenceInterface, IDictionary<string, IDictionary<string, ProteinGraph>> graphs)
        {
            TrainingAction(vm, trainingFiles, referenceInterface, graphs);
            TrainingPDBs.AddRange(trainingFiles.Select(file => file.Name));
        }
        public string Name { get; set; }
        public LinkedList<string> TrainingPDBs { get; private set; }
        public PredictionLogic(Func<PDBFile, IDictionary<string, ProteinGraph>, Dictionary<string, Dictionary<Residue, bool>>> predictionLogic, Action<CreatePredictorVM, IEnumerable<PDBFile>, IDictionary<string, IDictionary<Residue, bool>>, IDictionary<string, IDictionary<string, ProteinGraph>>> trainingLogic)
        {
            TrainingPDBs = new LinkedList<string>();
            TrainingAction = trainingLogic;
            PredictionFunc = predictionLogic;
        }
        public Action<CreatePredictorVM, IEnumerable<PDBFile>, IDictionary<string, IDictionary<Residue, bool>>, IDictionary<string, IDictionary<string, ProteinGraph>>> TrainingAction { get; set; }
        public Func<PDBFile, IDictionary<string, ProteinGraph>, Dictionary<string, Dictionary<Residue, bool>>> PredictionFunc { get; set; }
        public Dictionary<string, Dictionary<Residue, bool>> Predict(PDBFile file, IDictionary<string, ProteinGraph> graphs)
        {
            if (TrainingPDBs.Any(pdb => pdb.Equals(file.Name)))
            {
                Log.Post("This PDB was used to train this predictor.");
            }
            return PredictionFunc(file, graphs);
        }


        IEnumerable<string> IPredictionLogic.TrainingPDBs
        {
            get
            {
                return TrainingPDBs;
            }
        }
    }

    public static class PredictionX
    {
        public static Dictionary<string, Dictionary<Residue, bool>> Predict(this IHas<IPredictionLogic> logicHolder, PDBFile file, IDictionary<string, ProteinGraph> graphs)
        {
            return logicHolder.Logic.Predict(file, graphs);
        }
        public static string Name(this IHas<IPredictionLogic> logicHolder)
        {
            return logicHolder.Logic.Name;
        }
        public static void Name(this IHas<IPredictionLogic> logicHolder, string name)
        {
            logicHolder.Logic.Name = name;
        }
        public static IEnumerable<string> TrainingPDBs(this IHas<IPredictionLogic> logicHolder)
        {
            return logicHolder.Logic.TrainingPDBs;
        }

        public static void Train(this IHas<IPredictionLogic> logicHolder, CreatePredictorVM vm, IEnumerable<PDBFile> trainingFiles, IDictionary<string, IDictionary<Residue, bool>> referenceInterface, IDictionary<string, IDictionary<string, ProteinGraph>> graphs)
        {
            logicHolder.Logic.Train(vm, trainingFiles, referenceInterface, graphs);
        }
    }
}
