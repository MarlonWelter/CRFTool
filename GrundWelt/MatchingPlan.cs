using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match = CodeBase.AgO<GrundWelt.PlayerCard, GrundWelt.PlayerCard>;

namespace GrundWelt
{
    public class MatchingPlan8Player
    {
        public MatchingPlan8Player(params PlayerCard[] players)
        {
            for (int i = 0; i < 8; i++)
            {
                Players[i] = players[i];
            }
        }

        public IEnumerable<Match> Matches(int round)
        {
            switch (round)
            {
                case 0:
                    yield return new Match(Players[0], Players[4]);
                    yield return new Match(Players[1], Players[5]);
                    yield return new Match(Players[2], Players[6]);
                    yield return new Match(Players[3], Players[7]);
                    break;
                case 1:
                    yield return new Match(Players[0], Players[5]);
                    yield return new Match(Players[1], Players[6]);
                    yield return new Match(Players[2], Players[7]);
                    yield return new Match(Players[3], Players[4]);
                    break;
                case 2:
                    yield return new Match(Players[0], Players[6]);
                    yield return new Match(Players[1], Players[7]);
                    yield return new Match(Players[2], Players[4]);
                    yield return new Match(Players[3], Players[5]);
                    break;
                case 3:
                    yield return new Match(Players[0], Players[7]);
                    yield return new Match(Players[1], Players[4]);
                    yield return new Match(Players[2], Players[5]);
                    yield return new Match(Players[3], Players[6]);
                    break;
                case 4:
                    yield return new Match(Players[0], Players[2]);
                    yield return new Match(Players[1], Players[3]);
                    yield return new Match(Players[4], Players[6]);
                    yield return new Match(Players[5], Players[7]);
                    break;
                case 5:
                    yield return new Match(Players[0], Players[3]);
                    yield return new Match(Players[1], Players[2]);
                    yield return new Match(Players[4], Players[7]);
                    yield return new Match(Players[5], Players[6]);
                    break;
                case 6:
                    yield return new Match(Players[0], Players[1]);
                    yield return new Match(Players[2], Players[3]);
                    yield return new Match(Players[4], Players[5]);
                    yield return new Match(Players[6], Players[7]);
                    break;
                default:
                    break;
            }
        }



        public PlayerCard[] Players = new PlayerCard[8];
    }
}
