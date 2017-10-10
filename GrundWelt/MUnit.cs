
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrundWelt
{
    public enum UnitState
    {
        Awake,
        Sleeping
    }


    //public abstract class GWUnit<PositionData> : SimpleWorkStation<IEnumerable<PositionData>, IEnumerable<PositionData>>
    //{
    //    protected override void ExecuteWork(IEnumerable<PositionData> currentInputItem)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public abstract class GWCell
    {
        public GWCell()
        {
            State = UnitState.Sleeping;
            ReEntryPoint = () => InternalRun();
        }

        public string Name { get; set; }

        public UnitState State { get; protected set; }

        public bool WorkCall { get; set; }

        protected Action ReEntryPoint;
        public LinkedList<GWCell> AwakeFollowUp = new LinkedList<GWCell>();
        protected abstract void DoWork();
        protected void InternalRun()
        {
            State = UnitState.Awake;
            DoWork();
            GoSleep();
        }
        public void Awake()
        {
            if (State == UnitState.Sleeping)
            {
                State = UnitState.Awake;
                Log.Post(Name + ": awakened");
                new Thread(() => ReEntryPoint()).Start();
            }
            else
            {
                WorkCall = true;
                Log.Post("Tried awaking unit " + this.Name + " but it is awake already.");
            }
        }

        protected void GoSleep()
        {
            State = UnitState.Sleeping;
            Log.Post(Name + ": sleeping");
            foreach (var unit in AwakeFollowUp)
            {
                unit.Awake();
            }
            if (WorkCall)
            {
                WorkCall = false;
                Awake();
            }
        }
    }
}
