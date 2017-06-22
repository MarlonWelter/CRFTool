using CodeBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public static class GraphCSVParser
    {
        public static int CreateEdgesToNbs = 5;
        public static GWGraph<Node3DInfo, Edge3DInfo, Graph3DInfo> Do(string file)
        {
            var graph = new GWGraph<Node3DInfo, Edge3DInfo, Graph3DInfo>();
            var nodeDict = new Dictionary<string, GWNode<Node3DInfo, Edge3DInfo, Graph3DInfo>>();
            var maxNodes = 1000;
            using (var reader = new StreamReader(file))
            {
                var tempLines = new LinkedList<AgO<string, double>>();
                var currentNode = "";
                var rdm = new Random();
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(',');
                    var nodeName = words[0].Substring(1, words[0].Length - 2);
                    if (!nodeName.EndsWith(".cpp"))
                        continue;
                    var othernodeName = words[1].Substring(1, words[1].Length - 2);
                    if (!othernodeName.EndsWith(".cpp"))
                        continue;

                    if (currentNode.Equals(words[0]))
                    {
                        tempLines.Add(new AgO<string, double>(line, double.Parse(words[2].Substring(1, words[2].Length - 2), CultureInfo.InvariantCulture)));
                        continue;
                    }
                    else
                    {
                        currentNode = words[0];
                    }
                    foreach (var tempLine in tempLines.OrderByDescending(tl => tl.Data2).Take(CreateEdgesToNbs))
                    {
                        words = tempLine.Data1.Split(',');
                        var weight = double.Parse(words[2].Substring(1, words[2].Length - 2), CultureInfo.InvariantCulture);

                        var node = default(GWNode<Node3DInfo, Edge3DInfo, Graph3DInfo>);
                        if (!nodeDict.ContainsKey(nodeName))
                        {
                            if (nodeDict.Count < maxNodes)
                            {
                                node = graph.CreateNode();
                                node.Data = new Node3DInfo();
                                node.Data.X = rdm.NextDouble();
                                node.Data.Y = rdm.NextDouble();
                                node.Data.Z = rdm.NextDouble();
                                nodeDict.Add(nodeName, node);
                            }
                            else
                                continue;
                        }
                        else
                        { node = nodeDict[nodeName]; }

                        var othernode = default(GWNode<Node3DInfo, Edge3DInfo, Graph3DInfo>);
                        if (!nodeDict.ContainsKey(othernodeName))
                        {
                            if (nodeDict.Count < maxNodes)
                            {
                                othernode = graph.CreateNode();
                                othernode.Data = new Node3DInfo();
                                othernode.Data.X = rdm.NextDouble();
                                othernode.Data.Y = rdm.NextDouble();
                                othernode.Data.Z = rdm.NextDouble();
                                nodeDict.Add(othernodeName, othernode);
                            }
                            else
                                continue;
                        }
                        else
                        { othernode = nodeDict[othernodeName]; }

                        if (!node.Neighbours.Contains(othernode))
                        {
                            if (weight >= 3.0)
                            {
                                var newEdge = graph.CreateEdge(node, othernode);
                                newEdge.Data = new Edge3DInfo();
                                newEdge.Data.Weight = weight;
                            }
                        }

                    }
                    tempLines.Clear();
                    tempLines.Add(new AgO<string, double>(line, double.Parse(words[2].Substring(1, words[2].Length - 2), CultureInfo.InvariantCulture)));
                }
            }
            return graph;
        }

        public static GWGraph<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo> DoCRF(string file)
        {
            var graph = new GWGraph<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>();
            var nodeDict = new Dictionary<string, GWNode<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>>();
            var maxNodes = 1000;
            using (var reader = new StreamReader(file))
            {
                var tempLines = new LinkedList<AgO<string, double>>();
                var currentNode = "";
                var rdm = new Random();
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(',');
                    var nodeName = words[0].Substring(1, words[0].Length - 2);
                    if (!nodeName.EndsWith(".cpp"))
                        continue;
                    var othernodeName = words[1].Substring(1, words[1].Length - 2);
                    if (!othernodeName.EndsWith(".cpp"))
                        continue;

                    if (currentNode.Equals(words[0]))
                    {
                        tempLines.Add(new AgO<string, double>(line, double.Parse(words[2].Substring(1, words[2].Length - 2), CultureInfo.InvariantCulture)));
                        continue;
                    }
                    else
                    {
                        currentNode = words[0];
                    }
                    foreach (var tempLine in tempLines.OrderByDescending(tl => tl.Data2).Take(CreateEdgesToNbs))
                    {
                        words = tempLine.Data1.Split(',');
                        var weight = double.Parse(words[2].Substring(1, words[2].Length - 2), CultureInfo.InvariantCulture);

                        var node = default(GWNode<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>);
                        if (!nodeDict.ContainsKey(nodeName))
                        {
                            if (nodeDict.Count < maxNodes)
                            {
                                node = graph.CreateNode();
                                node.Data = new CRFNode3DInfo();
                                node.Data.X = rdm.NextDouble();
                                node.Data.Y = rdm.NextDouble();
                                node.Data.Z = rdm.NextDouble();
                                nodeDict.Add(nodeName, node);
                            }
                            else
                                continue;
                        }
                        else
                        { node = nodeDict[nodeName]; }

                        var othernode = default(GWNode<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>);
                        if (!nodeDict.ContainsKey(othernodeName))
                        {
                            if (nodeDict.Count < maxNodes)
                            {
                                othernode = graph.CreateNode();
                                othernode.Data = new CRFNode3DInfo();
                                othernode.Data.X = rdm.NextDouble();
                                othernode.Data.Y = rdm.NextDouble();
                                othernode.Data.Z = rdm.NextDouble();
                                nodeDict.Add(othernodeName, othernode);
                            }
                            else
                                continue;
                        }
                        else
                        { othernode = nodeDict[othernodeName]; }

                        if (!node.Neighbours.Contains(othernode))
                        {
                            if (weight >= 3.0)
                            {
                                var newEdge = graph.CreateEdge(node, othernode);
                                newEdge.Data = new CRFEdge3DInfo();
                                newEdge.Data.Weight = weight;
                            }
                        }

                    }
                    tempLines.Clear();
                    tempLines.Add(new AgO<string, double>(line, double.Parse(words[2].Substring(1, words[2].Length - 2), CultureInfo.InvariantCulture)));
                }
            }
            return graph;
        }
        public static GWGraph<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo> DoCRF(string file, int timepoint, double minWeight)
        {
            var graph = new GWGraph<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>();
            var nodeDict = new Dictionary<string, GWNode<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>>();
            using (var reader = new StreamReader(file))
            {
                var rdm = new Random();
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(';');

                    var time = double.Parse(words[0], CultureInfo.InvariantCulture);
                    if (time != timepoint)
                        continue;

                    var nodeName = words[1].Substring(1, words[1].Length - 2);
                    var label = words[2].Substring(1, words[2].Length - 2);
                    var labelValue = double.Parse(words[3], CultureInfo.InvariantCulture);

                    var node = default(GWNode<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>);
                    if (!nodeDict.ContainsKey(nodeName))
                    {
                        node = graph.CreateNode();
                        node.Data = new CRFNode3DInfo();
                        node.Data.X = rdm.NextDouble();
                        node.Data.Y = rdm.NextDouble();
                        node.Data.Z = rdm.NextDouble();
                        nodeDict.Add(nodeName, node);
                    }
                    else
                    { node = nodeDict[nodeName]; }

                    var othernodesNames = words[4].Substring(1, words[4].Length - 2).Split(',');

                    if (othernodesNames.Length > 1)
                    {
                        for (int k = 0; k < othernodesNames.Length; k += 2)
                        {
                            var otherNodeName = othernodesNames[k];
                            var weight = double.Parse(othernodesNames[k + 1], CultureInfo.InvariantCulture);

                            var othernode = default(GWNode<CRFNode3DInfo, CRFEdge3DInfo, Graph3DInfo>);
                            if (!nodeDict.ContainsKey(otherNodeName))
                            {
                                othernode = graph.CreateNode();
                                othernode.Data = new CRFNode3DInfo();
                                othernode.Data.X = rdm.NextDouble();
                                othernode.Data.Y = rdm.NextDouble();
                                othernode.Data.Z = rdm.NextDouble();
                                nodeDict.Add(otherNodeName, othernode);
                            }
                            else
                            { othernode = nodeDict[otherNodeName]; }

                            if (!node.Neighbours.Contains(othernode))
                            {
                                if (weight >= minWeight)
                                {
                                    var newEdge = graph.CreateEdge(node, othernode);
                                    newEdge.Data = new CRFEdge3DInfo();
                                    newEdge.Data.Weight = weight;
                                }
                            }
                        }
                    }

                }
            }
            return graph;
        }

        public static GWGraph<SSPND, SSPED, SSPGD> Do(string nodesFile, string edgesFile)
        {
            var graph = new GWGraph<SSPND, SSPED, SSPGD>();
            var nodeDict = new Dictionary<string, GWNode<SSPND, SSPED, SSPGD>>();
            using (var reader = new StreamReader(nodesFile))
            {
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(',');
                    var nodeName = words[0];
                    var category = words[1];
                    var value = double.Parse(words[2], CultureInfo.InvariantCulture);
                    var newNode = graph.CreateNode();
                    nodeDict.Add(nodeName, newNode);
                    newNode.Data = new SSPND(nodeName, category, value);
                }
            }
            using (var reader = new StreamReader(edgesFile))
            {
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(',');
                    var nodename1 = words[0];
                    var nodename2 = words[1];
                    var weight = double.Parse(words[2], CultureInfo.InvariantCulture);
                    var type = words[3];

                    var node1 = nodeDict[nodename1];
                    var node2 = nodeDict[nodename2];

                    var edge = graph.CreateEdge(node1, node2);

                    edge.Data = new SSPED(weight, type);
                }
            }

            return graph;
        }

        public static GWGraph<SSPND, SSPED, SSPGD> CGR(string graphFile)
        {
            var graph = new GWGraph<SSPND, SSPED, SSPGD>();
            var nodeDict = new Dictionary<string, GWNode<SSPND, SSPED, SSPGD>>();

            using (var reader = new StreamReader(graphFile))
            {
                var line = reader.ReadLine();
                if (!line.Equals("#graph#"))
                    throw new ArgumentException("File has to start with #graph#");
                {
                    var name = reader.ReadLine();
                    graph.Name = name;
                }

                while ((line = reader.ReadLine()) != "#nodes#")
                    continue;

                line = reader.ReadLine(); //Id, Category, Value
                while (!((line = reader.ReadLine()).Equals("#edges#")))
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        var words = line.Split(',');
                        var nodeName = words[0];
                        var category = words[1];
                        var value = double.Parse(words[2], CultureInfo.InvariantCulture);
                        var newNode = graph.CreateNode();
                        nodeDict.Add(nodeName, newNode);
                        newNode.Data = new SSPND(nodeName, category, value);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                if (line != null)
                {
                    line = reader.ReadLine(); //Source,Target,Weight,Type
                    while ((line = reader.ReadLine()) != null && !line.NullOrEmpty() && !line.Equals("#statistics#"))
                    {
                        line = line.Replace("\"", "");
                        var words = line.Split(',');
                        var nodename1 = words[0];
                        var nodename2 = words[1];
                        var weight = double.Parse(words[2], CultureInfo.InvariantCulture);
                        var type = words[3];

                        var node1 = nodeDict[nodename1];
                        var node2 = nodeDict[nodename2];

                        var edge = graph.CreateEdge(node1, node2);

                        edge.Data = new SSPED(weight, type);
                    }
                }

                graph.Data = new SSPGD();
                graph.Data.Categories = graph.Nodes.Select(n => n.Data.Category).Distinct().ToList();

                CreateCategoryGraph.Do(graph);
            }

            return graph;
        }
    }
}
