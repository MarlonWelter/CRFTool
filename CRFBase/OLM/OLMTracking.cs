using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class OLMTracking
    {
        public OLMTracking(int weights, int[] trackDistances, double[] initWeights, string storeLocation)
        {
            StoreLocation = storeLocation;
            Weights = weights;
            TrackDistances = trackDistances;
            Distances = new List<double>[trackDistances.Length];
            Losses = new LinkedList<double>();
            LossesMidTerm = new LinkedList<double>();
            LosseslongTerm = new LinkedList<double>();
            AverageChanges = new LinkedList<double>[trackDistances.Length];
            AverageChangesLong = new LinkedList<double>[trackDistances.Length];
            for (int i = 0; i < trackDistances.Length; i++)
            {
                Distances[i] = new List<double>();
                AverageChanges[i] = new LinkedList<double>();
                AverageChangesLong[i] = new LinkedList<double>();
            }
            weightHistories = new List<List<double>>();
            for (int i = 0; i < weights; i++)
            {
                weightHistories.Add(new List<double>());
                weightHistories[i].Add(initWeights[i]);
            }
        }
        public string StoreLocation { get; set; }
        public int Iteration { get; set; }

        public int Weights { get; set; }

        public LinkedList<double> Losses { get; set; }
        public LinkedList<double> LossesMidTerm { get; set; }
        public LinkedList<double> LosseslongTerm { get; set; }
        public int[] TrackDistances { get; set; }
        public List<double>[] Distances { get; set; }

        public LinkedList<double>[] AverageChanges { get; set; }
        public LinkedList<double>[] AverageChangesLong { get; set; }

        private List<List<double>> weightHistories;
        public List<List<double>> WeightHistories
        {
            get { return weightHistories; }
            set { weightHistories = value; }
        }
        public void Track(double[] weightVector, double loss)
        {
            Losses.AddLast(loss);
            if (Iteration == 0)
            {
                LossesMidTerm.AddLast(loss);
                LosseslongTerm.AddLast(loss);
            }
            else
            {
                LossesMidTerm.AddLast(LossesMidTerm.Last.Value * (1.0 - (1.0 / Math.Min(10, Iteration + 1))) + loss * ((1.0 / Math.Min(10, Iteration + 1))));
                LosseslongTerm.AddLast(LosseslongTerm.Last.Value * (1.0 - (1.0 / Math.Min(100, Iteration + 1))) + loss * ((1.0 / Math.Min(100, Iteration + 1))));
            }
            //add weights to memory
            for (int i = 0; i < Weights; i++)
            {
                weightHistories[i].Add(weightVector[i]);
            }

            //compute changes
            for (int i = 0; i < TrackDistances.Length; i++)
            {
                var distance = 0.0;
                var iterationDistance = TrackDistances[i];
                if (weightHistories[0].Count > iterationDistance)
                {
                    for (int bm = 0; bm < Weights; bm++)
                    {
                        distance += (weightHistories[bm][iterationDistance] - weightVector[bm]) * (weightHistories[bm][iterationDistance] - weightVector[bm]);
                    }

                    Distances[i].Add(distance);
                    var iteration = Iteration + 1 - iterationDistance;
                    if (iteration == 0)
                    {
                        AverageChanges[i].AddLast(distance);
                        AverageChangesLong[i].AddLast(distance);
                    }
                    else
                    {
                        AverageChanges[i].AddLast(AverageChanges[i].Last.Value * (1.0 - (1.0 / Math.Min(10, iteration + 1))) + distance * ((1.0 / Math.Min(10, iteration + 1))));
                        AverageChangesLong[i].AddLast(AverageChangesLong[i].Last.Value * (1.0 - (1.0 / Math.Min(100, iteration + 1))) + distance * ((1.0 / Math.Min(100, iteration + 1))));
                    }
                }
            }

            Iteration++;

            //using (var writer = new StreamWriter(StoreLocation))
            //{
            //    //writer.WriteLine("Iteration: " + Iteration);
            //    //writer.WriteLine("loss: " + Math.Round(Losses.Last.Value, 3));
            //    //writer.WriteLine("loss2: " + Math.Round(LossesMidTerm.Last.Value, 3));
            //    //writer.WriteLine("loss3: " + Math.Round(LosseslongTerm.Last.Value, 3));
            //    //for (int i = 0; i < TrackDistances.Length; i++)
            //    //{
            //    //    if (Iteration >= TrackDistances[i])
            //    //    {
            //    //        writer.WriteLine("Distance-" + i + " :" + Math.Round(Distances[i][Distances[i].Count - 1], 10));
            //    //        writer.WriteLine("DistanceMidTerm-" + i + " :" + Math.Round(AverageChanges[i].Last.Value, 10));
            //    //        writer.WriteLine("DistanceLongTerm-" + i + " :" + Math.Round(AverageChangesLong[i].Last.Value, 10));
            //    //    }
            //    //}
            //    //if(Iteration % 10 == 0)
            //    {
            //        for (int i = 0; i < Weights; i++)
            //        {
            //            writer.WriteLine("Weights: " + WeightsInLine(weightHistories[i]));
            //        }
            //    }
            //    //writer.WriteLine();
            //}
        }

        public void WriteWeights()
        {
            using (var writer = new StreamWriter(StoreLocation, true))
            {
                for (int i = 0; i < Weights; i++)
                {
                    writer.WriteLine("Weights: " + WeightsInLine(weightHistories[i]));
                }
            }
        }
        private string WeightsInLine(IList<double> w)
        {
            var line = "[";
            for (int i = 0; i < w.Count; i++)
            {
                line += " " + Math.Round(w[i], 5);
            }

            return line + " ]";
        }
    }
}
