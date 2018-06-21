using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeBase;
using System.Collections;

namespace CRFBase
{
    public class CRFLabelling : IComparable<CRFLabelling>
    {

        public CRFLabelling()
        {
        }

        public CRFLabelling(CRFLabelling parent)
        {
            AssignedLabels = parent.AssignedLabels.Clone() as int[];
        }
        //this array holds the labellings for all nodes. The index for a node is its GraphId
        public int[] AssignedLabels { get; set; }

        public double Score { get; set; }
        public double LastAddedScore { get; set; }
        public long BorderFingerPrint { get; set; }
        public IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> LastNodeAdded { get; set; }
        public void AddScore(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> chosenVertex, int state)
        {
            var addedScore = 0.0;

            AssignedLabels[chosenVertex.GraphId] = state;

            addedScore += chosenVertex.Data.Score(state);
            foreach (var edge in chosenVertex.Edges)
            {
                var headData = edge.Head.Data;
                var footData = edge.Foot.Data;
                if (!headData.IsChosen || !footData.IsChosen)
                    continue;

                addedScore += edge.Score(AssignedLabels[edge.Head.GraphId], AssignedLabels[edge.Foot.GraphId]);
            }
            Score += (addedScore);
            LastAddedScore = (addedScore);
            LastNodeAdded = chosenVertex;
        }

        public override string ToString()
        {
            string name = "";
            foreach (var vertex in AssignedLabels)
            {
                name += "Vertex " + vertex + ": " + AssignedLabels[vertex] + "\n";
            }
            return name;
        }

        public int CompareTo(CRFLabelling other)
        {
            if (Score > other.Score)
                return 1;
            else if (Score == other.Score)
                return 0;
            return -1;
        }
    }
}
