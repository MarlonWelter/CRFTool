using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.QueueHeuristic
{

    class MinBorderActionData
    {
        public MinBorderActionData(MinBorderPositionData position, IGWNode node, MinBorderOperations operation)
        {
            Node = node;
            Operation = operation;
        }
        public IGWNode Node { get; set; }
        public MinBorderOperations Operation { get; set; }
    }

    enum MinBorderOperations
    {
        Add,
        Remove
    }

    class MinBorderInput
    {
        public MinBorderInput(IEnumerable<IGWNode> nodes)
        {
            Items = nodes;
        }


        public IEnumerable<IGWNode> Items { get; set; }

        public MinBorderPositionData ToPositionData()
        {
            var startPosition = new MinBorderPositionData();

            startPosition.UnQueuedNodes.AddRange(Items);

            return startPosition;
        }
    }

    class MinBorderPositionData
    {
        public MinBorderPositionData()
        {
            Queue = new LinkedList<IGWNode>();
            UnQueuedNodes = new LinkedList<IGWNode>();
            Border = new LinkedList<IGWNode>();
        }
        public int MaxBorder { get; set; }
        public LinkedList<IGWNode> Queue { get; private set; }
        public LinkedList<IGWNode> Border { get; private set; }
        public LinkedList<IGWNode> UnQueuedNodes { get; private set; }

    }


}
