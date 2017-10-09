
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CodeBase;

//namespace CRFBase
//{
//    public class Graphs
//    {
//        public static CRFGraph ExampleGraphOne()
//        {
//            var graph = new CRFGraph();
//            var node1 = new CRFNode("0");
//            node1.Scores = new double[2] { 0.3, 0.7 };
//            graph.Nodes.Add(node1);


//            Normalize(graph, node1);


//            return graph;
//        }

//        public static CRFGraph ExampleGraphTwo()
//        {
//            var graph = new CRFGraph();
//            var node1 = new CRFNode("0");
//            node1.Scores = new double[2] { 0.3, 0.7 };


//            var node2 = new CRFNode("1");
//            node2.Scores = new double[2] { 0.6, 0.4 };

//            var edge1 = new CRFEdge();
//            edge1.Connect(node1, node2);
//            edge1.Scores = new double[2, 2] { { 1.0, 1.0 }, { 1.0, 1.0 } };

//            graph.Nodes.AddRange(node1, node2);
//            graph.Edges.AddRange(edge1);


//            Normalize(graph, node1);

//            return graph;
//        }

//        public static CRFGraph ExampleGraphThree()
//        {
//            var graph = new CRFGraph();
//            var node1 = new CRFNode("0");
//            node1.Scores = new double[2] { 0.3, 0.7 };


//            var node2 = new CRFNode("1");
//            node2.Scores = new double[2] { 0.6, 0.4 };

//            var edge1 = new CRFEdge();
//            edge1.Connect(node1, node2);
//            edge1.Scores = new double[2, 2] { { 1.0, 2.0 }, { 3.0, 4.0 } };

//            graph.Nodes.AddRange(node1, node2);
//            graph.Edges.AddRange(edge1);
//            Normalize(graph, node1);

//            return graph;
//        }

//        private static void Normalize(CRFGraph graph, CRFNode node1)
//        {
//            //normalize
//            var score = 0.0;
//            //foreach (var node in graph.Nodes)
//            {
//                GoThroughGraph(new LinkedList<CRFNode>(graph.Nodes).First, new Dictionary<CRFNode, int>(), (dict) =>
//                {
//                    var localscore = 0.0;
//                    foreach (var n in graph.Nodes)
//                    {
//                        n.TempAssign = dict[n];
//                        localscore += Math.Log(n.Score(n.TempAssign));
//                    }
//                    foreach (var edge in graph.Edges)
//                    {
//                        localscore += Math.Log(edge.TempScore());
//                    }
//                    score += Math.Exp(localscore);
//                });
//            }

//            node1.Scores[0] /= score;
//            node1.Scores[1] /= score;
//        }

//        private static void GoThroughGraph(LinkedListNode<CRFNode> currentNode, Dictionary<CRFNode, int> assignments, Action<Dictionary<CRFNode, int>> doOneEndNode)
//        {
//            var assignmentsCopy = new Dictionary<CRFNode, int>(assignments);
//            assignments.Add(currentNode.Value, 0);
//            assignmentsCopy.Add(currentNode.Value, 1);
//            if (currentNode.Equals(currentNode.List.Last))
//            {
//                doOneEndNode(assignments);
//                doOneEndNode(assignmentsCopy);
//            }
//            else
//            {
//                GoThroughGraph(currentNode.Next, assignments, doOneEndNode);
//                GoThroughGraph(currentNode.Next, assignmentsCopy, doOneEndNode);
//            }
//        }

//        public static CRFGraph ExampleGraphFour()
//        {
//            var graph = new CRFGraph();
//            var node1 = new CRFNode("0");
//            node1.Scores = new double[2] { 0.3, 0.3 };


//            var node2 = new CRFNode("1");
//            node2.Scores = new double[2] { 0.6, 0.6 };

//            var node3 = new CRFNode("2");
//            node3.Scores = new double[2] { 0.6, 0.6 };

//            var edge1 = new CRFEdge();
//            edge1.Connect(node1, node2);
//            edge1.Scores = new double[2, 2] { { 1.0, 2.0 }, { 3.0, 4.0 } };

//            var edge2 = new CRFEdge();
//            edge2.Connect(node2, node3);
//            edge2.Scores = new double[2, 2] { { 1.0, 2.0 }, { 3.0, 4.0 } };

//            graph.Nodes.AddRange(node1, node2, node3);
//            graph.Edges.AddRange(edge1, edge2);

//            Normalize(graph, node1);
//            return graph;
//        }

//        public static CRFGraph ExampleGraphFive()
//        {
//            var graph = new CRFGraph();
//            var node1 = new CRFNode("0");
//            node1.Scores = new double[2] { 1, 0.1 };


//            var node2 = new CRFNode("1");
//            node2.Scores = new double[2] { 0.1, 1 };

//            var node3 = new CRFNode("2");
//            node3.Scores = new double[2] { 0.1, 1 };

//            var edge1 = new CRFEdge();
//            edge1.Connect(node1, node2);
//            edge1.Scores = new double[2, 2] { { 1.0, 0.01 }, { 0.01, 1.0 } };

//            var edge2 = new CRFEdge();
//            edge2.Connect(node2, node3);
//            edge2.Scores = new double[2, 2] { { 1.0, 0.01 }, { 0.01, 1.0 } };


//            var edge3 = new CRFEdge();
//            edge3.Connect(node1, node3);
//            edge3.Scores = new double[2, 2] { { 1.0, 0.01 }, { 0.01, 1.0 } };

//            graph.Nodes.AddRange(node1, node2, node3);
//            graph.Edges.AddRange(edge1, edge2);

//            Normalize(graph, node1);
//            return graph;
//        }

//        public static CRFGraph ExampleGraphSix()
//        {
//            var graph = new CRFGraph();
//            var node1 = new CRFNode("0");
//            node1.Scores = new double[2] { 1, 0.1 };


//            var node2 = new CRFNode("1");
//            node2.Scores = new double[2] { 0.1, 1 };

//            var node3 = new CRFNode("2");
//            node3.Scores = new double[2] { 0.1, 1 };

//            var edge1 = new CRFEdge();
//            edge1.Connect(node1, node2);
//            edge1.Scores = new double[2, 2] { { 1.0, 10 }, { 10, 1.0 } };

//            var edge2 = new CRFEdge();
//            edge2.Connect(node2, node3);
//            edge2.Scores = new double[2, 2] { { 1.0, 10 }, { 10, 1.0 } };


//            var edge3 = new CRFEdge();
//            edge3.Connect(node1, node3);
//            edge3.Scores = new double[2, 2] { { 1.0, 10 }, { 10, 1.0 } };

//            graph.Nodes.AddRange(node1, node2, node3);
//            graph.Edges.AddRange(edge1, edge2);

//            Normalize(graph, node1);
//            return graph;
//        }

//        public static LinkedList<CRFGraph> AllGraphs()
//        {
//            var ll = new LinkedList<CRFGraph>();
//            ll.AddRange(ExampleGraphOne(), ExampleGraphTwo(), ExampleGraphThree(), ExampleGraphFour(), ExampleGraphFive(), ExampleGraphSix());
//            return ll;
//        }
//    }
//}
