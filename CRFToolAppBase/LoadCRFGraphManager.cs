using CodeBase;
using CRFBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRFToolAppBase
{
    class LoadCRFGraphManager : GWManager<LoadCRFGraph>
    {
        protected override void OnRequest(LoadCRFGraph request)
        {
            var graph = default(IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>);

            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Graph Files|*.crf";
                openFileDialog1.Title = "Select a CRF graph File";

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        graph = JSONX.LoadFromJSON<GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>>(openFileDialog1.FileName);
                    }
                    catch(Exception e)
                    { // try simple format
                        graph = CRFInput.ParseFile_CRF_Class(openFileDialog1.FileName);
                    }
                }
            }
            if (graph != null)
            {
                int nodeCounter = 0;
                foreach (var node in graph.Nodes)
                {
                    node.Data.Ordinate = nodeCounter;
                    nodeCounter++;
                }
            }
            request.Graph = graph;
        }
    }

    public class LoadCRFGraph : GWRequest<LoadCRFGraph>
    {
        public IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> Graph { get; set; }
    }
}
