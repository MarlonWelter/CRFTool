using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace CRFToolAppBase
{
    public class SoftwareGraphIsing : IsingData
    {
        public SoftwareGraphIsing()
        {
            CorrelationParameter = 1.0;
            ConformityParameter = 1.0;
        }
    }
    public class ProteinGraphIsing : IsingData
    {
        public ProteinGraphIsing()
        {
            CorrelationParameter = 1.0;
            ConformityParameter = 1.0;
        }
    }
}
