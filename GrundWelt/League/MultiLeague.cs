using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{
    public class MultiLeague<PositionData, ActionData>
    {

        public League<PositionData, ActionData>[] Leagues { get; protected set; }


        public int LeagueSizes { get; set; }
        public void StartSeason()
        {
            for (int matchDay = 0; matchDay < LeagueSizes; matchDay++)
            {
                foreach (var league in Leagues)
                {
                    league.ExecuteMatchDay();
                }
            }

            //Handle promotion & relegation
            for (int league = 0; league < Leagues.Length; league++)
            {
                if (league < Leagues.Length - 1)
                {
                    var dem1 = Leagues[league].Standings[LeagueSizes - 2];
                    var dem2 = Leagues[league].Standings[LeagueSizes - 1];
                    var prom1 = Leagues[league - 1].Standings[LeagueSizes - 2];
                    var prom2 = Leagues[league - 1].Standings[LeagueSizes - 1];
                }
            }
        }
    }

  
}
