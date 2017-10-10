using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public interface IUniverse<PositionData>
    {

    }


    //public interface IInputData<T>
    //    where T : IUniverse<PositionData>
    //{
    //}


    //public interface IResultData<T>
    //    where T : IUniverse<PositionData>
    //{
    //}


    public interface IInputData<T>
    {
        T ToPositionData();
    }

    public interface IPositionData<ActionType>
    {

    }



    public interface IStrategy<PositionData>
    {
        bool Apply(PositionData position);
    }

    public interface IModel<T>
    {

    }

    public interface IEvaluationMethod<T>
    {
        double Evaluate(T position);
    }

   


    public abstract class Runner<T> : GWCell
    {
        public MEvent<Runner<T>> EndRun = new MEvent<Runner<T>>();
        protected abstract void Run();

        public T CurrentPosition { get; set; }
        protected override void DoWork()
        {
            Run();
            EndRun.Enter(this);
        }
    }

    public interface IResultTrackingUnit<T>
    {

    }
}
