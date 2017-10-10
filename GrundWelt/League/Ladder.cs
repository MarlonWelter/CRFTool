using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class RandomPairingLadder<Player, PositionData, ActionData>
         where Player : IHas<IPlayerLogic<Player, PositionData, ActionData>>
    {
        public List<Player> PlayersTable = new List<Player>();
        public int MatchesPerSeason { get; set; }

        public void SetPlayers(IList<Player> players, int matchesPerSeason)
        {
            PlayersTable = players.ToList();
            MatchesPerSeason = matchesPerSeason;
        }

        private LinkedList<MatchInfo> currentMatches = new LinkedList<MatchInfo>();

        public void PlayNextMatch(GameRelayStatus relaystatus)
        {
            if (!currentMatches.NotNullOrEmpty())
            {
                DoPairings();
            }
            var nextMatch = currentMatches.First();
            currentMatches.RemoveFirst();
            PlayMatch(nextMatch, relaystatus);
        }
        public void PlayMatchDay(GameRelayStatus relayStatus)
        {
            if (!currentMatches.NotNullOrEmpty())
            {
                DoPairings();
            }
            foreach (var match in currentMatches)
            {
                PlayMatch(match, relayStatus);
            }
        }

        private void DoPairings()
        {
            var allPlayers = PlayersTable.ToList();

            while (allPlayers.Count > 0)
            {
                var playerOneIndex = Program.Random.Next(allPlayers.Count);
                var playerOne = allPlayers[playerOneIndex];
                allPlayers.RemoveAt(playerOneIndex);
                var playerTwoIndex = Program.Random.Next(allPlayers.Count);
                var playerTwo = allPlayers[playerTwoIndex];
                allPlayers.RemoveAt(playerTwoIndex);
                currentMatches.Add(new MatchInfo(playerOne.Logic.PlayerCard, playerTwo.Logic.PlayerCard));
            }
        }

        private void PlayMatch(MatchInfo match, GameRelayStatus relayStatus)
        {
            if (relayStatus == GameRelayStatus.Broadcast)
                new AnnounceMatch(match.PlayerOne, match.PlayerTwo).RequestInDefaultContext();

            Match(match);

            match.PlayerOne.Matches.AddLast(match);
            match.PlayerTwo.Matches.AddLast(match);
            switch (match.Result)
            {
                case MatchResult.PlayerOneWins:
                    match.PlayerOne.Points += 2.0;
                    break;
                case MatchResult.PlayerTwoWins:
                    match.PlayerTwo.Points += 2.0;
                    break;
                case MatchResult.Draw:
                    match.PlayerOne.Points += 1.0;
                    match.PlayerTwo.Points += 1.0;
                    break;
                case MatchResult.Aborted:
                    break;
                default:
                    break;
            }

            //order players 
            PlayersTable = PlayersTable.OrderByDescending(pl => pl.Logic.PlayerCard.Points).ToList();
        }

        public void Mate()
        {
            var players = PlayersTable.ToList();
            for (int i = 0; i < (double)PlayersTable.Count / 2; i++)
            {
                var mother = players[i];
                var father = players[IndividualX.Random.Next(PlayersTable.Count)];
                var offspring = mother.Logic.Mate(father);
                PlayersTable[i + (int)((double)PlayersTable.Count / 2)] = offspring;
            }
        }

        protected abstract void Match(MatchInfo match);
    }
}
