
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public static class GrowPatchLogic
    {
        public static IEnumerable<NodeType> GrowPatch<NodeType, EdgeType>(this NodeType node, int size)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IMarkable, ITempSCore
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var patch = new LinkedList<NodeType>();

            patch.AddLast(node);

            //Log.Post("Growing Patch For Node " + node.MitId);


            node.Mark();
            var neighbours = node.Neighbours();
            neighbours.Each(n => n.Temp++);
            LinkedList<NodeType> border = new LinkedList<NodeType>();

            border.AddRange(neighbours);

            while (patch.Count < size)
            {
                if (border.Count == 0)
                    break;
                var dict = new Dictionary<NodeType, double>();
                foreach (var item in border)
                {
                    // dict.Add(item, (double)item.Neighbours().Where(n => n.IsMarked).Count());
                    dict.Add(item, item.Temp);
                }
                var nextNode = ProbabilityChooser.ChooseByDistribution(dict);
                // var nextNode = border[rand.Next(border.Count)];
                nextNode.Mark();
                patch.Add(nextNode);
                border.Remove(nextNode);
                neighbours = nextNode.Neighbours();
                neighbours.Each(n => n.Temp++);
                border.AddRange(neighbours.Where(n => !n.IsMarked && !border.Contains(n)));
            }

            //Log.Post("Growed Patch For Node " + node.MitId + " with size " + patch.Count);
            patch.Each(n => { n.Unmark(); n.Temp = 0; });
            border.Each(n => n.Temp = 0);
            return patch;
        }
        public static IEnumerable<NodeType> GrowPatchSimple<NodeType, EdgeType>(this NodeType node, int size)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IMarkable
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var patch = new LinkedList<NodeType>();

            patch.AddLast(node);

            //Log.Post("Growing Patch For Node " + node.MitId);


            node.Mark();
            var neighbours = node.Neighbours();
            LinkedList<NodeType> border = new LinkedList<NodeType>();

            border.AddRange(neighbours);

            while (patch.Count < size)
            {
                if (border.Count == 0)
                    break;

                var nextNode = border.RandomElement(Random);
                // var nextNode = border[rand.Next(border.Count)];
                nextNode.Mark();
                patch.Add(nextNode);
                border.Remove(nextNode);
                neighbours = nextNode.Neighbours();
                //  neighbours.Each(n => if(!n.IsMarked && !border.Contains(n))
                border.AddRange(neighbours.Where(n => !n.IsMarked && !border.Contains(n)));
            }

            //Log.Post("Growed Patch For Node " + node.MitId + " with size " + patch.Count);
            patch.Each(n => n.Unmark());
            return patch;
        }
        public static IEnumerable<NodeType> GrowPatchSimpleNoMark<NodeType, EdgeType>(this NodeType node, int size)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var patch = new LinkedList<NodeType>();

            patch.AddLast(node);

            //Log.Post("Growing Patch For Node " + node.MitId);


            var neighbours = node.Neighbours();
            LinkedList<NodeType> border = new LinkedList<NodeType>();

            border.AddRange(neighbours);

            while (patch.Count < size)
            {
                if (border.Count == 0)
                    break;

                var nextNode = border.RandomElement(Random);
                // var nextNode = border[rand.Next(border.Count)];
                
                patch.Add(nextNode);
                border.Remove(nextNode);
                neighbours = nextNode.Neighbours();
                //  neighbours.Each(n => if(!n.IsMarked && !border.Contains(n))
                border.AddRange(neighbours.Where(n => !patch.Contains(n) && !border.Contains(n)));
            }

            //Log.Post("Growed Patch For Node " + node.MitId + " with size " + patch.Count);
            return patch;
        }

        public static Random Random = new Random();
        public static IEnumerable<NodeType> GrowPatchSimple<NodeType, EdgeType>(this NodeType node, int size, Func<NodeType, bool> allowedNodes)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IMarkable
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var patch = new LinkedList<NodeType>();

            patch.AddLast(node);

            //Log.Post("Growing Patch For Node " + node.MitId);


            node.Mark();
            var neighbours = node.Neighbours().Where(nb => allowedNodes(nb));
            LinkedList<NodeType> border = new LinkedList<NodeType>();

            border.AddRange(neighbours);

            while (patch.Count < size)
            {
                if (border.Count == 0)
                    break;

                var nextNode = border.RandomElement(Random);
                // var nextNode = border[rand.Next(border.Count)];
                nextNode.Mark();
                patch.Add(nextNode);
                border.Remove(nextNode);
                neighbours = nextNode.Neighbours().Where(nb => allowedNodes(nb));
                //  neighbours.Each(n => if(!n.IsMarked && !border.Contains(n))
                border.AddRange(neighbours.Where(n => !n.IsMarked && !border.Contains(n)));
            }

            //Log.Post("Growed Patch For Node " + node.MitId + " with size " + patch.Count);
            patch.Each(n => n.Unmark());
            return patch;
        }
        public static IEnumerable<NodeType> GrowPatchWeighted<NodeType, EdgeType>(this NodeType node, int size, Func<NodeType, double> nodeWeight)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IMarkable
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var patch = new LinkedList<NodeType>();

            patch.AddLast(node);

            //Log.Post("Growing Patch For Node " + node.MitId);


            node.Mark();
            var neighbours = node.Neighbours();
            LinkedList<NodeType> border = new LinkedList<NodeType>();

            border.AddRange(neighbours);

            while (patch.Count < size)
            {
                if (border.Count == 0)
                    break;

                var nextNode = border.WeightedTake(nodeWeight, Random);
                if (nextNode == null)
                    break;
                // var nextNode = border[rand.Next(border.Count)];
                nextNode.Mark();
                patch.Add(nextNode);
                border.Remove(nextNode);
                neighbours = nextNode.Neighbours();
                //  neighbours.Each(n => if(!n.IsMarked && !border.Contains(n))
                border.AddRange(neighbours.Where(n => !n.IsMarked && !border.Contains(n)));
            }

            //Log.Post("Growed Patch For Node " + node.MitId + " with size " + patch.Count);
            patch.Each(n => n.Unmark());
            return patch;
        }

    }
}
