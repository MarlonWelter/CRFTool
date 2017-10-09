
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    /**
     * Nodes: The nodes, that needs to be brought to order
     * 
     * StartPatch: The nodes that are allready selected
     * 
     * 
     * */
    public class ComputeOrder : IHas<IRequestLogic<ComputeOrder>>
    {
        public ComputeOrder(IEnumerable<IGWNode> nodes, IEnumerable<IGWNode> startPatch)
        {
            StartPatch = startPatch;
            Nodes = nodes;
        }

        private LinkedList<IGWNode> orderedNodes = new LinkedList<IGWNode>();
        public LinkedList<IGWNode> OrderedNodes
        {
            get { return orderedNodes; }
            set { orderedNodes = value; }
        }

        public IEnumerable<IGWNode> StartPatch { get; set; }
        public IEnumerable<IGWNode> Nodes { get; set; }


        private RequestLogic<ComputeOrder> logic = new RequestLogic<ComputeOrder>();
        public IRequestLogic<ComputeOrder> Logic { get { return logic; } }
        public Guid GWId { get; set; }
    }

    class ComputeOrderManager : IRequestListener
    {
        public ComputeOrderManager(IGWContext context = null)
        {
            Context = context;
            Register();
        }
        public IGWContext Context { get; set; }
        public void Register()
        {
            this.DoRegister<ComputeOrder>(OnComputeOrder);
        }

        private void OnComputeOrder(ComputeOrder obj)
        {
            var resultingOrder = GreedyMinBorderQueueComputing.ComputeQueue(obj.Nodes.ToList(), obj.StartPatch);
            obj.OrderedNodes = resultingOrder;
        }

        public void Unregister()
        {
            this.DoUnregister<ComputeOrder>(OnComputeOrder);
        }
    }
}
