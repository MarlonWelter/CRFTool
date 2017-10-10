using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFBase;
using CRFGraph = CodeBase.IGWGraph<CodeBase.ICRFNodeData, CodeBase.ICRFEdgeData, CodeBase.ICRFGraphData>;

namespace PPIBase
{
    //public class SimpleSampling
    //{
    //    public static Dictionary<Assign, double> Do(CRFGraph graph, int samplingSize)
    //    {
    //        var rdm = new Random();
    //        var sumAssigns = new Dictionary<Assign, double>();
    //        //fill sumAssign
    //        foreach (var node in graph.Nodes)
    //        {
    //            sumAssigns.Add(new Assign { Node = node, Label = 0 }, 0);
    //            sumAssigns.Add(new Assign { Node = node, Label = 1 }, 0);
    //        }

    //        for (int currentSampling = 0; currentSampling < samplingSize; currentSampling++)
    //        {
    //            var score = 0.0;
    //            foreach (var node in graph.Nodes)
    //            {
    //                node.TempAssign = (rdm.NextDouble() >= 0.5) ? 1 : 0;
    //                score += Math.Log(node.Score(node.TempAssign));
    //            }
    //            foreach (var edge in graph.Edges)
    //            {
    //                score += Math.Log(edge.TempScore());
    //            }
    //            foreach (var node in graph.Nodes)
    //            {
    //                sumAssigns[new Assign { Node = node, Label = node.TempAssign }] += Math.Exp(score);
    //            }
    //        }

    //        return sumAssigns;
    //    }

    //}

    //public class RejectionMethodSampling<SampleElementType>
    //{
    //    public static IEnumerable<SampleElementType> Do(Random rdm, int sampleSize, RandomVariable<SampleElementType> randomVariable, Func<SampleElementType, double> marginalDistribution)
    //    {
    //        //Create Sample
    //        var sample = randomVariable.DrawSample(sampleSize).ToList();

    //        //Rejection Step
    //        var filteredSample = new LinkedList<SampleElementType>();
    //        //compute C
    //        double c = 0.0;
    //        foreach (var item in sample)
    //        {
    //            double marginalProbability = marginalDistribution(item);
    //            double randVarProb = randomVariable.MarginalProbability(item);
    //            c = Math.Max(c, marginalProbability / randVarProb);

    //        }
    //        Log.Post("C: " + Math.Round(c, 2));
    //        // reject
    //        foreach (var item in sample)
    //        {

    //            double marginalProbability = marginalDistribution(item);
    //            double randVarProb = randomVariable.MarginalProbability(item);
    //            var chance = marginalProbability / (randVarProb * c);
    //            var rdmNr = rdm.NextDouble();
    //            bool reject = rdmNr > chance;

    //            if (!reject)
    //            {
    //                filteredSample.AddLast(item);

    //            }
    //        }

    //        return filteredSample;
    //    }
    //}

    public struct Assign
    {
        public CRFNode Node { get; set; }
        public int Label { get; set; }
    }

    

    //public class UniformDistributionRV<T> : RandomVariable<T>
    //{
    //    public UniformDistributionRV(IEnumerable<T> elements)
    //    {
    //        this.elements = new List<T>(elements);
    //    }
    //    private Random random = new Random();
    //    private List<T> elements;

    //    public IEnumerable<T> DrawSample(int sampleSize)
    //    {
    //        return elements.RandomTake(sampleSize, random);
    //    }

    //    public T Draw()
    //    {
    //        return elements[random.Next(elements.Count)];
    //    }

    //    public double MarginalProbability(T data)
    //    {
    //        return 1.0 / elements.Count;
    //    }
    //}

    //public class CRFGraphUniformDistributionRV : RandomVariable<Dictionary<CRFNode, int>>
    //{
    //    public CRFGraphUniformDistributionRV(CRFGraph graph, Random random)
    //    {
    //        Graph = graph;
    //        this.random = random;
    //    }
    //    private Random random;
    //    public CRFGraph Graph { get; set; }

    //    public IEnumerable<Dictionary<CRFNode, int>> DrawSample(int sampleSize)
    //    {
    //        for (int i = 0; i < sampleSize; i++)
    //        {
    //            var assigns = new Dictionary<CRFNode, int>();
    //            foreach (var node in Graph.Nodes)
    //            {
    //                assigns.Add(node, random.NextDouble() > 0.5 ? 1 : 0);
    //            }
    //            yield return assigns;
    //        }
    //    }

    //    public Dictionary<CRFNode, int> Draw()
    //    {
    //        return DrawSample(1).First();
    //    }

    //    public double MarginalProbability(Dictionary<CRFNode, int> element)
    //    {
    //        return 1.0 / Math.Pow(2, Graph.Nodes.Count);
    //    }
    //}

}
