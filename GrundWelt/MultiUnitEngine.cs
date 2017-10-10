using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class MultiUnitEngine<T, ActionType> : SimpleWorkStation<IInputData<T>, T>
    {
        public MultiUnitEngine(IEnumerable<GWUnit<T, ActionType>> units, TrackBestResultLogic<T> resultTrackingUnit, int inputBuffer, int outputBuffer)
            : base(inputBuffer, outputBuffer)
        {
            Units = units.ToList();
            ResultTrackingLogic = resultTrackingUnit;
        }

        public IList<GWUnit<T, ActionType>> Units { get; internal set; }

        public TrackBestResultLogic<T> ResultTrackingLogic { get; set; }

        private int lastUnitStarted;
        private IInputData<T> CurrentInputItem;
        protected override void ExecuteWork(IInputData<T> currentInputItem)
        {
            lastUnitStarted = -1;
            this.CurrentInputItem = currentInputItem;

            StartUnit();

            return;
        }

        private void FinishRun()
        {
            ReEntryPoint = this.InternalRun;
            AddOutput(ResultTrackingLogic.BestResult);
        }

        private void StartUnit()
        {
            ReEntryPoint = () => StartUnit();
            if (lastUnitStarted + 1 < Units.Count)
            {
                var unit = Units[lastUnitStarted + 1];
                unit.InputStack.Add(CurrentInputItem.ToPositionData());
                unit.NewOutput.AddPath((res) => ResultTrackingLogic.AddResult(res));
                unit.AwakeFollowUp.AddLast(this);
                //ResultTrackingUnit.TrackUnit(unit);
                lastUnitStarted++;
                unit.Awake();
            }
            else
            {
                FinishRun();
            }
            this.GoSleep();
        }
    }
}
