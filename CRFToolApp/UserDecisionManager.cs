using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolApp
{
    class UserDecisionManager : GWManager<UserDecision>
    {
        protected override void OnRequest(UserDecision request)
        {
            var window = new UserDecisionUI();
            window.SetUserOptions(request);
            window.ShowDialog();
        }
    }
}
