﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolApp
{
    public class Build
    {
        public static void Do()
        {
            new UserInputManager();
            new UserDecisionManager();
            new ShowGraphsManager();
        }
    }
}
