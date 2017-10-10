
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class SimpleWorkStation<InputData, OutputData> : GWCell
    {
        public SimpleWorkStation(int inputBufferSize, int outputBufferSize)
        {
            InputStack = new Buffer<InputData>();
            OutputStack = new Buffer<OutputData>();
        }

        public Buffer<InputData> InputStack;
        protected Buffer<OutputData> OutputStack;
        public MEvent<OutputData> NewOutput = new MEvent<OutputData>();

        protected void AddOutput(OutputData data)
        {
            OutputStack.Add(data);
            NewOutput.Enter(data);
        }

        protected override void DoWork()
        {
            //while (true)
            {
                var currentInputItem = InputStack.Get();
                if (currentInputItem == null)
                    return;
                ExecuteWork(currentInputItem);
                //OutputStack.Add(result);
            }
        }

        protected abstract void ExecuteWork(InputData currentInputItem);
    }
}
