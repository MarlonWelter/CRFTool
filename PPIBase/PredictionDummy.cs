using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class PredictionDummy : IRequestListener
    {
        public IGWContext Context { get; set; }
        public PredictionDummy()
        {
            Register();
        }

        public void Register()
        {
            this.DoRegister<DoPrediction>(OnRequest);
        }
        public void Unregister()
        {
            this.DoUnregister<DoPrediction>(OnRequest);
        }

        private void OnRequest(DoPrediction obj)
        {
            //var rdm = new Random();

            //var prediction = new Dictionary<Residue, bool>();

            //foreach (var residue in obj.FileOne.Residues)
            //{
            //    if (rdm.NextDouble() <= 0.3)
            //    {
            //        prediction.Add(residue, true);
            //    }
            //    else
            //        prediction.Add(residue, false);
            //}
            //obj.PredictionOne = prediction;


            //var prediction2 = new Dictionary<Residue, bool>();

            //foreach (var residue in obj.FileTwo.Residues)
            //{
            //    if (rdm.NextDouble() <= 0.3)
            //    {
            //        prediction2.Add(residue, true);
            //    }
            //    else
            //        prediction2.Add(residue, false);
            //}
            //obj.PredictionTwo = prediction2;
        }
    }
}
