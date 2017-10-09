using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class AnalyzeClassification
    {
        public static ClassificationResultLogic Do<T>(IDictionary<T,bool> classification, IDictionary<T, bool> reference)
        {
            var logic = new ClassificationResultLogic();

            try
            {
                foreach (var entry in classification)
                {
                    if (entry.Value)
                    {
                        if (reference[entry.Key])
                            logic.TruePositives++;
                        else
                            logic.FalsePositives++;
                    }
                    else
                    {
                        if (reference[entry.Key])
                            logic.FalseNegatives++;
                        else
                            logic.TrueNegatives++;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            return logic;
        }
    }
}
