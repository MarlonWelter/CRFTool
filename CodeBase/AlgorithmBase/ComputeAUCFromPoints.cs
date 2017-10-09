
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class ComputeAUCFromPoints
    {
        public static double Do(IEnumerable<AgO<double, double>> points)
        {
            var ptsOrdered = points.OrderBy(p => p.Data1).ToList();
            ptsOrdered.Insert(0, new AgO<double, double>(0,0));
            ptsOrdered.Add(new AgO<double, double>(1.0,1.0));
            var auc = 0.0;

            for (int i = 0; i < ptsOrdered.Count()-1; i++)
            {
                var area = ((ptsOrdered[i].Data2 + ptsOrdered[i + 1].Data2) / 2.0) * (ptsOrdered[i + 1].Data1 - ptsOrdered[i].Data1);
                auc += area;
            }

            return auc;
        }
    }
}
