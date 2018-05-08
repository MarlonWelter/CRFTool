
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface ICRFGraphData
    {
        List<int[]> Sample { get; set; }
        string[] Characteristics { get; set; }
        int[] ReferenceLabeling { get; set; }
        int[] AssginedLabeling { get; set; }
    }
    public class CRFGraphData : ICRFGraphData, ICategoryGraph
    {
        public int NumberCategories { get; set; }
        public int NumberOfLabels { get; set; }
        public List<int[]> Sample { get; set; }
        public int[] ReferenceLabeling { get; set; }
        public int[] Viterbi { get; set; }
        public int[] AssginedLabeling { get; set; }
        public string[] Characteristics { get; set; }
        //wird momentan nicht wirklich benutzt, da auch der CRFNodeData eien observation enthält. Kann eventuell entfernt werden.
        //public int[] Observations { get; set; }
    }
    public class ProteinCRFGraphData : CRFGraphData
    {
        public int CoreResidues { get; set; }
    }
    public interface ICRFNodeData : ICoordinated, ICategoryNodeData
    {
        double[] Characteristics { get; set; }
        int UnchosenNeighboursTemp { get; set; }
        double[] Scores { get; set; }
        string Id { get; set; }
        bool IsChosen { get; set; }
        //int Ordinate { get; set; }
        int ReferenceLabel { get; set; }

        int TempAssign { get; set; }
        int TempCount { get; set; }
        int LastUpdate { get; set; }

        // Beobachtungen
        int Observation { get; set; }
    }
    public interface ICRFNodeDataBinary
    {
        int UnchosenNeighboursTemp { get; set; }
        double ScoreFalse { get; set; }
        double ScoreTrue { get; set; }
        string Id { get; set; }
        bool IsInQueue { get; set; }
        bool IsChosen { get; set; }
        int Ordinate { get; set; }
        int ReferenceLabel { get; set; }

        int TempAssign { get; set; }
        int TempCount { get; set; }
        int LastUpdate { get; set; }
    }
    public class CRFNodeData : ICRFNodeData, ICoordinated, ICRFNode3DInfo
    {
        public CRFNodeData()
        {

        }
        public CRFNodeData(string id)
        {
            Id = id;
        }
        public double[] Characteristics { get; set; }
        public int UnchosenNeighboursTemp { get; set; }
        public double[] Scores { get; set; }
        public string Id { get; set; }
        public bool IsInQueue { get; set; }
        public bool IsChosen { get; set; }
        public int Ordinate { get; set; }
        public int ReferenceLabel { get; set; }
        public int AssignedLabel { get; set; }
        public int TempAssign { get; set; }
        public int TempCount { get; set; }
        public int LastUpdate { get; set; }

        public int Observation { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int CommunityId { get; set; }
        public int Labelling { get; set; }

        public int Category { get; set; }
    }
    public class CRFNodeDataBinary : ICRFNodeDataBinary
    {
        public CRFNodeDataBinary(double scoreFalse, double scoreTrue)
        {
            ScoreTrue = scoreTrue;
            ScoreFalse = scoreFalse;
        }
        public int UnchosenNeighboursTemp { get; set; }

        public double ScoreTrue { get; set; }
        public double ScoreFalse { get; set; }
        public string Id { get; set; }
        public bool IsInQueue { get; set; }
        public bool IsChosen { get; set; }
        public int Ordinate { get; set; }
        public int ReferenceLabel { get; set; }

        public int TempAssign { get; set; }
        public int TempCount { get; set; }
        public int LastUpdate { get; set; }
    }

    public interface ICRFEdgeData
    {
        double Weight { get; set; }

        double[,] Scores { get; set; }
    }
    public interface ICRFEdgeDataBinary
    {
        double Weight { get; set; }

        int Ordinate { get; set; }

        double ScoreTT { get; set; }
        double ScoreTF { get; set; }
        double ScoreFT { get; set; }
        double ScoreFF { get; set; }
    }
    public class CRFEdgeData : ICRFEdgeData, IEdge3DInfo
    {

        public double Weight { get; set; }

        public double[,] Scores { get; set; }
    }
    public class CRFEdgeDataBinary : ICRFEdgeDataBinary
    {
        public CRFEdgeDataBinary(double scoreFF, double scoreFT, double scoreTF, double scoreTT)
        {
            ScoreTT = scoreTT;
            ScoreTF = scoreTF;
            ScoreFT = scoreFT;
            ScoreFF = scoreFF;
        }
        public double Weight { get; set; }

        public int Ordinate { get; set; }

        public double ScoreTT { get; set; }
        public double ScoreTF { get; set; }
        public double ScoreFT { get; set; }
        public double ScoreFF { get; set; }
    }

    public class CRFNode : IHas<INodeLogic<CRFNode, CRFEdge>>
    {
        public CRFNode()
        {

        }
        public CRFNode(string id)
        {
            Id = id;
        }
        public double Score(int state)
        {
            return Scores[state];
        }
        public int UnchosenNeighboursTemp { get; set; }
        public double[] Scores { get; set; }
        public string Id { get; set; }
        public bool IsInQueue { get; set; }
        public bool IsChosen { get; set; }
        public int Ordinate { get; set; }
        public int ReferenceLabel { get; set; }

        public int TempAssign { get; set; }
        public int TempCount { get; set; }
        public int LastUpdate { get; set; }

        private NodeLogic<CRFNode, CRFEdge> logic = new NodeLogic<CRFNode, CRFEdge>();
        public INodeLogic<CRFNode, CRFEdge> Logic
        {
            get { return logic; }
        }
        public Guid GWId { get; set; }
    }

    public class CRFEdge : IHas<IEdgeLogic<CRFNode, CRFEdge>>
    {
        public CRFEdge()
        {

        }
        public CRFEdge(CRFNode head, CRFNode foot)
        {
            this.Connect(head, foot);
        }
        public double TempScore()
        {
            return Scores[logic.Head.TempAssign, logic.Foot.TempAssign];
        }
        public double Score(CRFNode node1, int label1, CRFNode node2, int label2)
        {
            if (this.Head().Equals(node1))
            {
                return Scores[label1, label2];
            }
            else
                return Scores[label2, label1];
        }
        public double Score(AgO<int, int> state)
        {
            return Scores[state.Data1, state.Data2];
        }
        public double Score(int state1, int state2)
        {
            return Scores[state1, state2];
        }
        public double Weight { get; set; }

        public int Ordinate { get; set; }

        public double[,] Scores { get; set; }

        private EdgeLogic<CRFNode, CRFEdge> logic = new EdgeLogic<CRFNode, CRFEdge>();
        public IEdgeLogic<CRFNode, CRFEdge> Logic
        {
            get { return logic; }
        }
        public Guid GWId { get; set; }
    }

    public static class CRFX
    {
        public static double TempScore(this IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge)
        {
            var tempAssignHead = edge.Head.Data.TempAssign;
            if (tempAssignHead < 0)
                return 0.0;
            var tempAssignFoot = edge.Foot.Data.TempAssign;
            if (tempAssignFoot < 0)
                return 0.0;
            return edge.Data.Scores[tempAssignHead, tempAssignFoot];
        }
        public static double Score(this IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int node1, int label1, int node2, int label2)
        {
            if (edge.Head.GraphId.Equals(node1))
            {
                return edge.Data.Scores[label1, label2];
            }
            else
                return edge.Data.Scores[label2, label1];
        }
        public static double Score(this ICRFNodeData nodedata, int state)
        {
            return nodedata.Scores[state];
        }
        public static double Score(this ICRFNodeDataBinary nodedata, bool state)
        {
            return state ? nodedata.ScoreTrue : nodedata.ScoreFalse;
        }
        //public double Score(AgO<int, int> state)
        //{
        //    return Scores[state.Data1, state.Data2];
        //}
        public static double Score(this IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int state1, int state2)
        {
            return edge.Data.Scores[state1, state2];
        }
        public static double Score(this IGWEdge<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> edge, bool state1, bool state2)
        {
            if (state1)
            {
                return state2 ? edge.Data.ScoreTT : edge.Data.ScoreTF;
            }
            else
            {
                return state2 ? edge.Data.ScoreFT : edge.Data.ScoreFF;
            }
        }
    }
}
