using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class SendPhalanxMessage : GWRequest<SendPhalanxMessage>
    {
        public SendPhalanxMessage(SpeedChangeMessage msg)
        {
            Message = msg;
        }
        public readonly SpeedChangeMessage Message;
    }

    public enum SpeedChangeMessage
    {
        Stop,
        SpeedUp,
        SlowDown
    }
    public interface IOptimizationControl<DataType> : IComputationInfo
    {
        void Start();
        void SpeedUp();
        void SlowDown();
        void Stop();

        DataType Data { get; }
        ICRModificationLogic Update { get; }
    }

    public static class OptimizationControlX
    {
        //public static IOptimizationControl<DataType> Create<DataType, InputData, PositionData, ActionData>(IOptimizationCenter<InputData, PositionData, ActionData> center, DataType data, ICRModificationLogic update, Action start)
        //{
        //    return new OptimizationControl<InputData, PositionData, ActionData, DataType>(center, data, update, start);
        //}
    }
    public interface IComputationInfo
    {
        double Progress { get; set; }
        bool IsActive { get; set; }
    }
    class ComputationInfo : IComputationInfo
    {
        private double progress;

        public double Progress
        {
            get { return progress; }
            set { progress = From + value / Length; }
        }

        public bool IsActive { get; set; }

        public double From { get; set; } = 0.0;
        public double Length { get; set; } = 1.0;
    }
}
