using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface IResult
    {
        bool IsSuccesful { get; }
        double Score { get; }
    }

    public interface ITestSeriesResult<Resulttype> where Resulttype : IResult
    {
        List<Resulttype> Results { get; }
        double SuccesRatio { get; }
        int Testruns { get; }
    }

    class TestSeriesResult<Resulttype> : ITestSeriesResult<Resulttype> where Resulttype : IResult
    {
        private List<Resulttype> results = new List<Resulttype>();

        public List<Resulttype> Results
        {
            get { return results; }
            set { results = value; }
        }

        public double SuccesRatio { get; set; }
        public int Testruns { get; set; }
    }

    public static class ResultExtensions
    {
        public static ITestSeriesResult<Resulttype> ToTestSeriesResult<Resulttype>(this IEnumerable<Resulttype> results)
            where Resulttype : IResult
        {
            var seriesresult = new TestSeriesResult<Resulttype>();

            int succes = 0;
            foreach (var item in results)
            {
                succes += item.IsSuccesful ? 1 : 0;
            }

            seriesresult.Results = results.OrderByDescending(res => res.Score).ToList();
            seriesresult.Testruns = results.Count();
            seriesresult.SuccesRatio = ((double)succes) / seriesresult.Testruns;

            return seriesresult;
        }
    }
}
