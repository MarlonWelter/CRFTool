
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class LWQuboid : IHas<IQuboidLogic>
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }	

        private IQuboidLogic logic = new QuboidLogic();
        public IQuboidLogic Logic
        {
            get { return logic; }
        }


        public LWQuboid(double width, double length, double height)
        {
            this.Width(width);
            this.Length(length);
            this.Height(height);
        }
        public LWQuboid()
        {
        }
        public LWQuboid(IHas<IQuboidLogic> quboid)
        {
            this.TakeSimpleValues(quboid);
        }
    }
}
