using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class PrimeFactorisation
    {
        /// <summary>
        /// Initializes a new instance of the PrimeFactorisation class.
        /// </summary>
        public PrimeFactorisation()
        {
            Primes = new List<int>();
            Multiplicity = new List<int>();
        }
        public int TheNumber { get; set; }
        public List<int> Primes { get; set; }
        public List<int> Multiplicity { get; set; }
        public int PrimeFactors
        {
            get
            {
                int sum = 0;
                foreach (var item in Multiplicity)
                {
                    sum += item;
                }
                return sum;
            }
        }
        public IList<int> PrimeEnumeration()
        {
            IList<int> list = new List<int>();
            for (int i = 0; i < Multiplicity.Count; i++)
            {
                for (int p = 0; p < Multiplicity[i]; p++)
                {
                    list.Add(Primes[i]);
                }
            }
            return list;
        }
    }
}
