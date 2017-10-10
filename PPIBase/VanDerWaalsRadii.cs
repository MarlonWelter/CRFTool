
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public static class VanDerWaalsRadii
    {
        private static IDictionary<string, double> mapping = new Dictionary<string, double>();

        private static bool init = false;


        public static double GetRadius(string atom)
        {
            var check = atom.Trim().Substring(0, 1);
            if (!init)
            {
                mapping.Add("H", 1.1);
                mapping.Add("C", 1.7);
                mapping.Add("N", 1.55);
                mapping.Add("O", 1.52);
                mapping.Add("S", 1.8);
                init = true;
            }
            try
            {
                return mapping[check];
            }
            catch
            {
                Log.Post("Cannot find entry for atom " + atom, LogCategory.Inconsistency);
                return -1;
            }
        }
    }
}
