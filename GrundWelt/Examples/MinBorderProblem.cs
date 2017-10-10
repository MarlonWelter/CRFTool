using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{

    public class MinBorderUnit : GWUnit<MinBorderInstance, MinBorderAction>
    {
        public MinBorderUnit(IEvaluationMethod<MinBorderAction> actionEvaluation)
            : base(new MinBorderMind(actionEvaluation, new MinBorderBody()))
        {

        }
    }

    public class MinBorderBigUnit : GWUnit<MinBorderInstance, MinBorderAction>
    {
        public MinBorderBigUnit(IEvaluationMethod<MinBorderAction> actionEvaluation, int size)
            : base(new MinBorderBigMind(actionEvaluation, size, new MinBorderBigBody(size)))
        {

        }
    }

    public class MinBorderBody : GWBody<MinBorderInstance, MinBorderAction>
    {

        public override void ExecuteAction(MinBorderAction action)
        {
            var nextNode = action.NextNode;
            var position = action.Position;
            if (!action.BorderChangeSet)
            {
                var borderSizeChange = nextNode.Neighbours().Any(nb => !nb.IsInOrder) ? 1 : 0;
                foreach (var neighbour in nextNode.Neighbours())
                {
                    if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 1)
                    {
                        borderSizeChange--;
                    }
                }
                action.BorderChange = borderSizeChange;
                action.BorderChangeSet = true;
            }

            nextNode.IsInOrder = true;
            nextNode.IsInBorder = nextNode.UnSelectedNeighbours > 0;
            foreach (var neighbour in nextNode.Neighbours())
            {
                //neighbour.IsInBorder = true;
                neighbour.UnSelectedNeighbours--;
                if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 0)
                {
                    neighbour.IsInBorder = false;
                }
            }

            position.CurrentBorderSize += action.BorderChange;
            position.MaxBorderSize = Math.Max(position.MaxBorderSize, position.CurrentBorderSize);
            position.NodeOrder[nextNode.OrderId] = position.NodesInOrder;
            position.UnorderedNodes.Remove(nextNode);
            position.NodesInOrder++;
        }
    }
    public class MinBorderBigBody : GWBigBody<MinBorderInstance, MinBorderAction>
    {

        public MinBorderBigBody(int size)
            : base(size)
        {

        }

        public override void ExecuteActions(IEnumerable<MinBorderAction> actions)
        {
            int counter = 0;
            foreach (var action in actions)
            {
                var nextNode = action.NextNode;
                var position = new MinBorderInstance();
                position.CurrentBorderSize = action.Position.CurrentBorderSize;
                position.MaxBorderSize = action.Position.MaxBorderSize;
                position.NodesInOrder = action.Position.NodesInOrder;
                position.Nodes = action.Position.Nodes;
                position.NodeOrder = new int[action.Position.NodeOrder.Length];
                Array.Copy(action.Position.NodeOrder, position.NodeOrder, position.NodeOrder.Length);
                position.UnorderedNodes.AddRange(action.Position.UnorderedNodes);

                CurrentPositions[counter] = position;
                counter++;

                if (!action.BorderChangeSet)
                {
                    var borderSizeChange = nextNode.Neighbours().Any(nb => !nb.IsInOrder) ? 1 : 0;
                    foreach (var neighbour in nextNode.Neighbours())
                    {
                        if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 1)
                        {
                            borderSizeChange--;
                        }
                    }
                    action.BorderChange = borderSizeChange;
                    action.BorderChangeSet = true;
                }

                nextNode.IsInOrder = true;
                nextNode.IsInBorder = nextNode.UnSelectedNeighbours > 0;
                foreach (var neighbour in nextNode.Neighbours())
                {
                    //neighbour.IsInBorder = true;
                    neighbour.UnSelectedNeighbours--;
                    if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 0)
                    {
                        neighbour.IsInBorder = false;
                    }
                }

                position.CurrentBorderSize += action.BorderChange;
                position.MaxBorderSize = Math.Max(position.MaxBorderSize, position.CurrentBorderSize);
                position.NodeOrder[nextNode.OrderId] = position.NodesInOrder;
                position.NodesInOrder++;
                position.UnorderedNodes.Remove(nextNode);
            }
        }
    }
    public class MinBorderMind : GWMind<MinBorderInstance, MinBorderAction>
    {
        public MinBorderMind(IEvaluationMethod<MinBorderAction> evaluationMethod, MinBorderBody body)
            : base(evaluationMethod)
        {
            Body = body;
        }
        protected override IEnumerable<MinBorderAction> FindPossibleActions(MinBorderInstance position)
        {
            return position.UnorderedNodes.Select(n => new MinBorderAction(position, n));
        }
    }
    public class MinBorderBigMind : GWBigMind<MinBorderInstance, MinBorderAction>
    {
        public MinBorderBigMind(IEvaluationMethod<MinBorderAction> evaluationMethod, int size, MinBorderBigBody body)
            : base(evaluationMethod, size, body)
        {
        }
        protected override IEnumerable<MinBorderAction> FindPossibleActions(MinBorderInstance position)
        {
            return position.UnorderedNodes.Select(n => new MinBorderAction(position, n));
        }
    }

    public class MinBorderAction
    {
        public MinBorderAction(MinBorderInstance position, MinBorderNode nextNode)
        {
            Position = position;
            NextNode = nextNode;
        }
        public MinBorderInstance Position { get; set; }

        public MinBorderNode NextNode { get; set; }

        public bool BorderChangeSet = false;
        public int BorderChange { get; set; }
    }
    public class MinBorderInstance
    {
        public int MaxBorderSize { get; set; }
        public int CurrentBorderSize { get; set; }

        public int[] NodeOrder { get; set; }

        public int NodesInOrder { get; set; }

        public int[] UnchosenNeighbours { get; set; }

        //private LinkedList<MinBorderNode> orderedNodes = new LinkedList<MinBorderNode>();
        //public LinkedList<MinBorderNode> OrderedNodes
        //{
        //    get { return orderedNodes; }
        //    set { orderedNodes = value; }
        //}

        public IEnumerable<MinBorderNode> Nodes { get; set; }

        private ICollection<MinBorderNode> unOrderedNodes = new LinkedList<MinBorderNode>();
        public ICollection<MinBorderNode> UnorderedNodes
        {
            get { return unOrderedNodes; }
            set { unOrderedNodes = value; }
        }
    }

    public class MinBorderInput : IInputData<MinBorderInstance>
    {
        public MinBorderInput(IEnumerable<IHas<INodeLogic>> nodes)
        {
            Nodes = nodes;
        }

        private IEnumerable<IHas<INodeLogic>> Nodes;

        public MinBorderInstance ToPositionData()
        {
            var position = new MinBorderInstance();

            position.UnorderedNodes = new TransformGraph<MinBorderNode>().Do(Nodes, transformNode).Nodes;

            position.Nodes = position.UnorderedNodes.ToList();

            position.UnchosenNeighbours = new int[position.Nodes.Count()];

            int counter = 0;
            foreach (var node in position.UnorderedNodes)
            {
                node.Name = counter.ToString();
                node.OrderId = counter;
                position.UnchosenNeighbours[counter] = node.Neighbours().Count;
                counter++;
            }

            position.NodeOrder = new int[counter];


            return position;
        }

        private MinBorderNode transformNode(IHas<INodeLogic> arg)
        {
            var newNode = new MinBorderNode(arg);

            newNode.UnSelectedNeighbours = arg.Logic.Neighbours.Count();

            return newNode;
        }
    }

    public class MinBorderInstanceEvaluation : IEvaluationMethod<MinBorderInstance>
    {
        public double Evaluate(MinBorderInstance position)
        {
            return -position.MaxBorderSize;
        }
    }

    public class CreateExampleInput
    {
        public static MinBorderInput Do()
        {
            //int dimX = 21;
            //int dimY = 12;
            int numberNodes = 123;
            int helixConectionSkip = 4;
            //var graph = GraphPackageOne.Tunnel<BasicNode>(dimX, dimY);
            //var graph = GraphPackageOne.ThreeCoreElementsGraph<BasicNode>(numberNodes);
            var graph = GraphPackageOne.Helix<BasicNode>(numberNodes, helixConectionSkip);

            var input = new MinBorderInput(graph.Nodes);

            return input;
        }
    }

    public class MinBorderNode : IHas<INodeLogic<MinBorderNode>>
    {
        public MinBorderNode(IHas<INodeLogic> originalNode)
        {
            OriginalNode = originalNode;
        }

        public string Name { get; set; }

        public int OrderId { get; set; }

        public IHas<INodeLogic> OriginalNode { get; set; }
        public bool IsInOrder { get; set; }

        public bool IsInBorder { get; set; }
        public int UnSelectedNeighbours { get; set; }

        private Guid id;

        public Guid GWId
        {
            get { return id; }
            set { id = value; }
        }

        private NodeLogic<MinBorderNode> logic = new NodeLogic<MinBorderNode>();
        public INodeLogic<MinBorderNode> Logic
        {
            get { return logic; }
        }

    }
    public class GreedyEvaluation : IEvaluationMethod<MinBorderAction>
    {
        public double Evaluate(MinBorderAction action)
        {
            var position = action.Position;
            if (!position.UnorderedNodes.Any())
                return -1;


            var node = action.NextNode;
            //int borderSizeChange =  node.UnSelectedNeighbours > 0 ? 1 : 0;
            //foreach (var neighbour in node.Neighbours())
            //{
            //    if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 1)
            //    {
            //        borderSizeChange--;
            //    }
            //    //if (!neighbour.IsInBorder)
            //    //{
            //    //    borderSizeChange++;
            //    //}
            //}
            if (!action.BorderChangeSet)
            {
                var borderSizeChange = node.Neighbours().Any(nb => !nb.IsInOrder) ? 1 : 0;
                foreach (var neighbour in node.Neighbours())
                {
                    if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 1)
                    {
                        borderSizeChange--;
                    }
                }
                action.BorderChange = borderSizeChange;
                action.BorderChangeSet = true;
            }


            return 100000 - action.BorderChange;
        }
    }
    public class GreedyStrategy : IStrategy<MinBorderInstance>
    {
        public bool Apply(MinBorderInstance position)
        {
            if (!position.UnorderedNodes.Any())
                return false;

            var currentBorderSize = position.CurrentBorderSize;

            var optimalNextNode = default(MinBorderNode);
            var optimalBorderChange = int.MaxValue;

            foreach (var node in position.UnorderedNodes)
            {
                int borderSizeChange = node.Neighbours().Any(nb => !nb.IsInOrder) ? 1 : 0;
                foreach (var neighbour in node.Neighbours())
                {
                    if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 1)
                    {
                        borderSizeChange--;
                    }
                    //if (!neighbour.IsInBorder)
                    //{
                    //    borderSizeChange++;
                    //}
                }
                if (borderSizeChange <= optimalBorderChange)
                {
                    optimalBorderChange = borderSizeChange;
                    optimalNextNode = node;
                }
            }

            //execute choosing optimalNextNode
            optimalNextNode.IsInOrder = true;
            optimalNextNode.IsInBorder = optimalNextNode.UnSelectedNeighbours > 0;
            foreach (var neighbour in optimalNextNode.Neighbours())
            {
                //neighbour.IsInBorder = true;
                neighbour.UnSelectedNeighbours--;
                if (neighbour.IsInBorder && neighbour.UnSelectedNeighbours == 0)
                {
                    neighbour.IsInBorder = false;
                }
            }
            position.CurrentBorderSize += optimalBorderChange;
            position.MaxBorderSize = Math.Max(position.MaxBorderSize, position.CurrentBorderSize);
            position.NodeOrder[optimalNextNode.OrderId] = position.NodesInOrder;
            position.NodesInOrder++;
            position.UnorderedNodes.Remove(optimalNextNode);

            return true;
        }

    }
}
