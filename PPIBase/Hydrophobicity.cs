using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public static class Hydrophobicity
    {
        private static Dictionary<string, double> values = new Dictionary<string, double>();


        private static void init()
        {
            values.Add("ala", 1.8);
            values.Add("arg", -4.5);
            values.Add("asn", -3.5);
            values.Add("asp", -3.5);
            values.Add("cys", 2.5);
            values.Add("gln", -3.5);
            values.Add("glu", -3.5);
            values.Add("gly", -0.4);
            values.Add("his", -3.2);
            values.Add("ile", 4.5);
            values.Add("leu", 3.8);
            values.Add("lys", -3.9);
            values.Add("met", 1.9);
            values.Add("phe", 2.8);
            values.Add("pro", -1.6);
            values.Add("ser", -0.8);
            values.Add("thr", -0.7);
            values.Add("trp", -0.9);
            values.Add("tyr", -1.3);
            values.Add("val", 4.2);
        }

        public static double GetHydrophobicity(string aminoacid)
        {
            if (values.Count == 0)
                init();
            try
            {
                return values[aminoacid.ToLower()];
            }
            catch
            {
                return 0.0;
            }
        }
    }
    public static class Polarity
    {
        private static Dictionary<string, bool> values = new Dictionary<string, bool>();


        private static void init()
        {
            values.Add("ala", false);
            values.Add("arg", true);
            values.Add("asn", true);
            values.Add("asp", true);
            values.Add("cys", true);
            values.Add("gln", true);
            values.Add("glu", true);
            values.Add("gly", false);
            values.Add("his", true);
            values.Add("ile", false);
            values.Add("leu", false);
            values.Add("lys", true);
            values.Add("met", false);
            values.Add("phe", false);
            values.Add("pro", false);
            values.Add("ser", true);
            values.Add("thr", true);
            values.Add("trp", false);
            values.Add("tyr", true);
            values.Add("val", false);
        }

        public static bool IsPolar(string aminoacid)
        {
            if (values.Count == 0)
                init();
            try
            {
                return values[aminoacid.ToLower()];
            }
            catch
            {
                return false;
            }
        }
    }
}
