using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeBase
{

    public class CommandKnowledgeManager : IRequestListener
    {
        public CommandKnowledgeManager()
        {
            this.Register();
        }
        public IGWContext Context { get; set; }

        private readonly Dictionary<string, Action<string[]>> commandKnowledge = new Dictionary<string, Action<string[]>>();

        public Dictionary<string, Action<string[]>> CommandDict
        {
            get { return commandKnowledge; }
        }

        public void Register()
        {
            this.DoRegister<LearnCommand>(OnLearnCommand);
            this.DoRegister<CommandIn>(OnCommandIn);
        }
        public void Unregister()
        {
            this.DoUnregister<LearnCommand>(OnLearnCommand);
            this.DoUnregister<CommandIn>(OnCommandIn);
        }

        private void OnLearnCommand(LearnCommand obj)
        {
            commandKnowledge.Add(obj.Command, obj.Action);
        }
        private void OnCommandIn(CommandIn obj)
        {
            try
            {
                if (commandKnowledge.ContainsKey(obj.Command))
                {
                    var thread = new Thread(() => { commandKnowledge[obj.Command](obj.CommandParameter); });
                    thread.Start();
                    obj.Feedback = CommandInResult.Forwarded;
                }
                else
                {
                    obj.Feedback = CommandInResult.UnknownCommand;
                }
            }
            catch (Exception e)
            {
                obj.Feedback = CommandInResult.Error;
                obj.FeedbackText = e.Message;
            }
        }

    }


    public class LearnCommand : GWRequest<LearnCommand>
    {
        public LearnCommand(string command, Action<string[]> parameter)
        {
            Command = command;
            Action = parameter;
        }
        public string Command { get; set; }
        public Action<string[]> Action { get; set; }
    }

    public class CommandIn : GWRequest<CommandIn>
    {
        public CommandIn(string command, string[] commandParameter = null)
        {
            Command = command;
            CommandParameter = commandParameter;
        }
        public string Command { get; set; }
        public string[] CommandParameter { get; set; }
        public CommandInResult Feedback { get; set; }

        public string FeedbackText { get; set; }
    }
    public enum CommandInResult
    {
        UnknownCommand,
        Forwarded,
        Error
    }
}
