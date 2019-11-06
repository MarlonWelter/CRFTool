using CodeBase;
using System;
using System.Linq;
using System.Threading;
using EDGraph = CodeBase.GWGraph<CRFBase.EDND, CRFBase.EDED, CRFBase.EDGD>;

namespace CRFBase
{
    public static class EmbeddingControlX
    {
        public static void TogglePause(this I3DEmbeddingControl control)
        {
            control.IsInPause = !control.IsInPause;
        }
    }
    public interface I3DEmbeddingControl
    {
        double CategoryFactor { get; set; }
        EDGraph Graph { get; set; }
        double Temperature { get; set; }
        bool UseCategories { get; set; }
        bool UseEdgeWeights { get; set; }
        bool CenterBalancePoint { get; set; }
        bool IsInPause { get; set; }
        void Start();
        void Stop();
    }

    public static class EmbeddingX
    {
        public static I3DEmbeddingControl CreateDefaultEmbeddingControl()
        {
            return new DefaultEmbeddingAlgorithm();
        }
    }

    public class EDED
    {
        public EDED(double weight)
        {
            Weight = weight;
        }

        public bool isIntraCommunity { get; set; }
        public double Weight { get; set; }
    }

    public class EDGD
    {
    }

    public class EDND
    {
        public EDND(double weight, string category, ICoordinated object3D, int label)
        {
            Weight = weight;
            Category = category;
            Object3D = object3D;
            Label = label;
        }

        public string Category { get; set; }
        public ICoordinated Object3D { get; set; }
        public double Weight { get; set; }
        public int Label { get; set; }
    }
    internal class DefaultEmbeddingAlgorithm : GeneralEmbeddingAlgorithm
    {
        public const double DefaultCategoryFactor = 0.2;
        public const double DefaultTemperature = 0.1;
        public const bool DefaultUseCategories = true;
        public const bool DefaultUseEdgeWeights = false;
        public const bool DefaultCenterBalancePoint = true;

        public DefaultEmbeddingAlgorithm() : base(DefaultTemperature, DefaultUseCategories, DefaultCategoryFactor, DefaultUseEdgeWeights, DefaultCenterBalancePoint)
        {
        }
    }

    internal class GeneralEmbeddingAlgorithm : I3DEmbeddingControl
    {
        public GeneralEmbeddingAlgorithm(double temperature, bool useCats, double catFactor, bool useEdgeWeights, bool centerBalancePoint)
        {
            Temperature = temperature;
            UseCategories = useCats;
            CategoryFactor = catFactor;
            UseEdgeWeights = useEdgeWeights;
            CenterBalancePoint = centerBalancePoint;
        }

        public double CategoryFactor { get; set; }
        public EDGraph Graph { get; set; }
        public double Temperature { get; set; }
        public bool UseCategories { get; set; }
        public bool UseEdgeWeights { get; set; }
        internal bool StopSimulation { get; set; }
        public bool CenterBalancePoint { get; set; }
        public bool IsInPause { get; set; }
        public int Iteration { get; set; }

        Random rdm = new Random();

        public void Do(Func<GWNode<EDND, EDED, EDGD>, GWNode<EDND, EDED, EDGD>, double> forceConnected, Func<GWNode<EDND, EDED, EDGD>, GWNode<EDND, EDED, EDGD>, double> forceNonConnected)
        {
            StopSimulation = false;
            Iteration = 0;
            double nodeCount = Graph.Nodes.Count;
            var startTime = DateTime.Now;

            //prepare
            var center = new Force();
            foreach (var node in Graph.Nodes)
            {
                node.Data.Object3D.X = rdm.Next(100);
                node.Data.Object3D.Y = rdm.Next(100);
                node.Data.Object3D.Z = rdm.Next(100);
                center.Add(node.Data.Object3D);
            }
            //center
            center.Multiply(-1.0 / nodeCount);
            if (CenterBalancePoint)
            {
                foreach (var node in Graph.Nodes)
                {
                    node.Data.Object3D.Move(center);
                }
            }


            var nodes = Graph.Nodes.ToList().RemoveWhere(n => !n.Neighbours.Any()).ToList();
            if (nodes.NotNullOrEmpty())
                innerRunEmbed(forceConnected, forceNonConnected);
        }

