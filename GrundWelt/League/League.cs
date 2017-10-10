using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GrundWelt
{

    public abstract class League<PositionData, ActionData>
    {

        public PlayerCard[] Standings { get; set; }

        public int Promotions { get; set; }
        public int Relegations { get; set; }

        public int CurrentMatchDay { get; set; }
        public void ExecuteMatchDay()
        {
            var matches = CreateMatching(CurrentMatchDay);

            foreach (var match in matches)
            {
                var playerOne = match.Data1;
                var playerTwo = match.Data2;
                new AnnounceMatch(playerOne, playerTwo).RequestInDefaultContext();
                var matchInfo = Match(playerOne, playerTwo);
                playerOne.Matches.AddLast(matchInfo);
                playerTwo.Matches.AddLast(matchInfo);
                switch (matchInfo.Result)
                {
                    case MatchResult.PlayerOneWins:
                        playerOne.Points += 2.0;
                        break;
                    case MatchResult.PlayerTwoWins:
                        playerTwo.Points += 2.0;
                        break;
                    case MatchResult.Draw:
                        playerOne.Points += 1.0;
                        playerTwo.Points += 2.0;
                        break;
                    case MatchResult.Aborted:
                        break;
                    default:
                        break;
                }
            }

            var players = Standings.OrderBy(pl => pl.Points).ToList();
            for (int k = 0; k < Standings.Length; k++)
            {
                Standings[k] = players[k];
            }
            CurrentMatchDay++;
        }

        public void StartSeason()
        {
            CurrentMatchDay = 0;
            for (int i = 0; i < Standings.Length - 1; i++)
            {
                ExecuteMatchDay();
            }
        }

        public abstract IEnumerable<AgO<PlayerCard, PlayerCard>> CreateMatching(int round);


        public abstract MatchInfo Match(PlayerCard playerOne, PlayerCard playerTwo);
    }

    public class AnnounceMatch : IHas<IRequestLogic<AnnounceMatch>>
    {
        public AnnounceMatch(PlayerCard playerOne, PlayerCard playerTwo)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
        }
        public PlayerCard PlayerOne { get; set; }
        public PlayerCard PlayerTwo { get; set; }

        public Guid GWId { get; set; }

        private RequestLogic<AnnounceMatch> logic = new RequestLogic<AnnounceMatch>();
        public IRequestLogic<AnnounceMatch> Logic
        {
            get { return logic; }
        }
    }



    public class PlayerCard
    {
        public PlayerCard(Guid Id1, string name)
        {
            Name = name;
            Id = Id1;
        }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public double Points { get; set; }
        private LinkedList<MatchInfo> matches = new LinkedList<MatchInfo>();

        public LinkedList<MatchInfo> Matches
        {
            get { return matches; }
            set { matches = value; }
        }
    }

    public enum MatchResult
    {
        PlayerOneWins,
        PlayerTwoWins,
        Draw,
        Aborted
    }
    public class MatchInfo
    {
        public MatchInfo()
        {

        }
        public MatchInfo(PlayerCard playerOne, PlayerCard playerTwo)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
        }
        public MatchResult Result { get; set; }
        public PlayerCard PlayerOne { get; set; }
        public PlayerCard PlayerTwo { get; set; }
    }
}
