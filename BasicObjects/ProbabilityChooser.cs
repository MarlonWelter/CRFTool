
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public static class ProbabilityChooser
    {
        public static Random Rand = new Random();
        public static T ChooseByDistribution<T>(this IDictionary<T, double> distribution)
        {
            double choice = Rand.NextDouble();
            T value = default(T);
            foreach (var entry in distribution)
            {
                value = entry.Key;
                choice -= entry.Value;
                if (choice <= 0)
                    return value;
            }
            
            return distribution.ChooseByDistributionNormalize<T>();
        }

        public static T ChooseByDistributionNormalize<T>(this IDictionary<T, double> distribution)
        {
            double choice = Rand.NextDouble() * distribution.Sum(kvp => kvp.Value);
            T value = default(T);
            foreach (var entry in distribution)
            {
                value = entry.Key;
                choice -= entry.Value;
                if (choice <= 0)
                    return value;
            }

            //this might happen if there are negative values.

            Log.Post("invalid data to randomly choose an element.", LogCategory.Critical);
            return default(T);
        }

        //public static T ChooseByDistribution<T>(this IEnumerable<AgO<T, double>> distribution)
        //{
        //    double choice = Rand.NextDouble();
        //    T value = default(T);
        //    foreach (var entry in distribution)
        //    {
        //        value = entry.Data1;
        //        choice -= entry.Data2;
        //        if (choice <= 0)
        //            return value;
        //    }
        
        //    return distribution.ChooseByDistribution();
        //}
    }
}
