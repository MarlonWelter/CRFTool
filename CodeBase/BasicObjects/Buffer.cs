using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{

    public class Buffer<Type>
    {
        public event EventHandler<SimO<Type>> ElementAdded;

        public Buffer(int bufferSize = int.MaxValue)
        {
            MaxSize = bufferSize;
        }

        private readonly LinkedList<Type> buffer = new LinkedList<Type>();
        public int MaxSize { get; set; }

        public Type Add(Type element)
        {
            var returnVal = default(Type);
            lock (buffer)
            {
                buffer.AddFirst(element);
                if (buffer.Count > MaxSize)
                {
                    returnVal = buffer.Last.Value;
                    buffer.RemoveLast();
                }
            }
            ElementAdded.Fire(element);
            return returnVal;
        }

        public IEnumerable<Type> Add(IEnumerable<Type> elements)
        {
            var returnVals = new LinkedList<Type>();
            foreach (var element in elements)
            {
                Add(element);
            }
            return returnVals;
        }

        public Type Get()
        {
            Type returnVal = default(Type);
            lock (buffer)
            {
                if (buffer.Count > 0)
                {
                    returnVal = buffer.Last.Value;
                    buffer.RemoveLast();
                }
            }
            return returnVal;
        }
        public Type NonRemovingGet()
        {
            Type returnVal = default(Type);
            lock (buffer)
            {
                if (buffer.Count > 0)
                {
                    returnVal = buffer.Last.Value;
                }
            }
            return returnVal;
        }

        public Type Get(Func<Type, bool> criteria)
        {
            Type returnVal = default(Type);
            lock (buffer)
            {
                if (buffer.Count > 0)
                {
                    returnVal = buffer.FirstOrDefault(criteria);
                    if ((!(returnVal == null) && !(returnVal.Equals(default(Type)))))
                        buffer.Remove(returnVal);
                }
            }
            return returnVal;
        }
        public IEnumerable<Type> GetMany(Func<Type, bool> criteria)
        {
            var returnVal = default(IEnumerable<Type>);
            lock (buffer)
            {
                if (buffer.Count > 0)
                {
                    returnVal = buffer.Where(criteria);
                    if ((!(returnVal == null) && (!returnVal.Equals(default(IEnumerable<Type>)))))
                    {
                        foreach (var item in returnVal)
                        {
                            buffer.Remove(item);
                        }
                    }
                }
            }
            return returnVal;
        }

        public Type NonRemovingGet(Func<Type, bool> criteria)
        {
            Type returnVal = default(Type);
            lock (buffer)
            {
                if (buffer.Count > 0)
                {
                    returnVal = buffer.FirstOrDefault(criteria);
                }
            }
            return returnVal;
        }

        public bool Remove(Type element)
        {
            var success = false;
            lock(buffer)
            {
                success = buffer.Remove(element);
            }
            return success;
        }

        public LinkedList<Type> GetAll()
        {
            var returnVal = new LinkedList<Type>();
            lock (buffer)
            {
                returnVal.AddRange(buffer);
                buffer.Clear();
            }
            return returnVal;
        }
        public LinkedList<Type> NonRemovingGetAll()
        {
            var ll = new LinkedList<Type>();
            lock(buffer)
            {
                ll.AddRange(buffer);
            }
            return ll;
        }
    }
}
