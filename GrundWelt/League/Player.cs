using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class IPlayerLogic<PlayerType, PositionData, ActionData> : IndividualLogic<PlayerType>
    {
        public abstract PlayerType Mate(PlayerType mate);

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
        public string Name { get; set; }

        public PlayerCard PlayerCard { get; set; }


        protected PositionData CurrentPosition { get; set; }

        public Action<ActionData> OnActionDecided { get; set; }

        public bool MoveRequested { get; set; }

        public void DoAction()
        {
            if (MoveRequested)
            {
                var actions = FindPossibleActions(CurrentPosition).ToList();
                if (!actions.Any())
                    return;
                var actionToExecute = ChooseAction(actions);
                //if (actionToExecute.NoAction)
                //    return;
                if (OnActionDecided != null)
                { OnActionDecided(actionToExecute); }
            }
        }

        public IList<double> Weights { get; set; }


        public abstract IEnumerable<ActionData> FindPossibleActions(PositionData position);

        public abstract ActionData ChooseAction(IEnumerable<ActionData> actions);

        public Guid GWId { get; set; }

        public double[] Code
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public PlayerType Owner
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class PlayerX
    {
        public static void Move<PlayerType, PositionData, ActionData>(this IHas<IPlayerLogic<PlayerType, PositionData, ActionData>> player, PositionData position)
        {
            player.Logic.Move(position);
        }
    }
}
