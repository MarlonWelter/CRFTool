using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class Program
    {

        public static Random Random { get; set; } = new Random();
        static void Main(string[] args)
        {

            new ConsoleLogger();

            var input = CreateExampleInput.Do();



            var units2 = CreateGWUnits();
            var engine2 = new MultiUnitEngine<MinBorderInstance, MinBorderAction>(units2, new TrackBestResultLogic<MinBorderInstance>(new MinBorderInstanceEvaluation()), 12, 12);
            engine2.NewOutput.AddPath((data) => OutputNodeList(data));
            engine2.Name = "engine";

            engine2.InputStack.Add(input);
            engine2.Awake();

            var units = CreateUnits();
            var engine = new MultiStrategyGreedyEngine<MinBorderInstance>(units, new TrackBestResultLogic<MinBorderInstance>(new MinBorderInstanceEvaluation()), 12, 12);
            engine.NewOutput.AddPath((data) => OutputNodeList(data));
            engine.Name = "engine";

            engine.InputStack.Add(input);
            engine.Awake();

            Console.Read();

        }

        private static void OutputNodeList(MinBorderInstance result)
        {
            Log.Post("Best Result Found: " + result.MaxBorderSize);
            var resultString = "Resulting List: ";
            foreach (var item in result.Nodes.OrderBy(n => result.NodeOrder[n.OrderId]))
            {
                resultString += item.Name + " - ";
            }
            resultString.Remove(resultString.Length - 3);
            Log.Post(resultString, LogCategory.Overview);
        }

        private static IEnumerable<Runner<MinBorderInstance>> CreateUnits()
        {
            yield return new SingleStrategyGreedyUnit<MinBorderInstance>(new GreedyStrategy()) { Name = "GreedyUnit" };
        }
        private static IEnumerable<GWUnit<MinBorderInstance, MinBorderAction>> CreateGWUnits()
        {
            yield return new MinBorderUnit(new GreedyEvaluation()) { Name = "GreedyUnit" };
            yield return new MinBorderBigUnit(new GreedyEvaluation(), 10) { Name = "GreedyUnit" };
        }
    }
}
