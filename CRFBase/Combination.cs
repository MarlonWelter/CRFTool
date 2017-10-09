using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeBase;
using System.Collections;

namespace CRFBase
{
    //public class Combination : IComparable<Combination>
    //{

    //    public Combination()
    //    {
    //    }

    //    public Combination(Combination parent)
    //    {
    //        Assignment = new BitArray(parent.Assignment);
    //    }
    //    public BitArray Assignment { get; set; }

    //    public double Score { get; set; }
    //    public double LastAddedScore { get; set; }
    //    public long BorderFingerPrint { get; set; }
    //    //public IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> LastNodeAdded { get; set; }
    //    public void AddScore(IGWNode<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> chosenVertex, bool state)
    //    {
    //        var addedScore = 0.0;

    //        Assignment[chosenVertex.Data.Ordinate] = state;

    //        addedScore += chosenVertex.Data.Score(state);
    //        //Console.WriteLine("Vertex " + chosenVertex.Id + ":  " + chosenVertex.Score(state));
    //        foreach (var edge in chosenVertex.Edges)
    //        {
    //            //var otherVertex = chosenVertex.Neighbour(edge);
    //            var headData = edge.Head.Data;
    //            var footData = edge.Foot.Data;
    //            if (!headData.IsChosen || !footData.IsChosen)
    //                continue;

    //            addedScore += edge.Score(Assignment[headData.Ordinate], Assignment[footData.Ordinate]);
    //        }
    //        Score += (addedScore);
    //        LastAddedScore = (addedScore);
    //        //LastNodeAdded = chosenVertex;
    //        //checkState();
    //    }

    //    public override string ToString()
    //    {
    //        string name = "";
    //        //foreach (var vertex in Assignment)
    //        //{
    //        //    name += "Vertex " + vertex + ": " + Assignment[vertex] ? 1 : 0 + "\n";
    //        //}
    //        return name;
    //    }

    //    public int CompareTo(Combination other)
    //    {
    //        if (Score > other.Score)
    //            return 1;
    //        else if (Score == other.Score)
    //            return 0;
    //        return -1;
    //    }
    //}

    //Combination for general case
    public class Combination : IComparable<Combination>
    {

        public Combination()
        {
        }

        public Combination(Combination parent)
        {
            Assignment = parent.Assignment.Clone() as int[];

        }
        public int[] Assignment { get; set; }

        public double Score { get; set; }
        public double LastAddedScore { get; set; }
        public long BorderFingerPrint { get; set; }
        public IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> LastNodeAdded { get; set; }
        public void AddScore(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> chosenVertex, int state)
        {
            var addedScore = 0.0;

            Assignment[chosenVertex.GraphId] = state;

            addedScore += chosenVertex.Data.Score(state);
            //Console.WriteLine("Vertex " + chosenVertex.Id + ":  " + chosenVertex.Score(state));
            foreach (var edge in chosenVertex.Edges)
            {
                //var otherVertex = chosenVertex.Neighbour(edge);
                var headData = edge.Head.Data;
                var footData = edge.Foot.Data;
                if (!headData.IsChosen || !footData.IsChosen)
                    continue;

                addedScore += edge.Score(Assignment[edge.Head.GraphId], Assignment[edge.Foot.GraphId]);
            }
            Score += (addedScore);
            LastAddedScore = (addedScore);
            LastNodeAdded = chosenVertex;
            //checkState();
        }

        public override string ToString()
        {
            string name = "";
            foreach (var vertex in Assignment)
            {
                name += "Vertex " + vertex + ": " + Assignment[vertex] + "\n";
            }
            return name;
        }

        public int CompareTo(Combination other)
        {
            if (Score > other.Score)
                return 1;
            else if (Score == other.Score)
                return 0;
            return -1;
        }
    }
}
