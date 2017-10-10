using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class MultiStrategyGreedyEngine<T> : SimpleWorkStation<IInputData<T>, T>
    {
        public MultiStrategyGreedyEngine(IEnumerable<Runner<T>> units, TrackBestResultLogic<T> resultTrackingUnit, int inputBuffer, int outputBuffer)
            : base(inputBuffer, outputBuffer)
        {
            Units = units.ToList();
            ResultTrackingUnit = resultTrackingUnit;
        }

        public IList<Runner<T>> Units { get; internal set; }

        public TrackBestResultLogic<T> ResultTrackingUnit { get; set; }

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
            AddOutput(ResultTrackingUnit.BestResult);
        }

        private void StartUnit()
        {
            ReEntryPoint = () => StartUnit();
            if (lastUnitStarted + 1 < Units.Count)
            {
                var unit = Units[lastUnitStarted + 1];
                unit.CurrentPosition = CurrentInputItem.ToPositionData();
                unit.AwakeFollowUp.AddLast(this);
                ResultTrackingUnit.TrackUnit(unit);
                unit.Awake();
                lastUnitStarted++;
            }
            else
            {
                FinishRun();
            }
            this.GoSleep();
        }
    }
}
