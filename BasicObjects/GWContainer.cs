using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class GWContainer : IHas<IContainerLogic>
    {
        public GWContainer()
        {

        }
        public GWContainer(IHas<IContainerLogic> model)
        {
            this.TakeSimpleValues(model);
        }

        private ContainerLogic logic = new ContainerLogic();
        public IContainerLogic Logic
        {
            get { return logic; }
        }
        private Guid gwId = Guid.NewGuid();

        public Guid GWId
        {
            get { return gwId; }
            set { gwId = value; }
        }

    }
}
