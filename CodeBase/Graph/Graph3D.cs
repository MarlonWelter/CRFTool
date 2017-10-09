using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class Graph3D : GWGraph<Node3DInfo, Edge3DInfo, Graph3DInfo>
    {

    }
    public class Graph3DInfo
    {
    }

    public interface ICRFNode3DInfo : ICoordinated
    {
        int CommunityId { get; set; }
        int ReferenceLabel { get; set; }
        int Labelling { get; set; }
    }

    public class Node3DInfo : ICRFNode3DInfo
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int ClusterId { get; set; }
        public double QualityValue { get; set; }
        public int CommunityId { get; set; }
        public int ReferenceLabel { get; set; }
        public int Labelling { get; set; }
    }

    public class CRFNode3DInfo : Node3DInfo
    {
        public double[] StateScores { get; set; }
        public int CurrentState { get; set; }
        public int[] InStateCounts { get; set; }

        public int LastUpdate { get; set; }
    }

    public class CRFEdge3DInfo : Edge3DInfo
    {
        public double[,] StateScores { get; set; }
    }
    public interface IEdge3DInfo
    {
        double Weight { get; set; }
    }
    public class Edge3DInfo
    {
        public double Weight { get; set; }
    }

    public class ShowGraph3D : GWRequest<ShowGraph3D>
    {
        public ShowGraph3D(IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object> graph)
        {
            Graph = graph;
        }
        public IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object> Graph { get; set; }
    }
}
