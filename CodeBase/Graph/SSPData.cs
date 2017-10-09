using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class SSPND
    {
        public SSPND(string name, string category, double value)
        {
            Name = name;
            Category = category;
            Value = value;
        }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Classifiation { get; set; }

        public int IDLabel { get; set; }
        public double Value { get; set; }
    }

    public class SSPED
    {
        public SSPED(double weight, string type)
        {
            Weight = weight;
            Type = type;
        }
        public double Weight { get; set; }
        public string Type { get; set; }
    }

    public class SSPGD
    {
        public List<string> Categories { get; set; }
        public CategoryGraph CategoryGraph { get; set; }
    }


}
