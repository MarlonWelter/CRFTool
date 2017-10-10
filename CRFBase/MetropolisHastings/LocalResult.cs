using System;

namespace CRFBase
{
    //stores homogenity and distinction ratios for each category of a graph 
    internal class LocalResult
    {   
        public LocalResult(int numberCategories, double conformity, double correlation)
        {
            Homs = new double[numberCategories];
            Distincts = new double[numberCategories];
            Conformity = conformity;
            Correlation = correlation;
        }
        public double Conformity { get; set; }
        public double Correlation { get; set; }
        //homogenities
        public double[] Homs { get; set; }
        //distinctions
        public double[] Distincts { get; set; }

        public double AvgHomogenity { get; set; }

        public double AvgDistinction { get; set; }
       
        public double ResultValue { get; set; }

        //adds values to current object
        public void AddValues(LocalResult resultToAdd)
        {
            if (Homs.Length != resultToAdd.Homs.Length)
            {
                throw new ArgumentException("LocalResults have wrong formats");
            }

            for(int i = 0; i < Homs.Length; i++)
            {
                Homs[i] += resultToAdd.Homs[i];
                Distincts[i] += resultToAdd.Distincts[i];
            }
        }


    }
}