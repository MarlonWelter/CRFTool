
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CodeBase
{
    public abstract class Producer<ProducedType>
    {
        private MEvent<ProducedType> producedItem = new MEvent<ProducedType>();
        public ICRModificationLogic<ProducedType> ProducedItem
        {
            get { return producedItem; }
        }        private Task task;

        public Task Task
        {
            get { return task; }
            private set { task = value; }
        }


        public int SleepingTime { get; set; }
        public bool IsActive { get; set; }

        public void Start()
        {
            var producer = new Task(LifeCycle);
            Task = producer;
            IsActive = true;
            producer.Start();
        }

        private void LifeCycle()
        {
            while(IsActive)
            {
                var product = Produce();
                if (product != null)
                {
                    producedItem.Enter(product);
                }
                Thread.Sleep(SleepingTime);
            }
        }

        public abstract ProducedType Produce();

        public void Stop()
        {
            IsActive = false;
        }
    }
}
