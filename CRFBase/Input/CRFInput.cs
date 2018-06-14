
using CodeBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CRFBase
{
    public class CRFInput
    {
        public static IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> ParseFile(string file)
        {

            var graph = new GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>();

            using (var reader = new StreamReader(file))
            {
                if (reader.ReadLine() != "numlabels:")
                    throw new IOException("unbekanntes FileFormat - File sollte mit 'numlabels:' beginnen");

                int numberLabels = int.Parse(reader.ReadLine());
                if (reader.ReadLine() != "nodes:")
                    throw new IOException("unbekanntes FileFormat - File sollte 'nodes:' in Zeile 3 enthalten");

                string line = string.Empty;
                while ((line = reader.ReadLine()) != "edges:")
                {
                    string[] words = Regex.Split(line, "\\s");
                    var node = graph.CreateNode();
                    var scores = new List<double>();
                    for (int counter = 1; counter < numberLabels + 1; counter++)
                    {
                        try
                        {
                            scores.Add(double.Parse(words[counter], CultureInfo.InvariantCulture));
                        }
                        catch (FormatException ex)
                        {
                            if (words[counter].Equals("N_INF"))
                                scores.Add(double.NegativeInfinity);
                            else
                                throw ex;
                        }
                    }
                    graph.Nodes.Add(node);
                    node.Data.Scores = scores.ToArray();
                    node.Data.Id = words[0];

                }
                while ((line = reader.ReadLine()) != null)
                {
                    string[] words = Regex.Split(line, "\\s");
                    string name = words[0] + "_" + words[1];
                    string check = words[1] + "_" + words[0];

                    //Don't use the double occurence edges
                    if (words[0].CompareTo(words[1]) > 0)
                        continue;

                    var edge = new GWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>(graph.Nodes.First((n) => n.Data.Id.Equals(words[0])), graph.Nodes.First((n) => n.Data.Id.Equals(words[1])));

                    graph.Edges.Add(edge);

                    edge.Data.Scores = new double[numberLabels, numberLabels];
                    int word = 2;
                    for (int dounter = 0; dounter < numberLabels; dounter++)
                    {
                        for (int counter = 0; counter < numberLabels; counter++)
                        {
                            edge.Data.Scores[dounter, counter] = (double.Parse(words[word], CultureInfo.InvariantCulture));
                            word++;
                        }
                    }

                }
            }
            return graph;
        }

        public static IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> ParseFileFormatTwo(string file)
        {

            var graph = new GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>();

            using (var reader = new StreamReader(file))
            {

                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    else if (line.StartsWith("NODE"))
                    {
                        string[] words = Regex.Split(line, "\\s").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                        var node = graph.CreateNode();
                        var scores = new List<double>();
                        for (int counter = 0; counter < 2; counter++)
                        {
                            try
                            {
                                scores.Add(double.Parse(words[counter + 5], CultureInfo.InvariantCulture));
                            }
                            catch (FormatException ex)
                            {
                                if (words[counter + 5].Equals("N_INF"))
                                    scores.Add(double.NegativeInfinity);
                                else
                                    throw ex;
                            }
                        }
                        graph.Nodes.Add(node);
                        node.Data = new CRFNodeData();
                        node.Data.Scores = scores.ToArray();
                        node.Data.Id = words[1];
                        node.Data.ReferenceLabel = int.Parse(words[7]);

                    }
                    else if (line.StartsWith("NEIGH"))
                    {
                        string[] words = Regex.Split(line, "\\s").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                        var stringNode = words[1].Substring(0, words[1].Length - 1);

                        for (int nb = 2; nb < words.Length; nb++)
                        {
                            //Don't use the double occurence edges
                            if (stringNode.CompareTo(words[nb]) > 0)
                                continue;

                            var edge = new GWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>(graph.Nodes.First((n) => n.Data.Id.Equals(stringNode)), graph.Nodes.First((n) => n.Data.Id.Equals(words[nb])));
                            edge.Data = new CRFEdgeData();
                            graph.Edges.Add(edge);
                        }
                    }

                }
            }
            return graph;
        }

        //Davids files
        public static GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> ParseFileFormatThree(string file)
        {

            var graph = new GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>();

            var edgeMode = false;
            int CountAdd = 0;

            using (var reader = new StreamReader(file))
            {

                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Equals("NODES"))
                    {
                        reader.ReadLine();
                        edgeMode = false;

                    }
                    else if (line.Equals("EDGES"))
                    {
                        reader.ReadLine();
                        edgeMode = true;
                    }
                    else if (!edgeMode)
                    {
                        string[] words = Regex.Split(line, ";");
                        var node = graph.CreateNode();
                        var scoreI = int.Parse(words[1]);
                        var scoreN = int.Parse(words[2]);
                        var scores = new List<double>();
                        scores.AddRange(scoreN + CountAdd, scoreI + CountAdd);
                        graph.Nodes.Add(node);
                        node.Data.Scores = scores.ToArray();
                        node.Data.Id = words[0];
                        node.Data.ReferenceLabel = (words[3].Equals("N")) ? 0 : 1;
                    }
                    else
                    {
                        string[] words = Regex.Split(line, ";");
                        var headId = words[0];
                        var footId = words[1];
                        var head = graph.Nodes.First(n => n.Data.Id.Equals(headId));
                        var foot = graph.Nodes.First(n => n.Data.Id.Equals(footId));

                        var edge = new GWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>(head, foot);
                        graph.Edges.Add(edge);
                    }
                }
            }
            return graph;
        }

        public static GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> ParseFile_CRF_Class(string file)
        {

            var graph = new GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>();
            graph.Data = new SGLGraphData();

            using (var reader = new StreamReader(file))
            {
                //if (reader.ReadLine() != "reflabels:")
                //    throw new IOException("unbekanntes FileFormat - File sollte mit 'numlabels:' beginnen");
                //int refLabels = int.Parse(reader.ReadLine());
                //if (reader.ReadLine() != "observations:")
                //    throw new IOException("unbekanntes FileFormat - File sollte mit 'numlabels:' beginnen");
                //int observations = int.Parse(reader.ReadLine());

                if (reader.ReadLine() != "nodes:")
                    throw new IOException("unbekanntes FileFormat - File sollte 'nodes:' in Zeile 3 enthalten");

                string line = string.Empty;
                while ((line = reader.ReadLine()) != "edges:" && line.NotNullOrEmpty())
                {
                    string[] words = Regex.Split(line, "\\s");
                    var node = graph.CreateNode();
                    var scores = new List<double>();
                    node.Data = new SGLNodeData();
                    node.Data.ReferenceLabel = int.Parse(words[1]);
                    node.Data.Observation = int.Parse(words[2]);

                    if (words.Length > 3)
                        node.Data.AssignedLabel = int.Parse(words[3]);

                    node.Data.Id = words[0];

                }
                while ((line = reader.ReadLine()).NotNullOrEmpty())
                {
                    string[] words = Regex.Split(line, "\\s");
                    string name = words[0] + "_" + words[1];
                    string check = words[1] + "_" + words[0];

                    var edge = graph.CreateEdge(graph.Nodes.First((n) => n.Data.Id.Equals(words[0])), graph.Nodes.First((n) => n.Data.Id.Equals(words[1])));

                    edge.Data = new SGLEdgeData();
                }
            }
            return graph;
        }
    }
}
