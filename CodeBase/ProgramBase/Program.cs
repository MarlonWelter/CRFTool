using CodeBase.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeBase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Build();
            var line = "";

            while ((line = Console.ReadLine()) != "exit")
            {
                var words = new Regex("\\s").Split(line);
                var cmd = new CommandIn(words[0], words.Skip(1).ToArray());

                cmd.RequestInDefaultContext();

                switch (cmd.Feedback)
                {
                    case CommandInResult.UnknownCommand:
                        Console.WriteLine("Unknown Command.");
                        break;
                    case CommandInResult.Forwarded:
                        Console.WriteLine("Command is being processed.");
                        break;
                    case CommandInResult.Error:
                        Console.WriteLine("Error: " + cmd.FeedbackText);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void Build()
        {
            new CommandKnowledgeManager();
            new GraphAnalysisManager();
            new LearnCommand("anaGraphs", (args) => AnalyseStandardGraphs.Do(int.Parse(args[0]))).RequestInDefaultContext();
            new LearnCommand("logC", (args) => new ConsoleLogger()).RequestInDefaultContext();
        }

    }
    public static class BaseProgram
    {
        public static MEvent Exit = new MEvent("Program.Exit");
    }
}
