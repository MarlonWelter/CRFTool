using CodeBase;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    class BetaDistribution
    {
        private List<double> scores;
        private double mean;
        private double variance;
        private double alpha;
        private double beta;
        private double B;

        public BetaDistribution(List<double> scores)
        {
            this.scores = scores;
            calculateMean();
            calculateVariance();
            calculateAlpha();
            calculateBeta();
            B = SpecialFunctions.Beta(alpha, beta);
        }

        private void calculateMean()
        {
            var sum = 0.0;
            foreach (var value in scores)
                sum += value;
            mean = sum/scores.Count;
        }

        private void calculateVariance()
        {
            var sum = 0.0;
            foreach (var value in scores)
                sum += Math.Pow(value-mean,2);
            variance = sum / scores.Count;
        }

        private void calculateAlpha()
        {
            alpha = -(mean * (Math.Pow(mean, 2) - mean + variance)) / variance;
        }

        private void calculateBeta()
        {
            beta = ((mean - 1) * (Math.Pow(mean, 2) - mean + variance)) / variance;
        }

        public double getValue(double score)
        {
            return 1/B * Math.Pow(score, alpha-1) * Math.Pow(1-score, beta-1);
        }

        public List<double> getSamples(int number)
        {
            Beta dist = new Beta(alpha, beta);
            return dist.Samples().Take(number).ToList();
        }
    }
}
