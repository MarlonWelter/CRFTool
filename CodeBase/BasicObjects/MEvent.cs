
using System;
using System.Collections.Generic;

namespace CodeBase
{
    public static class CrossroadsExtensions
    {
        public static void Enter(this ICRActionLogic<bool> logic)
        {
            logic.Enter(true);
        }
        public static void Enter<T>(this IHas<ICRActionLogic<T>> logicHolder, T data)
        {
            logicHolder.Logic.Enter(data);
        }
        public static void AddPath<T>(this IHas<ICRModificationLogic<T>> logicHolder, Action<T> callBackMethod)
        {
            logicHolder.Logic.AddPath(callBackMethod);
        }
        public static void RemovePath<T>(this IHas<ICRModificationLogic<T>> logicHolder, Action<T> callBackMethod)
        {
            logicHolder.Logic.RemovePath(callBackMethod);
        }
    }

    public interface ICRModificationLogic<out DataType> : ICRModificationLogic
    {
        void AddPath(Action<DataType> action);
        void RemovePath(Action<DataType> action);
    }
    public interface ICRModificationLogic : ILogic
    {
        void AddPath(Action action);
        void RemovePath(Action action);
    }
    public interface ICRActionLogic<in DataType> : IGWContext, ILogic
    {
        void Enter(DataType data);
    }
    public interface ICRActionLogic
    {
        void Enter();
    }
    public interface ICrossroadsLogic<Datatype> : ICRModificationLogic<Datatype>, ICRActionLogic<Datatype>
    {

    }
    public interface ICrossroadsLogic : ICRModificationLogic, ICRActionLogic
    {

    }

    public class CrossroadsLogic<Type> : ICrossroadsLogic<Type>
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public static CrossroadsLogic<Type> operator +(CrossroadsLogic<Type> eventlogic, Action<Type> action)
        {
            eventlogic.AddPath(action);
            return eventlogic;
        }

        private readonly LinkedList<Action<Type>> Listener = new LinkedList<Action<Type>>();
        private readonly LinkedList<Action> Listener2 = new LinkedList<Action>();
        public void Enter(Type request)
        {
            //lock (Listener)
            {
                foreach (var item in Listener)
                {
                    item(request);
                }
                foreach (var item in Listener2)
                {
                    item();
                }
            }
        }
        public void AddPath(Action<Type> callBackMethod)
        {
            lock (Listener)
            {
                if (!Listener.Contains(callBackMethod))
                    Listener.AddLast(callBackMethod);
            }
        }
        public void RemovePath(Action<Type> callBackMethod)
        {
            lock (Listener)
            {
                Listener.Remove(callBackMethod);
            }
        }

        public void AddPath(Action callBackMethod)
        {
            lock (Listener)
            {
                if (!Listener2.Contains(callBackMethod))
                    Listener2.AddLast(callBackMethod);
            }
        }
        public void RemovePath(Action callBackMethod)
        {
            lock (Listener)
            {
                Listener2.Remove(callBackMethod);
            }
        }
    }

    public class CrossroadsLogic : ICrossroadsLogic
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public static CrossroadsLogic operator +(CrossroadsLogic eventlogic, Action action)
        {
            eventlogic.AddPath(action);
            return eventlogic;
        }

        private readonly LinkedList<Action> Listener = new LinkedList<Action>();
        public void Enter()
        {
            //lock (Listener)
            {
                foreach (var item in Listener)
                {
                    item();
                }
            }
        }
        public void AddPath(Action callBackMethod)
        {
            lock (Listener)
            {
                if (!Listener.Contains(callBackMethod))
                    Listener.AddLast(callBackMethod);
            }
        }
        public void RemovePath(Action callBackMethod)
        {
            lock (Listener)
            {
                Listener.Remove(callBackMethod);
            }
        }

    }

    public class MEvent<DataType> : CrossroadsLogic<DataType>, ILogWriter
    {
        public static MEvent<DataType> operator +(MEvent<DataType> eventlogic, Action<DataType> action)
        {
            eventlogic.AddPath(action);
            return eventlogic;
        }

        public MEvent(string logName = "AnonymusEvent")
        {
            name = logName;
        }
        public IGWContext Context { get; set; }
        public bool DoLog { get; set; }
        private readonly string name;
        public string Name
        {
            get { return name; }
        }
    }

    public class MEvent : CrossroadsLogic
    {

        public MEvent(string logName = "AnonymusEvent")
        {
            name = logName;
        }
        public IGWContext Context { get; set; }
        public bool DoLog { get; set; }
        private readonly string name;
        public string Name
        {
            get { return name; }
        }
    }
}
