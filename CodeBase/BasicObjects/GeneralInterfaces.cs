using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface IWeighted
    {
        double Weight { get; set; }
    }

    public static class WeightedExtensions
    {
        public static readonly Random Rand = new Random();
        public static T ChooseByWeight<T>(this IEnumerable<T> elements)
            where T : IWeighted
        {
            var sum = elements.Sum(e => e.Weight);
            double choice = Rand.NextDouble() * sum;
            T value = default(T);
            foreach (var entry in elements)
            {
                value = entry;
                choice -= entry.Weight;
                if (choice <= 0)
                    return value;
            }

            throw new Exception("Couldn't choose an element.");
        }
        public static T ChooseByWeight<T>(this IEnumerable<AgO<T, double>> elements)
        {
            var sum = elements.Sum(e => e.Data2);
            double choice = Rand.NextDouble() * sum;
            T value = default(T);
            foreach (var entry in elements)
            {
                value = entry.Data1;
                choice -= entry.Data2;
                if (choice <= 0)
                    return value;
            }

            throw new Exception("Couldn't choose an element.");
        }
        public static T ChooseByProb<T>(this IEnumerable<AgO<T, double>> elements, Random rand)
        {
            double choice = rand.NextDouble();
            T value = default(T);
            foreach (var entry in elements)
            {
                value = entry.Data1;
                choice -= entry.Data2;
                if (choice <= 0)
                    return value;
            }
            return ChooseByWeight(elements);
        }
    }
}
