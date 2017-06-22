using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeBase
{
    public static class LinkedListHandler
    {

        public static void SortedInsert<S, T>(this LinkedList<S> List, ICollection<S> Data, int maxSize, IComparer<T> comparer) where S : T
        {
            foreach (S TData in Data)
            {
                SortedInsert(List, TData, maxSize, comparer);
            }
        }
        public static void SortedInsert<S>(this LinkedList<S> List, ICollection<S> Data, int maxSize) where S : IComparable
        {
            foreach (S TData in Data)
            {
                SortedInsert(List, TData, maxSize);
            }
        }
        public static void SortedInsert<S>(this LinkedList<S> List, ICollection<S> Data) where S : IComparable
        {
            foreach (S TData in Data)
            {
                SortedInsert(List, TData);
            }
        }
        public static void SortedInsert<S>(this IList<S> List, ICollection<S> Data, int maxSize, IComparer<S> comparer)
        {
            foreach (S TData in Data)
            {
                SortedInsert(List, TData, maxSize, comparer);
            }
        }
        public static void SortedInsert<S, T>(this LinkedList<S> List, ICollection<S> Data, IComparer<T> comparer) where S : T
        {
            foreach (S TData in Data)
            {
                SortedInsert(List, TData, comparer);
            }
        }
        public static void SortedInsert<S, T>(this IList<S> List, ICollection<S> Data, IComparer<T> comparer) where S : T
        {
            foreach (S TData in Data)
            {
                SortedInsert(List, TData, comparer);
            }
        }
        public static S SortedInsert<S, T>(this LinkedList<S> List, S Data, int maxSize, IComparer<T> comparer) where S : T
        {
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                if (List.Count > maxSize)
                {
                    var item = List.Last.Value;
                    List.RemoveLast();
                    return item;
                }
                return default(S);
            }
            LinkedListNode<S> node = List.Last;
            while (comparer.Compare(Data, node.Value) > 0)
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List.Last.Value;
                        List.RemoveLast();
                        return item;
                    }
                    return default(S);
                }
            }
            List.AddAfter(node, Data);
            if (List.Count > maxSize)
            {
                var item = List.Last.Value;
                List.RemoveLast();
                return item;
            }
            return default(S);
        }

        public static int SortedInsertPos<S, T>(this LinkedList<S> List, S Data, int maxSize, IComparer<T> comparer) where S : T
        {
            int insertPosition = List.Count;
            if (insertPosition == 0)
            {
                List.AddFirst(Data);
                if (insertPosition + 1 > maxSize)
                {
                    var item = List.Last.Value;
                    List.RemoveLast();
                    return insertPosition;
                }
                return insertPosition;
            }
            LinkedListNode<S> node = List.Last;
            while (comparer.Compare(Data, node.Value) > 0)
            {
                insertPosition--;
                if (node.Previous != null)
                {
                    node = node.Previous;
                }
                else
                {
                    List.AddBefore(node, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List.Last.Value;
                        List.RemoveLast();
                        return insertPosition;
                    }
                    return insertPosition;
                }
            }
            List.AddAfter(node, Data);
            if (List.Count > maxSize)
            {
                var item = List.Last.Value;
                List.RemoveLast();
                return insertPosition;
            }
            return insertPosition;
        }

        public static S SortedInsert<S>(this LinkedList<S> List, S Data, int maxSize, Func<S, double> score)
        {
            if (List == null)
                return default(S);
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                if (List.Count > maxSize)
                {
                    var item = List.Last.Value;
                    List.RemoveLast();
                    return item;
                }
                return default(S);
            }
            LinkedListNode<S> node = List.Last;
            while (score(Data) > score(node.Value))
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List.Last.Value;
                        List.RemoveLast();
                        return item;
                    }
                    return default(S);
                }
            }
            List.AddAfter(node, Data);
            if (List.Count > maxSize)
            {
                var item = List.Last.Value;
                List.RemoveLast();
                return item;
            }
            return default(S);
        }
        public static S SortedInsert<S>(this LinkedList<S> List, S Data, int maxSize) where S : IComparable
        {
            if (List == null)
                return default(S);
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                if (List.Count > maxSize)
                {
                    var item = List.Last.Value;
                    List.RemoveLast();
                    return item;
                }
                return default(S);
            }
            LinkedListNode<S> node = List.Last;
            while (Data.CompareTo(node.Value) > 0)
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List.Last.Value;
                        List.RemoveLast();
                        return item;
                    }
                    return default(S);
                }
            }
            List.AddAfter(node, Data);
            if (List.Count > maxSize)
            {
                var item = List.Last.Value;
                List.RemoveLast();
                return item;
            }
            return default(S);
        }

        public static S SortedInsertScore<S>(this LinkedList<S> List, S Data, int maxSize) where S : IScoreHolder
        {
            if (List == null)
                return default(S);
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                if (List.Count > maxSize)
                {
                    var item = List.Last.Value;
                    List.RemoveLast();
                    return item;
                }
                return default(S);
            }
            LinkedListNode<S> node = List.Last;
            while (Data.Score > (node.Value).Score)
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List.Last.Value;
                        List.RemoveLast();
                        return item;
                    }
                    return default(S);
                }
            }
            List.AddAfter(node, Data);
            if (List.Count > maxSize)
            {
                var item = List.Last.Value;
                List.RemoveLast();
                return item;
            }
            return default(S);
        }

        public static S SortedInsert<S>(this IList<S> List, S Data, int maxSize, IComparer<S> comparer)
        {
            if (List.Count == 0)
            {
                List.Add(Data);
                if (List.Count > maxSize)
                {
                    var item = List[0];
                    List.Remove(item);
                    return item;
                }
                return default(S);
            }
            int counter = List.Count - 1;
            while (comparer.Compare(Data, List[counter]) > 0)
            {
                if (counter > 0)
                    counter--;
                else
                {
                    List.Insert(0, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List[List.Count - 1];
                        List.Remove(item);
                        return item;
                    }
                    return default(S);
                }
            }
            List.Insert(counter + 1, Data);
            if (List.Count > maxSize)
            {
                var item = List[List.Count - 1];
                List.Remove(item);
                return item;
            }
            return default(S);
        }
        public static S SortedInsert<S>(this IList<S> List, S Data, int maxSize, Func<S, double> comparer)
        {
            if (List.Count == 0)
            {
                List.Add(Data);
                if (List.Count > maxSize)
                {
                    var item = List[0];
                    List.Remove(item);
                    return item;
                }
                return default(S);
            }
            int counter = List.Count - 1;
            while (comparer(Data) > comparer(List[counter]))
            {
                if (counter > 0)
                    counter--;
                else
                {
                    List.Insert(0, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List[List.Count - 1];
                        List.Remove(item);
                        return item;
                    }
                    return default(S);
                }
            }
            List.Insert(counter + 1, Data);
            if (List.Count > maxSize)
            {
                var item = List[List.Count - 1];
                List.Remove(item);
                return item;
            }
            return default(S);
        }
        public static S SortedInsert<S>(this IList<S> List, S Data, int maxSize) where S : IComparable
        {
            if (List.Count == 0)
            {
                List.Add(Data);
                if (List.Count > maxSize)
                {
                    var item = List[0];
                    List.Remove(item);
                    return item;
                }
                return default(S);
            }
            int counter = List.Count - 1;
            while (Data.CompareTo(List[counter]) > 0)
            {
                if (counter > 0)
                    counter--;
                else
                {
                    List.Insert(0, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List[List.Count - 1];
                        List.Remove(item);
                        return item;
                    }
                    return default(S);
                }
            }
            List.Insert(counter + 1, Data);
            if (List.Count > maxSize)
            {
                var item = List[List.Count - 1];
                List.Remove(item);
                return item;
            }
            return default(S);
        }
        //public static S SortedInsert<S>(this IList<S> List, S Data, int maxSize, Func<S, double> score)
        //{
        //    if (List.Count == 0)
        //    {
        //        List.Add(Data);
        //        if (List.Count > maxSize)
        //        {
        //            var item = List[0];
        //            List.Remove(item);
        //            return item;
        //        }
        //        return default(S);
        //    }
        //    int counter = List.Count - 1;
        //    while (score(Data) > score(List[counter]))
        //    {
        //        if (counter > 0)
        //            counter--;
        //        else
        //        {
        //            List.Insert(0, Data);
        //            if (List.Count > maxSize)
        //            {
        //                var item = List[List.Count - 1];
        //                List.Remove(item);
        //                return item;
        //            }
        //            return default(S);
        //        }
        //    }
        //    List.Insert(counter + 1, Data);
        //    if (List.Count > maxSize)
        //    {
        //        var item = List[List.Count - 1];
        //        List.Remove(item);
        //        return item;
        //    }
        //    return default(S);
        //}

        public static void SortedInsert<S, T>(this LinkedList<S> List, S Data, IComparer<T> comparer) where S : T
        {
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                return;
            }
            LinkedListNode<S> node = List.Last;
            while (comparer.Compare(Data, node.Value) > 0)
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    return;
                }
            }
            List.AddAfter(node, Data);
        }
        public static void SortedInsert<S>(this LinkedList<S> List, S Data) where S : IComparable
        {
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                return;
            }
            LinkedListNode<S> node = List.Last;
            while (Data.CompareTo(node.Value) > 0)
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    return;
                }
            }
            List.AddAfter(node, Data);
        }
        public static void SortedInsert<S>(this LinkedList<S> List, S Data, Func<S, double> score)
        {
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                return;
            }
            LinkedListNode<S> node = List.Last;
            while (score(Data) > score(node.Value))
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    return;
                }
            }
            List.AddAfter(node, Data);
        }
        public static void SortedInsert<S, T>(this IList<S> List, S Data, IComparer<T> comparer) where S : T
        {
            if (List.Count == 0)
            {
                List.Add(Data);
                return;
            }
            int counter = List.Count - 1;
            while (comparer.Compare(Data, List[counter]) > 0)
            {
                if (counter > 0)
                    counter--;
                else
                {
                    List.Insert(0, Data);
                    return;
                }
            }
            List.Insert(counter + 1, Data);
        }
        public static void SortedInsert<S, T>(this IList<S> List, S Data, Func<T, double> comparer) where S : T
        {
            if (List.Count == 0)
            {
                List.Add(Data);
                return;
            }
            int counter = List.Count - 1;
            while (comparer(Data) > comparer(List[counter]))
            {
                if (counter > 0)
                    counter--;
                else
                {
                    List.Insert(0, Data);
                    return;
                }
            }
            List.Insert(counter + 1, Data);
        }
        public static void SortedInsert<S>(this IList<S> List, S Data) where S : IComparable
        {
            if (List.Count == 0)
            {
                List.Add(Data);
                return;
            }
            int counter = List.Count - 1;
            while (Data.CompareTo(List[counter]) > 0)
            {
                if (counter > 0)
                    counter--;
                else
                {
                    List.Insert(0, Data);
                    return;
                }
            }
            List.Insert(counter + 1, Data);
        }
        public static LinkedList<S> Copy<S>(this ICollection<S> model)
        {
            LinkedList<S> newList = new LinkedList<S>();
            foreach (S data in model)
            {
                newList.AddLast(data);
            }
            return newList;
        }

    }
}