        private void innerRunEmbed(Func<GWNode<EDND, EDED, EDGD>, GWNode<EDND, EDED, EDGD>, double> forceConnected, Func<GWNode<EDND, EDED, EDGD>, GWNode<EDND, EDED, EDGD>, double> forceNonConnected)
        {
            var nodes = Graph.Nodes.ToArray();
            while (!StopSimulation && !IsInPause)
            {
                var rdmNode = nodes.RandomTake(1, rdm).First();

                var force = new Force();

                var doTunneling = false;// rdm.NextDouble() < 0.01;

                foreach (var nb in Graph.Nodes)
                {
                    if (nb.GraphId == rdmNode.GraphId)
                        continue;

                    CategoryFactor = 0.025;
                    if (rdmNode.Neighbours.Contains(nb))
                    {
                        
                        var strength = forceConnected(rdmNode, nb);
                        if (UseCategories && nb.Data.Category != rdmNode.Data.Category)
                            strength *= CategoryFactor;
                        var direction = nb.Data.Object3D.Minus(rdmNode.Data.Object3D);
                        direction.Normalize();
                        direction.Multiply(strength);
                        force.Add(direction);
                    }
                    if (!doTunneling)
                    {
                        var strength = forceNonConnected(nb, rdmNode);
                        if (UseCategories && nb.Data.Category == rdmNode.Data.Category)
                            strength *= CategoryFactor;
                        var direction = nb.Data.Object3D.Minus(rdmNode.Data.Object3D);
                        direction.Normalize();
                        direction.Multiply(strength);
                        force.Add(direction);
                    }

                    //add force to 2D - plane
                    //  force.Add(new Vector(0, 0, -rdmNode.Data.Object3D.Z / 500.0));

                    Temperature = 1;
                    force.Multiply(Temperature);
                    rdmNode.Data.Object3D.Move(force);

                    if (CenterBalancePoint)
                    {
                        force.Multiply(-1.0 / nodes.Length);
                        foreach (var node in Graph.Nodes)
                        {
                            node.Data.Object3D.Move(force);
                        }
                    }

                    Iteration++;
                }
            }
        }

        public void EmbedStartAsync()
        {
            Temperature = 0.1;
            var thread = new Thread(() => Do((n, nb) => ForceNbs(n.Data.Object3D, nb.Data.Object3D), (n, nb) => ForceRepell(n.Data.Object3D, nb.Data.Object3D)));
            thread.Start();
        }
        public double ForceForeign(ICoordinated from, ICoordinated to)
        {
            var dist = from.Distance(to);

            var optimalMinDistance = 100;

            return -Math.Max(0, (optimalMinDistance - dist) / optimalMinDistance);
        }
        public double ForceRepell(ICoordinated from, ICoordinated to)
        {
            var dist = from.Distance(to);

            var distOne = 20;

            //  return 20 * Math.Min(0, -distOne + dist * 0.25);
            //   return -distOne / (3 * dist);
            //  return 12 * -Math.Min(100, Math.Exp(-dist * 0.0));
            return -1000 * (1 / (Math.Pow(dist, 1.1))); 
        }
        public double ForceNbs(ICoordinated from, ICoordinated to)
        {
            var dist = from.Distance(to);

            //if (dist < 50)
            //    return 25;
            //if( dist < 200)
            //{
            //    return  (dist-50) * 0.125;
            //}
            //else

            //return (dist-200) * 0.5;
            return  Math.Pow(Math.Log(dist), 2);
        }

        public void Start()
        {
            if (CheckConditions())
                EmbedStartAsync();
        }

        public void Stop()
        {
            StopSimulation = true;
        }

        private bool CheckConditions()
        {
            if (Graph == null)
                return false;

            return true;
        }

        public void Pause()
        {
            IsInPause = true;
        }

        public void UnPause()
        {
            if (CheckConditions())
            {
                var thread = new Thread(() => innerRunEmbed((n, nb) => ForceNbs(n.Data.Object3D, nb.Data.Object3D), (n, nb) => ForceRepell(n.Data.Object3D, nb.Data.Object3D)));
                thread.Start();
            }
        }
    }
}