using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.Threading;

namespace GrundWelt
{
    public static class PauseX
    {
        public static bool Pause { get; set; }
    }
    public abstract class TwoPlayerGameLogic<PositionData, ActionData>
        where ActionData : GrundWeltAction
    {

        public Player<PositionData, ActionData> PlayerOne { get; set; }
        public Player<PositionData, ActionData> PlayerTwo { get; set; }

        public Player<PositionData, ActionData> PlayerToMove { get; set; }
        public PositionData Position { get; set; }

        protected readonly List<ActionData> Actions = new List<ActionData>();

        public void StartGame(PositionData startPosition, Player<PositionData, ActionData> playerOne, Player<PositionData, ActionData> playerTwo)
        {
            Actions.Clear();
            PlayerOne = playerOne;
            PlayerOne.OnActionDecided = this.MakeMove;
            PlayerTwo = playerTwo;
            PlayerTwo.OnActionDecided = this.MakeMove;
            Position = startPosition;
            PlayerToMove = playerOne;

            if (!CheckFinishGame())
            {
                Thread.Sleep(1000);
                PlayerToMove.Move(Position);
            }

            while (MatchInfo == null)
                Thread.Sleep(200);
        }
        public MatchInfo MatchInfo { get; set; }

        public void MakeMove(ActionData action)
        {
            try
            {
                while (PauseX.Pause)
                    Thread.Sleep(100);

                ExecuteAction(action);

                if (OnNewMove != null)
                    OnNewMove(action);

                PlayerToMove = PlayerToMove == PlayerOne ? PlayerTwo : PlayerOne;
                if (!CheckFinishGame())
                {
                    Thread.Sleep(400);

                    PlayerToMove.Move(Position);
                }
                else
                {
                    FinishGame();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        protected abstract void FinishGame();
        public abstract void ExecuteAction(ActionData action);

        protected abstract bool CheckFinishGame();

        public Action<ActionData> OnNewMove { get; set; }
    }
    public enum GameRelayStatus
    {
        Wait,
        Broadcast,
        Background,
    }
    public abstract class TwoPlayerGameLogicV2<Player, PositionData, ActionData>
        where ActionData : GrundWeltAction
        where Player : IHas<IPlayerLogic<Player, PositionData, ActionData>>
    {

        public GameRelayStatus RelayStatus { get; set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public Player PlayerToMove { get; set; }
        public PositionData Position { get; set; }

        protected readonly List<ActionData> Actions = new List<ActionData>();

        public void StartGame(PositionData startPosition, Player playerOne, Player playerTwo)
        {
            Actions.Clear();
            PlayerOne = playerOne;
            PlayerOne.Logic.OnActionDecided = this.MakeMove;
            PlayerTwo = playerTwo;
            PlayerTwo.Logic.OnActionDecided = this.MakeMove;
            Position = startPosition;
            PlayerToMove = playerOne;

            if (!CheckFinishGame())
            {
                PlayerToMove.Move(Position);
            }

            while (MatchInfo == null)
                Thread.Sleep(20);
        }
        public MatchInfo MatchInfo { get; set; }

        public void MakeMove(ActionData action)
        {
            try
            {
                while (PauseX.Pause)
                    Thread.Sleep(100);

                ExecuteAction(action);

                if (OnNewMove != null)
                    OnNewMove(action);

                PlayerToMove = PlayerToMove.Equals(PlayerOne) ? PlayerTwo : PlayerOne;
                if (!CheckFinishGame())
                {
                    if (RelayStatus == GameRelayStatus.Broadcast)
                        Thread.Sleep(400);

                    PlayerToMove.Move(Position);
                }
                else
                {
                    FinishGame();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        protected abstract void FinishGame();
        public abstract void ExecuteAction(ActionData action);

        protected abstract bool CheckFinishGame();

        public Action<ActionData> OnNewMove { get; set; }
    }

    public abstract class Player<PositionData, ActionData>
        where ActionData : GrundWeltAction
    {
        public void Awake()
        {
            new Thread(DoAction).Start();
        }
        public void Move(PositionData position)
        {
            CurrentPosition = position;
            MoveRequested = true;
            Awake();
        }
        public Player(Func<PositionData, IEnumerable<ActionData>> findActions)
        {
            FindPossibleActions = findActions;
        }

        public string Name { get; set; }

        protected PositionData CurrentPosition { get; set; }

        public Action<ActionData> OnActionDecided { get; set; }

        public bool MoveRequested { get; set; }

        public void DoAction()
        {
            if (MoveRequested)
            {
                var actions = FindPossibleActions(CurrentPosition);
                if (!actions.Any())
                    return;
                var actionToExecute = ChooseAction(actions);
                //if (actionToExecute.NoAction)
                //    return;
                if (OnActionDecided != null)
                { OnActionDecided(actionToExecute); }
            }
        }

        public readonly Func<PositionData, IEnumerable<ActionData>> FindPossibleActions;

        public abstract ActionData ChooseAction(IEnumerable<ActionData> actions);

    }
}
