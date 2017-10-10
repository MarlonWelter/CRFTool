using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using CRFGraph = CodeBase.IGWGraph<CodeBase.ICRFNodeData, CodeBase.ICRFEdgeData, CodeBase.ICRFGraphData>;

namespace CRFBase
{
    public class CRFAssignment //: IDictionary<CRFNode, int>
    {
        private Dictionary<CRFNode, int> dict = new Dictionary<CRFNode, int>();
        
        public void Assign(CRFNode node, int value)
        {
            if (dict.ContainsKey(node))
                dict[node] = value;
            else
                dict.Add(node, value);
        }
        public int ValueOf(CRFNode node)
        {
            return dict[node];
        }

        public CRFGraph Graph { get; set; }

        public double TakeScore()
        {
            var score = 1.0;
            foreach (var node in Graph.Nodes)
            {
                score += node.Data.Score(node.Data.TempAssign);
            }
            foreach (var edge in Graph.Edges)
            {
                score += edge.TempScore();
            }
            return score;
        }

    }
}
