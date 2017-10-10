using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt.NeuralNetwork
{
    public class GWNeuralNetwork<InputData>
    {

        public GWNeuralNetwork(List<NeuralNetworkInputNode<InputData>> inputNodes, int[] innerLayers)
        {
            Graph = new GWGraph<NeuralNetworkNodeData, NeuralNetworkEdgeData, GWNeuralNetwork<InputData>>();
            Graph.GraphStyle = GraphStyle.Directed;

            InputNodes = inputNodes.ToList();

            // init layers
            Layers = new List<GWNode<NeuralNetworkNodeData, NeuralNetworkEdgeData, GWNeuralNetwork<InputData>>>[innerLayers.Length + 2];
            for (int layer = 0; layer < Layers.Length; layer++)
            {
                Layers[layer] = new List<GWNode<NeuralNetworkNodeData, NeuralNetworkEdgeData, GWNeuralNetwork<InputData>>>();
            }

            //inputLayer:
            for (int i = 0; i < InputNodes.Count; i++)
            {
                var node = Graph.CreateNode();
                node.Data = InputNodes[i];
                Layers[0].Add(node);
            }

            //create inner layers
            var random = new Random();
            int defConnects = 4;
            for (int layer = 1; layer <= innerLayers.Length; layer++)
            {
                Layers[layer] = new List<GWNode<NeuralNetworkNodeData, NeuralNetworkEdgeData, GWNeuralNetwork<InputData>>>();
                for (int i = 0; i < innerLayers[layer - 1]; i++)
                {
                    var node = Graph.CreateNode();
                    node.Data = new NeuralNetworkNodeData();

                    //connect with former layer
                    for (int k = 0; k < defConnects; k++)
                    {
                        var otherNode = Layers[layer - 1].RandomElement(random);
                        var edge = Graph.CreateEdge(otherNode, node);
                        edge.Data = new NeuralNetworkEdgeData();
                        edge.Data.Weight = random.NextDouble();
                    }

                    Layers[layer].Add(node);
                }
            }

            //create endingPoint
            var endnode = Graph.CreateNode();
            endnode.Data = new NeuralNetworkNodeData();
            for (int i = 0; i < Layers[Layers.Length - 2].Count; i++)
            {
                var edge = Graph.CreateEdge(Layers[Layers.Length - 2][i], endnode);
                edge.Data = new NeuralNetworkEdgeData();
                edge.Data.Weight = random.NextDouble();
            }
            Layers[Layers.Length - 1].Add(endnode);

        }
        public int Depth { get; set; }

        public List<NeuralNetworkInputNode<InputData>> InputNodes { get; set; }

        public List<GWNode<NeuralNetworkNodeData, NeuralNetworkEdgeData, GWNeuralNetwork<InputData>>>[] Layers { get; set; }

        public double Input(InputData data)
        {
            for (int i = 0; i < InputNodes.Count; i++)
            {
                InputNodes[i].Input(data);
                Layers[0][i].Data.Value = InputNodes[i].Value; //TODO: check if this is needed or same object
            }

            for (int layer = 1; layer < Layers.Length; layer++)
            {
                for (int i = 0; i < Layers[layer].Count; i++)
                {
                    var node = Layers[layer][i];
                    node.Data.Value = 0;
                    foreach (var edge in node.InEdges)
                    {
                        node.Data.Value += edge.Data.Weight * edge.Foot.Data.Value;
                    }
                }
            }

            return Layers[Layers.Length - 1][0].Data.Value;
        }
        public GWGraph<NeuralNetworkNodeData, NeuralNetworkEdgeData, GWNeuralNetwork<InputData>> Graph { get; set; }

        public void Store(string file)
        {
            using (var writer = new StreamWriter(file))
            {
                Store(writer);
            }
        }
        public void Store(StreamWriter writer)
        {
            //Graph.Store(writer);
            StoreX.StoreObj<IGWGraph>(Graph, writer);

            foreach (var layer in Layers)
            {
                writer.WriteLine("layer:");
                foreach (var node in layer)
                {
                    writer.WriteLine("node:" + node.GraphId);
                    foreach (var edgeIn in node.InEdges)
                    {
                        writer.WriteLine("  edge:" + edgeIn.Data.Weight + ":" + edgeIn.Foot.GraphId + ":" + edgeIn.Head.GraphId);
                    }
                }
            }
        }

        //public void Load(StreamReader reader)
        //{
        //    var line = "";
        //    var layer = false;
        //    var node = false;
        //    var nodeId = 0;
        //    while ((line = reader.ReadLine()) != null)
        //    {
        //        if (line.Equals("layer:"))
        //        {
        //            layer = true;
        //            break;
        //        }
        //        if (line.StartsWith("node:"))
        //        {

        //        }
        //    }
        //}
    }



    public class NeuralNetworkNodeData : GWStoreable
    {
        [GWStore]
        public double Value { get; set; }

        public void Store(StreamWriter writer)
        {
            StoreX.StoreObj<NeuralNetworkNodeData>(this, writer);
        }
    }

    public class NeuralNetworkEdgeData
    {
        public double Weight { get; set; }
    }
    public abstract class NeuralNetworkInputNode<InputData> : NeuralNetworkNodeData
    {
        public abstract void Input(InputData data);
    }
}
