//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace General.General
//{
//    public class MITCell
//    {
//        public string Id { get; protected set; }


//        public MITCell(MITCell context)
//        {
//            if (context != null)
//            {
//                SendTo(context);
//                Context = context;
//            }

//            object stub = null;
//            CommandOut(GeneralCommands.MITCellId, ref stub, null);
//            Id = stub as string;

//        }
//        public MITCell Context { get; protected set; }

//        public delegate void CommandReaction(object sender, ref object returnValue, params object[] parameter);
//        protected readonly Dictionary<MITCommand, LinkedList<MITCell>> ForwardList = new Dictionary<MITCommand, LinkedList<MITCell>>();
//        protected readonly LinkedList<MITCell> CommandReceiver = new LinkedList<MITCell>();
//        protected readonly Dictionary<MITCommand, CommandReaction> CommandBindings = new Dictionary<MITCommand, CommandReaction>();

//        protected void CommandOut(MITCommand command, ref object returnValue, params object[] parameter)
//        {
//            foreach (var item in CommandReceiver)
//            {
//                item.CommandIn(command, this, ref returnValue, parameter);
//            }
//            if (ForwardList.ContainsKey(command))
//            {
//                foreach (var item in ForwardList[command])
//                {
//                    item.CommandIn(command, this, ref returnValue, parameter);
//                }
//            }
//        }
//        protected void CommandOut<T>(MITCommand command, ref T returnValue, params object[] parameter)
//        {
//            object stub = returnValue;
//            foreach (var item in CommandReceiver)
//            {
//                item.CommandIn(command, this, ref stub, parameter);
//            }
//            if (ForwardList.ContainsKey(command))
//            {
//                foreach (var item in ForwardList[command])
//                {
//                    item.CommandIn(command, this, ref stub, parameter);
//                }
//            }
//            returnValue = (T)stub;
//        }
//        protected void CommandOut(MITCommand command, params object[] parameter)
//        {
//            object stub = null;
//            foreach (var item in CommandReceiver)
//            {
//                item.CommandIn(command, this, ref stub, parameter);
//            }
//            if (ForwardList.ContainsKey(command))
//            {
//                foreach (var item in ForwardList[command])
//                {
//                    item.CommandIn(command, this, ref stub, parameter);
//                }
//            }
//        }

//        protected T CommandOut<T>(MITCommand command, params object[] parameter)
//        {
//            object stub = null;
//            foreach (var item in CommandReceiver)
//            {
//                item.CommandIn(command, this, ref stub, parameter);
//            }
//            if (ForwardList.ContainsKey(command))
//            {
//                foreach (var item in ForwardList[command])
//                {
//                    item.CommandIn(command, this, ref stub, parameter);
//                }
//            }
//            return (T)stub;
//        }

//        public void CommandIn(MITCommand command, object sender, ref object returnValue, params object[] parameter)
//        {
//            if (CommandBindings.ContainsKey(command))
//                CommandBindings[command](sender, ref returnValue, parameter);
//            else
//            {
//                if (ForwardList.ContainsKey(command))
//                {
//                    foreach (var item in ForwardList[command])
//                    {
//                        item.CommandIn(command, sender, ref returnValue, parameter);
//                    }
//                }
//                else
//                    CommandOut(command, ref returnValue, parameter);
//            }
//        }

//        public void ReceiveFrom(MITCell cell)
//        {
//            cell.SendTo(this);
//        }

//        public void SendTo(MITCell cell)
//        {
//            CommandReceiver.AddLast(cell);
//        }

//        public void ForwardTo(MITCommand command, MITCell cell)
//        {
            
//            if (ForwardList.ContainsKey(command))
//            {
//                if(!ForwardList[command].Contains(cell))
//                    ForwardList[command].AddLast(cell);
//            }
//            else
//            {
//                LinkedList<MITCell> list = new LinkedList<MITCell>();
//                list.AddFirst(cell);
//                ForwardList.Add(command, list);
//            }
//        }

//        protected void Bind(MITCommand command, CommandReaction reaction)
//        {
//            CommandBindings.Add(command, reaction);
//            Context.ForwardTo(command, this);
//        }
//    }
//}
