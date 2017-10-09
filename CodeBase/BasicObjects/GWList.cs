using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class GWList<T> : IList<T>
    {
        private List<T> backList = new List<T>();

        public int CurrentIndex { get; private set; }
        public T CurrentElement
        {
            get { return backList.NotNullOrEmpty() ? backList[CurrentIndex] : default(T); }
            set { if (backList.NotNullOrEmpty()) backList[CurrentIndex] = value; }
        }

        public void Next() => CurrentIndex = (CurrentIndex + 1) % backList.Count;
        public void Previous() => CurrentIndex = CurrentIndex > 0 ? CurrentIndex - 1 : CurrentIndex;

        public T this[int index]
        {
            get
            {
                return backList[index];
            }

            set
            {
                backList[index] = value;
            }
        }

        public int Count => backList.Count;

        public bool IsReadOnly => false;

        public void Add(T item) => backList.Add(item);

        public void Clear() => backList.Clear();

        public bool Contains(T item) => backList.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => backList.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => backList.GetEnumerator();

        public int IndexOf(T item) => backList.IndexOf(item);

        public void Insert(int index, T item) => backList.Insert(index, item);

        public bool Remove(T item) => backList.Remove(item);

        public void RemoveAt(int index) => backList.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => (backList as IEnumerable).GetEnumerator();
    }

    public class GWLinkedList<T> where T : IScoreHolder
    {
        public GWLinkedList(int maxSize, double keyProbability)
        {
            MaxSize = maxSize;
            KeyProbability = keyProbability;
        }
        public int MaxSize { get; set; }

        public int Count { get; private set; }

        private Random Random = new Random();
        public double KeyProbability { get; set; }

        public GWLinkedListNode<T> First { get; set; }
        public GWLinkedListNode<T> Last { get; set; }

        public List<T> ToList()
        {
            var linkedList = new LinkedList<T>();
            var node = Last;
            while (node != null)
            {
                linkedList.AddFirst(node.Data);
                node = node.Next;
            }

            return linkedList.ToList();
        }

        private int internalCounter = 0;
        public void SortedInsert(T data)
        {
            if (Last == null)
            {
                var newNode2 = new GWLinkedListNode<T>(data, internalCounter++ == 0);
                internalCounter = internalCounter % 4 == 0 ? 0 : internalCounter;
                First = newNode2;
                Last = newNode2;
                return;
            }

            if (Count == MaxSize && Last.Data.Score >= data.Score)
                return;

            var newNode = new GWLinkedListNode<T>(data, internalCounter++ == 0);
            internalCounter = internalCounter % 4 == 0 ? 0 : internalCounter;
            Count++;

            var node = Last;
            if (newNode.Data.Score > node.Data.Score)
                InsertSmallSpeed(node, newNode, true);
            else
            {
                if (Count <= MaxSize)
                {
                    node.Previous = newNode;
                    newNode.Next = node;
                    Last = newNode;
                }
            }
        }

        private void InsertSmallSpeed(GWLinkedListNode<T> node, GWLinkedListNode<T> nodeToInsert, bool canGoHighSpeed)
        {
            while (nodeToInsert.Data.Score > node.Data.Score)
            {
                if (node.IsKey && canGoHighSpeed)
                {
                    InsertHighSpeed(node, nodeToInsert);
                    return;
                }
                if (node.Next != null)
                    node = node.Next;
                else
                {
                    node.Next = nodeToInsert;
                    nodeToInsert.Previous = node;
                    First = nodeToInsert;

                    if (Count > MaxSize)
                    {
                        RemoveLast();
                        return;
                    }
                    return;
                }
            }
            var oldPrevious = node.Previous;
            node.Previous = nodeToInsert;
            nodeToInsert.Next = node;
            nodeToInsert.Previous = oldPrevious;
            oldPrevious.Next = nodeToInsert;

            if (Count > MaxSize)
            {
                RemoveLast();
                return;
            }
        }

        private void InsertHighSpeed(GWLinkedListNode<T> node, GWLinkedListNode<T> nodeToInsert)
        {
            while (node.NextKey != null && nodeToInsert.Data.Score > node.NextKey.Data.Score)
            {
                node = node.NextKey;
            }
            if (node.IsKey)
            {
                nodeToInsert.PreviousKey = node;
                if (node.NextKey != null)
                {
                    node.NextKey.PreviousKey = nodeToInsert;
                    nodeToInsert.NextKey = node.NextKey;
                }
                node.NextKey = nodeToInsert;
            }
            InsertSmallSpeed(node, nodeToInsert, false);
        }

        private void RemoveLast()
        {
            var item = Last;
            if (item.IsKey)
            {
                if (item.NextKey != null)
                    item.NextKey.PreviousKey = null;
                item.NextKey = null;
            }
            Last = item.Next;
            item.Next = null;
            Last.Previous = null;
            Count--;
        }
    }

    public class GWLinkedListNode<T>
    {
        public GWLinkedListNode(T data, bool isKey)
        {
            Data = data;
            IsKey = isKey;
        }
        public T Data { get; set; }
        public GWLinkedListNode<T> Next { get; set; }
        public GWLinkedListNode<T> Previous { get; set; }

        public bool IsKey { get; set; }
        public GWLinkedListNode<T> NextKey { get; set; }
        public GWLinkedListNode<T> PreviousKey { get; set; }
    }
}
