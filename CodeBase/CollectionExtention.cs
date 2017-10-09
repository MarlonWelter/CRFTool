using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public static class X
    {
        public static IEnumerable<int> FromTo(int from, int to)
        {
            if (from > to)
                throw new ArgumentException("'from' must not be bigger than 'to'");
            int counter = from;
            while (counter <= to)
            {
                yield return counter;
                counter++;
            }
        }

        public static T Next<T>(this T t) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return t;

            var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            var currentIndex = values.IndexOf(t);
            var newIndex = (currentIndex + 1) % values.Count;

            return values[newIndex];
        }
    }
    public static class ColX
    {
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> elements, Func<T, double> primaryComparer, params Func<T, double>[] secondaryComparers)
        {
            return elements.OrderBy(t => t, new MultiStepComparer<T>(new SimpleComparer<T>(primaryComparer), (secondaryComparers ?? new Func<T, double>[0]).Select(f => new SimpleComparer<T>(f))));
        }
        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> elements, Func<T, double> primaryComparer, params Func<T, double>[] secondaryComparers)
        {
            return elements.OrderByDescending(t => t, new MultiStepComparer<T>(new SimpleComparer<T>(primaryComparer), (secondaryComparers ?? new Func<T, double>[0]).Select(f => new SimpleComparer<T>(f))));
        }
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> elements, IComparer<T> primaryComparer, params IComparer<T>[] secondaryComparers)
        {
            return elements.OrderBy(t => t, new MultiStepComparer<T>(primaryComparer, secondaryComparers));
        }
        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> elements, IComparer<T> primaryComparer, params IComparer<T>[] secondaryComparers)
        {
            return elements.OrderByDescending(t => t, new MultiStepComparer<T>(primaryComparer, secondaryComparers));
        }
        public static void AddRange<S>(this IDictionary<S, int> dict, IDictionary<S, int> dictToAdd)
        {
            foreach (var item in dictToAdd)
            {
                if (dict.ContainsKey(item.Key))
                    dict[item.Key] += item.Value;
                else
                    dict.Add(item);
            }
        }
        public static void Subtract<S>(this IDictionary<S, int> dict, IDictionary<S, int> dictToAdd)
        {
            foreach (var item in dictToAdd)
            {
                dict[item.Key] -= item.Value;
            }
        }
        public static bool SameElements<T>(this IEnumerable<T> elements, IEnumerable<T> otherElements)
        {
            bool sameElements = true;
            foreach (var item in elements)
            {
                if (!otherElements.Contains(item))
                {
                    sameElements = false;
                    break;
                }
            }
            if (sameElements)
            {
                foreach (var item in otherElements)
                {
                    if (!elements.Contains(item))
                    {
                        sameElements = false;
                        break;
                    }
                }
            }

            return sameElements;
        }

        public static void Do<Type>(this IEnumerable<Type> elements, Action<Type> action)
        {
            foreach (var item in elements)
            {
                action(item);
            }
        }
        public static double Mean(this IEnumerable<double> elements)
        {
            var sum = elements.Sum();
            return sum / elements.Count();
        }
        public static void Add<S, T>(this LinkedList<AgO<S, T>> ll, S s, T t)
        {
            ll.Add(new AgO<S, T>(s, t));
        }
        public static bool Contains<S, T>(this LinkedList<AgO<S, T>> ll, S s)
        {
            return ll.Any(el => el.Data1.Equals(s));
        }
        public static bool Contains<S, T>(this LinkedList<AgO<S, T>> ll, S s, T t)
        {
            return ll.Any(el => el.Data1.Equals(s) && el.Data2.Equals(t));
        }
        public static double Product(this IEnumerable<double> values)
        {
            var product = 1.0;

            foreach (var value in values)
            {
                product *= value;
            }

            return product;
        }
        public static double Product<T>(this IEnumerable<T> elements, Func<T, double> valueOf)
        {
            var product = 1.0;

            foreach (var element in elements)
            {
                product *= valueOf(element);
            }

            return product;
        }
        public static AgO<S, T>[] Init<S, T>(this AgO<S, T>[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new AgO<S, T>();
            }
            return array;
        }

        public static IEnumerable<T> Plus<T>(this IEnumerable<T> baseElements, params T[] elements)
        {
            return baseElements.Concat(elements);
        }

        public static Action<P1> Fix2<P1, P2>(Action<P1, P2> action, P2 p2)
        {
            return new Action<P1>(n => action(n, p2));
        }
        public static Action<P2> Fix1<P1, P2>(this Action<P1, P2> action, P1 p1)
        {
            return new Action<P2>(n => action(p1, n));
        }
        public static void Add(this double[] arrayBase, double[] arrayToAdd, double weight = 1.0)
        {
            for (int i = 0; i < arrayBase.Length; i++)
            {
                arrayBase[i] += arrayToAdd[i] * weight;
            }
        }
        public static void Add(this int[] arrayBase, int[] arrayToAdd, int weight = 1)
        {
            for (int i = 0; i < arrayBase.Length; i++)
            {
                arrayBase[i] += arrayToAdd[i] * weight;
            }
        }

        public static bool EqualClassification<T, C>(this IEnumerable<KeyValuePair<T, C>> dict, IDictionary<T, C> dict2)
        {
            bool sameClassification = true;
            foreach (var entry in dict)
            {
                try
                {
                    sameClassification = entry.Value.Equals(dict2[entry.Key]);
                }
                catch (KeyNotFoundException)
                {
                    sameClassification = false;
                }
                if (!sameClassification)
                    break;
            }
            return sameClassification;
        }

        public static bool EqualClassification<T, C>(this IDictionary<T, C> dict, IDictionary<T, C> dict2)
        {
            bool sameClassification = true;
            foreach (var entry in dict)
            {
                try
                {
                    sameClassification = dict[entry.Key].Equals(dict2[entry.Key]);
                }
                catch (KeyNotFoundException)
                {
                    sameClassification = false;
                }
                if (!sameClassification)
                    break;
            }
            return sameClassification;
        }

        public static T WeightedTake<T>(this LinkedList<T> elements, Func<T, double> weight, Random rand)
        {
            double sum = elements.Sum(e => weight(e));
            if (sum <= 0)
                return default(T);
            double index = rand.NextDouble() * sum;
            var element = elements.First;
            index -= weight(element.Value);

            while (index > 0)
            {
                element = element.Next;
                index -= weight(element.Value);
            }
            return element.Value;
        }

        public static int ChooseIndexWeighted<T>(this IList<T> elements, Func<T, double> weight, Random rand)
        {
            double sum = elements.Sum(e => weight(e));
            if (sum <= 0)
                return -1;

            var take = rand.NextDouble() * sum;

            for (int i = 0; i < elements.Count; i++)
            {
                take -= weight(elements[i]);
                if (take <= 0)
                    return i;
            }
            return -1;
        }
        public static int ChooseTransition(this double[,] transitionMatrix, int startingLabel, Random rand)
        {
            // compute sum
            double sum = 0;
            for (int i = 0; i < transitionMatrix.GetLength(1); i++)
            {
                sum += transitionMatrix[startingLabel, i];
            }

            if (sum <= 0)
                return -1;

            var take = rand.NextDouble() * sum;

            for (int i = 0; i < transitionMatrix.GetLength(1); i++)
            {
                take -= transitionMatrix[startingLabel, i];
                if (take <= 0)
                    return i;
            }
            return -1;
        }

        public static T RandomElement<T>(this LinkedList<T> elements, Random rand)
        {
            int count = elements.Count;
            int index = rand.Next(count);
            var element = elements.First;
            //if (index <= (double)count / 2)
            //{
            for (int i = 0; i < index; i++)
            {
                element = element.Next;
            }
            //}
            //else
            //{
            //    for (int i = 0; i < count - index; i++)
            //    {
            //        element = element.Previous;
            //    }
            //}
            return element.Value;
        }
        public static LinkedList<T> RandomizeOrder<T>(this IEnumerable<T> elements)
        {
            var rand = new Random();
            var ll = new LinkedList<T>();
            var list = elements.ToList();

            while (list.Count > 0)
            {
                var element = list[rand.Next(list.Count)];
                list.Remove(element);
                ll.AddLast(element);
            }

            return ll;
        }
        public static void LinkedListInit<T>(this IList<ICollection<T>> intervals)
        {
            for (int i = 0; i < intervals.Count; i++)
            {
                intervals[i] = new LinkedList<T>();
            }
        }
        public static void InsertByValue<T>(this IList<ICollection<T>> intervals, T element, double value)
        {
            if (value == 1.0)
            {
                intervals[intervals.Count() - 1].Add(element);
            }
            else
            {
                int interval = (int)(value * intervals.Count);
                intervals[interval].Add(element);
            }
        }
        public static void CountByValue<T>(this IList<int> intervals, double value)
        {
            if (value == 1.0)
            {
                intervals[intervals.Count() - 1]++;
            }
            else
            {
                int interval = (int)(value * intervals.Count);
                intervals[interval]++;
            }
        }
        public static T RandomElement<T>(this IList<T> list, Random random)
        {
            return list[random.Next(list.Count)];
        }
        public static IEnumerable<T> RandomTake<T>(this IList<T> list, int numberToTake, Random random)
        {
            for (int i = 0; i < numberToTake; i++)
            {
                yield return list[random.Next(list.Count)];
            }
        }
        public static IEnumerable<T> TakeEachByChance<T>(this IEnumerable<T> list, double chance, Random random)
        {
            foreach (var item in list)
            {
                if (random.NextDouble() <= chance)
                    yield return item;
            }
        }

        public static IEnumerable<T>[] DivideByScoreAequidistant<T>(this IEnumerable<T> collection, Func<T, double> func, double ScoreMax, int intervals)
        {
            var intervalCol = new LinkedList<T>[intervals];
            for (int interval = 0; interval < intervals; interval++)
            {
                intervalCol[interval] = new LinkedList<T>();
            }
            foreach (var item in collection)
            {
                if (func(item) >= ScoreMax)
                {
                    intervalCol[intervals - 1].AddLast(item);
                }
                else if (func(item) < 0)
                {
                    intervalCol[0].AddLast(item);
                }
                else
                    intervalCol[Math.Min(intervalCol.Length - 1, (int)(func(item) * (double)intervals / ScoreMax))].AddLast(item);
            }
            return intervalCol;
        }
        public static IEnumerable<T>[] DivideByScoreAequidistant<T>(this IEnumerable<T> collection, Func<T, double> func, int intervals)
        {
            var min = collection.Min(t => func(t));
            var max = collection.Max(t => func(t));
            var length = max - min;

            var intervalCol = new LinkedList<T>[intervals];
            for (int interval = 0; interval < intervals; interval++)
            {
                intervalCol[interval] = new LinkedList<T>();
            }
            foreach (var item in collection)
            {
                if (func(item) >= max)
                {
                    intervalCol[intervals - 1].AddLast(item);
                }

                else
                    intervalCol[Math.Min(intervalCol.Length - 1, (int)((func(item) - min) * (double)intervals / length))].AddLast(item);
            }
            return intervalCol;
        }

        public static IEnumerable<T> Take<T>(this IList<T> list, int from, int to)
        {
            return list.Skip(from).Take(to - from);
        }

        public static IEnumerable<T>[] SplitToIntervals<T>(this IList<T> collection, int intervals)
        {
            var dividedCol = new IEnumerable<T>[intervals];

            var count = collection.Count();
            int from = 0;
            int to = 0;
            for (int interval = 0; interval < intervals; interval++)
            {
                from = to;
                to = (int)((((double)interval + 1) / intervals) * count + 0.5);
                dividedCol[interval] = collection.Take(from, to);
            }

            return dividedCol;
        }

        public static IEnumerable<ReturnType> Repeat<ReturnType>(this int times, Func<ReturnType> func)
        {
            for (int run = 0; run < times; run++)
            {
                yield return func();
            }
        }
        public static void Repeat(this int times, Action func)
        {
            for (int run = 0; run < times; run++)
            {
                func();
            }
        }
        public static void Repeat<ParameterType>(this int times, Action<ParameterType> func, ParameterType parameter)
        {
            for (int run = 0; run < times; run++)
            {
                func(parameter);
            }
        }
        public static IEnumerable<ReturnType> Repeat<ParameterType, ReturnType>(this int times, Func<ParameterType, ReturnType> func, ParameterType parameter)
        {
            for (int run = 0; run < times; run++)
            {
                yield return func(parameter);
            }
        }
        public static void Repeat<ParameterType1, ParameterType2>(this int times, Action<ParameterType1, ParameterType2> func, ParameterType1 parameter1, ParameterType2 parameter2)
        {
            for (int run = 0; run < times; run++)
            {
                func(parameter1, parameter2);
            }
        }
        public static IEnumerable<ReturnType> Repeat<ParameterType1, ParameterType2, ReturnType>(this int times, Func<ParameterType1, ParameterType2, ReturnType> func, ParameterType1 parameter1, ParameterType2 parameter2)
        {
            for (int run = 0; run < times; run++)
            {
                yield return func(parameter1, parameter2);
            }
        }
        public static IEnumerable<ReturnType> Repeat<ParameterType1, ParameterType2, ParameterType3, ReturnType>(this int times, Func<ParameterType1, ParameterType2, ParameterType3, ReturnType> func, ParameterType1 parameter1, ParameterType2 parameter2, ParameterType3 parameter3)
        {
            for (int run = 0; run < times; run++)
            {
                yield return func(parameter1, parameter2, parameter3);
            }
        }

        public static void Each<Type>(this IEnumerable<Type> elements, Action<Type> action)
        {
            foreach (var item in elements)
            {
                action(item);
            }
        }

        public static Random Rand = new Random();
        public static Type ChooseNormalized<Type>(this IEnumerable<Type> elements, Func<Type, double> probability, double totalProb = 1.0)
        {
            double choice = Rand.NextDouble() * totalProb;
            Type value = default(Type);
            foreach (var entry in elements)
            {
                value = entry;
                choice -= probability(entry);
                if (choice <= 0)
                    return value;
            }

            //check if total prob is correct:
            var checkTotalProb = elements.Sum(el => probability(el));

            if (checkTotalProb != totalProb)
                return elements.ChooseNormalized<Type>(probability, checkTotalProb);
            else
            {
                Log.Post("error chosing a random element.", LogCategory.Critical);
                return default(Type);
            }
        }



        public static KeyValuePair<Type, CompareType> MaxEntry<Type, CompareType>(this IEnumerable<KeyValuePair<Type, CompareType>> elements)
            where CompareType : IComparable<CompareType>
        {
            var maxEl = default(KeyValuePair<Type, CompareType>);
            foreach (var entry in elements)
            {
                if (maxEl.Equals(default(KeyValuePair<Type, CompareType>)))
                    maxEl = entry;
                else
                    maxEl = (maxEl.Value.CompareTo(entry.Value) >= 0) ? maxEl : entry;
            }
            return maxEl;
        }

        public static Type MaxEntry<Type, CompareType>(this IEnumerable<Type> elements, Func<Type, CompareType> keySelector)
            where CompareType : IComparable<CompareType>
        {
            var maxEl = default(Type);
            var maxKey = default(CompareType);
            foreach (var entry in elements)
            {
                if (maxEl == null)
                {
                    maxEl = entry;
                    maxKey = keySelector(entry);
                }
                else
                {
                    var entryKey = keySelector(entry);
                    if (maxKey.CompareTo(entryKey) < 0)
                    {
                        maxEl = entry;
                        maxKey = entryKey;
                    }
                }
            }
            return maxEl;
        }

        public static LinkedList<Type> MaxEntries<Type>(this IEnumerable<Type> elements, Func<Type, double> keySelector, int count)
        {
            var maxEls = new LinkedList<Type>();
            foreach (var entry in elements)
            {
                maxEls.SortedInsert(entry, count, keySelector);
            }
            return maxEls;
        }
        //public static LinkedList<Type> MaxEntries<Type>(this IEnumerable<Type> elements, Func<Type, double> keySelector)
        //{
        //    var maxEl = default(Type);
        //    foreach (var entry in elements)
        //    {
        //        maxEls.SortedInsert(entry, count, keySelector);
        //    }
        //    return maxEls;
        //}
        public static void MergeValues<Type>(this IList<Type> list1, IList<Type> list2, Func<Type, Type, Type> mergeFunction)
        {
            if (list1.Count != list2.Count)
                throw new ArgumentException("Cannot Merge two lists with different counts.");
            for (int i = 0; i < list1.Count; i++)
            {
                list1[i] = mergeFunction(list1[i], list2[i]);
            }
        }

        public static Dictionary<Keytype, int> RemoveFromEntryNewDict<Keytype>(this Dictionary<Keytype, int> dict, Keytype key, int many)
        {
            var dictNew = new Dictionary<Keytype, int>();

            foreach (var entry in dict)
            {
                dictNew.Add(entry.Key, entry.Value);
            }
            dictNew[key] -= many;

            return dictNew;
        }

        public static void Remove<T>(this ICollection<T> elements, IEnumerable<T> elementsToRemove)
        {
            foreach (var item in elementsToRemove)
            {
                elements.Remove(item);
            }
        }
        public static ICollection<T> RemoveWhere<T>(this ICollection<T> elements, Func<T, bool> criteria)
        {
            var temp = new LinkedList<T>();
            foreach (var item in elements)
            {
                if (criteria(item))
                    temp.AddLast(item);
            }
            foreach (var item in temp)
            {
                elements.Remove(item);
            }
            return elements;
        }
        public static LinkedList<T> NewLinkedList<T>(params T[] data)
        {
            var ll = new LinkedList<T>();
            foreach (var item in data)
            {
                ll.AddLast(item);
            }
            return ll;
        }
        public static IEnumerable<Type> ToIEnumerable<Type>(this Type element, params Type[] elements)
        {
            var list = new LinkedList<Type>();
            list.AddFirst(element);
            if (elements != null)
                list.AddRange(elements);
            return list;
        }

        public static List<IEnumerable<T>> SplitAndOrder<T, SplitKey>(this IEnumerable<T> elements, Func<T, SplitKey> mapper)
            where SplitKey : IComparable<SplitKey>
        {
            var returnlist = new LinkedList<IEnumerable<T>>();
            var ll = new Dictionary<SplitKey, LinkedList<T>>();
            var llnull = new LinkedList<T>();
            foreach (var item in elements)
            {
                var key = mapper(item);
                if (ll.ContainsKey(key))
                {
                    ll[key].AddLast(item);
                }
                else
                {
                    var list = new LinkedList<T>();
                    list.AddLast(item);
                    ll.Add(key, list);
                }
            }

            return ll.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).Cast<IEnumerable<T>>().ToList();
        }

        public static LinkedList<IEnumerable<T>> Split<T, SplitKey>(this IEnumerable<T> elements, Func<T, SplitKey> mapper)
        {
            var returnlist = new LinkedList<IEnumerable<T>>();
            var ll = new Dictionary<SplitKey, LinkedList<T>>();
            var llnull = new LinkedList<T>();
            foreach (var item in elements)
            {
                var key = mapper(item);
                if (key == null)
                {
                    llnull.AddLast(item);
                }
                else
                {
                    if (ll.ContainsKey(key))
                    {
                        ll[key].AddLast(item);
                    }
                    else
                    {
                        var list = new LinkedList<T>();
                        list.AddLast(item);
                        ll.Add(key, list);
                    }
                }
            }
            var temp = ll.Select<KeyValuePair<SplitKey, LinkedList<T>>, IEnumerable<T>>((kvp, i) => kvp.Value.Cast<T>());
            returnlist.AddRange(temp);
            if (llnull.Count > 0)
                returnlist.AddLast(llnull);
            return returnlist;
        }

        public static ElementType GetElementRelativePosition<ElementType>(this LinkedList<ElementType> list, double relativePosition)
        {
            if (relativePosition < 0 || relativePosition >= 1)
                throw new ArgumentException("Die relative Positionsabfrage einer List muss als Parameter eine Zahl >= 0 und < 1 enthalten");
            else
            {
                if (list == null || list.Count == 0)
                    return default(ElementType);
                else
                {
                    int index = (int)(relativePosition * list.Count);
                    var node = list.First;
                    while (index > 0)
                    {
                        node = node.Next;
                        index--;
                    }
                    return node.Value;
                }
            }
        }
        public static ElementType GetElementRelativePosition<ElementType>(this IList<ElementType> list, double relativePosition)
        {
            if (relativePosition < 0 || relativePosition >= 1)
                throw new ArgumentException("Die relative Positionsabfrage einer List muss als Parameter eine Zahl >= 0 und < 1 enthalten");
            else
            {
                if (list == null || list.Count == 0)
                    return default(ElementType);
                return list[(int)(relativePosition * list.Count)];
            }
        }
        public static void Add<T>(this LinkedList<T> ll, T element)
        {
            ll.AddLast(element);
        }
        public static ICollection<T> AddRange<T>(this ICollection<T> collection, params IEnumerable<T>[] enumerables)
        {
            foreach (var enumerable in enumerables)
            {
                foreach (var item in enumerable)
                {
                    collection.Add(item);
                }
            }
            return collection;
        }
        public static void AddRange<T>(this ICollection<T> collection, params T[] enumerable)
        {
            foreach (var item in enumerable)
            {
                collection.Add(item);
            }
        }
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                collection.Add(item);
            }
        }
        public static void AddRange<T>(this T[] collection, params T[] enumerable)
        {
            int counter = 0;
            int max = enumerable.Count();
            foreach (var item in enumerable)
            {
                if (counter >= max)
                    break;
                collection[counter] = (item);
                counter++;
            }
        }
        public static LinkedList<T> NewLinkedList<T>(this IEnumerable<T> col, params T[] data)
        {
            var ll = new LinkedList<T>();
            foreach (var item in col)
            {
                ll.AddLast(item);
            }
            foreach (var item in data)
            {
                ll.AddLast(item);
            }
            return ll;
        }
        public static T GetParameter<T>(this Pair<T>[] pairArray, T parameterKey)
        {
            return pairArray.FirstOrDefault(pair => pair.First.Equals(parameterKey)).Second;
        }

        public static double AggregatedScore<T>(this IDictionary<T, double> scoringFunction, IEnumerable<T> elements)
        {
            double score = 0;
            foreach (var item in elements)
            {
                score += scoringFunction[item];
            }
            return score;
        }

        public static IEnumerable<T> UnsafeCast<T>(this IEnumerable source)
        {
            foreach (var src in source)
            {
                yield return ((T)src);
            }
        }


        public static IEnumerable<BaseType> MergeToIEnumerable<BaseType, Type1, Type2>(this AgO<ICollection<Type1>, ICollection<Type2>> aggregatedObject)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
        {
            var colBaseType = new LinkedList<BaseType>();

            colBaseType.AddRange(aggregatedObject.Data1.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data2.Cast<BaseType>());

            return colBaseType;
        }

        public static IEnumerable<BaseType> MergeToIEnumerable<BaseType, Type1, Type2, Type3>(this AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>> aggregatedObject)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
        {
            var colBaseType = new LinkedList<BaseType>();

            colBaseType.AddRange(aggregatedObject.Data1.Cast<BaseType>(), aggregatedObject.Data2.Cast<BaseType>(), aggregatedObject.Data3.Cast<BaseType>());

            return colBaseType;
        }

        public static IEnumerable<BaseType> MergeToIEnumerable<BaseType, Type1, Type2, Type3, Type4>(this AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>> aggregatedObject)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
        {
            var colBaseType = new LinkedList<BaseType>();

            colBaseType.AddRange(aggregatedObject.Data1.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data2.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data3.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data4.Cast<BaseType>());

            return colBaseType;
        }


        public static IEnumerable<BaseType> MergeToIEnumerable<BaseType, Type1, Type2, Type3, Type4, Type5>(this AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>> aggregatedObject)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
        {
            var colBaseType = new LinkedList<BaseType>();

            colBaseType.AddRange(aggregatedObject.Data1.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data2.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data3.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data4.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data5.Cast<BaseType>());

            return colBaseType;
        }

        public static bool NotNullOrEmpty<BaseType>(this IEnumerable<BaseType> collection)
        {
            if (collection == null)
                return false;

            if (collection.Any())
                return true;

            return false;
        }
        public static bool NullOrEmpty<BaseType>(this IEnumerable<BaseType> collection)
        {
            return !collection.NotNullOrEmpty();
        }

        public static IEnumerable<BaseType> MergeToIEnumerable<BaseType, Type1, Type2, Type3, Type4, Type5, Type6>(this AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>> aggregatedObject)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
            where Type6 : class, BaseType
        {
            var colBaseType = new LinkedList<BaseType>();

            colBaseType.AddRange(aggregatedObject.Data1.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data2.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data3.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data4.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data5.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data6.Cast<BaseType>());

            return colBaseType;
        }

        public static IEnumerable<BaseType> MergeToIEnumerable<BaseType, Type1, Type2, Type3, Type4, Type5, Type6, Type7>(this AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>, ICollection<Type7>> aggregatedObject)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
            where Type6 : class, BaseType
            where Type7 : class, BaseType
        {
            var colBaseType = new LinkedList<BaseType>();

            colBaseType.AddRange(aggregatedObject.Data1.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data2.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data3.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data4.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data5.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data6.Cast<BaseType>());
            colBaseType.AddRange(aggregatedObject.Data7.Cast<BaseType>());

            return colBaseType;
        }

        //public static DefaultSubTypeCollection ToDefaultSubTypeCollection(this IEnumerable<IMITObject> collectionToSplit)
        //{
        //    var subtypeCollection = new DefaultSubTypeCollection();

        //    collectionToSplit.Each(
        //    (obj) =>
        //    {
        //        if (obj is TourLight)
        //        {
        //            subtypeCollection.Tours.AddLast((TourLight)obj);
        //        }
        //        if (obj is OrderLight)
        //        {
        //            subtypeCollection.Orders.AddLast((OrderLight)obj);
        //        }
        //        if (obj is TransportOrderLight)
        //        {
        //            subtypeCollection.TransportOrders.AddLast((TransportOrderLight)obj);
        //        }
        //        if (obj is CargoLight)
        //        {
        //            subtypeCollection.Cargo.AddLast((CargoLight)obj);
        //        }
        //        if (obj is CategoryLight)
        //        {
        //            subtypeCollection.Categories.AddLast((CategoryLight)obj);
        //        }
        //        if (obj is PackageProfile)
        //        {
        //            subtypeCollection.PackageProfiles.AddLast((PackageProfile)obj);
        //        }
        //        if (obj is DistributionEntry)
        //        {
        //            subtypeCollection.DistributionEntries.AddLast((DistributionEntry)obj);
        //        }
        //        if (obj is MITTourSection)
        //        {
        //            subtypeCollection.TourSections.AddLast((MITTourSection)obj);
        //        }
        //        if (obj is MITTourStop)
        //        {
        //            subtypeCollection.Tourstops.AddLast((MITTourStop)obj);
        //        }
        //    });
        //    return subtypeCollection;
        //}



        public static AgO<ICollection<Type1>, ICollection<Type2>> SplitToSubtypes<BaseType, Type1, Type2>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();

            Parallel.ForEach<BaseType>(collectionToSplit,
             (item, m, k) =>
             {
                 var asType1 = item as Type1;
                 if (asType1 != null)
                 {
                     lock (colType1)
                     {
                         colType1.AddLast(asType1);
                     }
                 }
                 else
                 {
                     var asType2 = item as Type2;
                     if (asType2 != null)
                     {
                         lock (colType2)
                         {
                             colType2.AddLast(asType2);
                         }
                     }
                 }
             }

         );
            return new AgO<ICollection<Type1>, ICollection<Type2>>(colType1, colType2);
        }

        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>> SplitToSubtypes<BaseType, Type1, Type2, Type3>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();

            Parallel.ForEach<BaseType>(collectionToSplit,
             (item, m, k) =>
             {
                 var asType1 = item as Type1;
                 if (asType1 != null)
                 {
                     lock (colType1)
                     {
                         colType1.AddLast(asType1);
                     }
                 }
                 else
                 {
                     var asType2 = item as Type2;
                     if (asType2 != null)
                     {
                         lock (colType2)
                         {
                             colType2.AddLast(asType2);
                         }
                     }
                     else
                     {
                         var asType3 = item as Type3;
                         if (asType3 != null)
                         {
                             lock (colType3)
                             {
                                 colType3.AddLast(asType3);
                             }
                         }
                     }
                 }
             }

         );
            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>>(colType1, colType2, colType3);
        }

        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>> SplitToSubtypes<BaseType, Type1, Type2, Type3, Type4>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();
            var colType4 = new LinkedList<Type4>();

            Parallel.ForEach<BaseType>(collectionToSplit,
             (item, m, k) =>
             {
                 var asType1 = item as Type1;
                 if (asType1 != null)
                 {
                     lock (colType1)
                     {
                         colType1.AddLast(asType1);
                     }
                 }
                 else
                 {
                     var asType2 = item as Type2;
                     if (asType2 != null)
                     {
                         lock (colType2)
                         {
                             colType2.AddLast(asType2);
                         }
                     }
                     else
                     {
                         var asType3 = item as Type3;
                         if (asType3 != null)
                         {
                             lock (colType3)
                             {
                                 colType3.AddLast(asType3);
                             }
                         }
                         else
                         {
                             var asType4 = item as Type4;
                             if (asType4 != null)
                             {
                                 lock (colType4)
                                 {
                                     colType4.AddLast(asType4);
                                 }
                             }
                         }
                     }
                 }
             }

         );
            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>>(colType1, colType2, colType3, colType4);
        }



        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>> SplitToSubtypes<BaseType, Type1, Type2, Type3, Type4, Type5>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();
            var colType4 = new LinkedList<Type4>();
            var colType5 = new LinkedList<Type5>();


            Parallel.ForEach<BaseType>(collectionToSplit,
             (item, m, k) =>
             {
                 var asType1 = item as Type1;
                 if (asType1 != null)
                 {
                     lock (colType1)
                     {
                         colType1.AddLast(asType1);
                     }
                 }
                 else
                 {
                     var asType2 = item as Type2;
                     if (asType2 != null)
                     {
                         lock (colType2)
                         {
                             colType2.AddLast(asType2);
                         }
                     }
                     else
                     {
                         var asType3 = item as Type3;
                         if (asType3 != null)
                         {
                             lock (colType3)
                             {
                                 colType3.AddLast(asType3);
                             }
                         }
                         else
                         {
                             var asType4 = item as Type4;
                             if (asType4 != null)
                             {
                                 lock (colType4)
                                 {
                                     colType4.AddLast(asType4);
                                 }
                             }
                             else
                             {
                                 var asType5 = item as Type5;
                                 if (asType5 != null)
                                 {
                                     lock (colType5)
                                     {
                                         colType5.AddLast(asType5);
                                     }
                                 }
                             }
                         }
                     }
                 }
             }

         );
            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>>(colType1, colType2, colType3, colType4, colType5);
        }



        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>> SplitToSubtypes<BaseType, Type1, Type2, Type3, Type4, Type5, Type6>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
            where Type6 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();
            var colType4 = new LinkedList<Type4>();
            var colType5 = new LinkedList<Type5>();
            var colType6 = new LinkedList<Type6>();


            Parallel.ForEach<BaseType>(collectionToSplit,
             (item, m, k) =>
             {
                 var asType1 = item as Type1;
                 if (asType1 != null)
                 {
                     lock (colType1)
                     {
                         colType1.AddLast(asType1);
                     }
                 }
                 else
                 {
                     var asType2 = item as Type2;
                     if (asType2 != null)
                     {
                         lock (colType2)
                         {
                             colType2.AddLast(asType2);
                         }
                     }
                     else
                     {
                         var asType3 = item as Type3;
                         if (asType3 != null)
                         {
                             lock (colType3)
                             {
                                 colType3.AddLast(asType3);
                             }
                         }
                         else
                         {
                             var asType4 = item as Type4;
                             if (asType4 != null)
                             {
                                 lock (colType4)
                                 {
                                     colType4.AddLast(asType4);
                                 }
                             }
                             else
                             {
                                 var asType5 = item as Type5;
                                 if (asType5 != null)
                                 {
                                     lock (colType5)
                                     {
                                         colType5.AddLast(asType5);
                                     }
                                 }
                                 else
                                 {
                                     var asType6 = item as Type6;
                                     if (asType6 != null)
                                     {
                                         lock (colType6)
                                         {
                                             colType6.AddLast(asType6);
                                         }
                                     }
                                 }
                             }
                         }
                     }
                 }
             }

         );
            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>>(colType1, colType2, colType3, colType4, colType5, colType6);
        }


        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>, ICollection<Type7>> SplitToSubtypes<BaseType, Type1, Type2, Type3, Type4, Type5, Type6, Type7>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
            where Type6 : class, BaseType
            where Type7 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();
            var colType4 = new LinkedList<Type4>();
            var colType5 = new LinkedList<Type5>();
            var colType6 = new LinkedList<Type6>();
            var colType7 = new LinkedList<Type7>();


            Parallel.ForEach<BaseType>(collectionToSplit,
             (item, m, k) =>
             {
                 var asType1 = item as Type1;
                 if (asType1 != null)
                 {
                     lock (colType1)
                     {
                         colType1.AddLast(asType1);
                     }
                 }
                 else
                 {
                     var asType2 = item as Type2;
                     if (asType2 != null)
                     {
                         lock (colType2)
                         {
                             colType2.AddLast(asType2);
                         }
                     }
                     else
                     {
                         var asType3 = item as Type3;
                         if (asType3 != null)
                         {
                             lock (colType3)
                             {
                                 colType3.AddLast(asType3);
                             }
                         }
                         else
                         {
                             var asType4 = item as Type4;
                             if (asType4 != null)
                             {
                                 lock (colType4)
                                 {
                                     colType4.AddLast(asType4);
                                 }
                             }
                             else
                             {
                                 var asType5 = item as Type5;
                                 if (asType5 != null)
                                 {
                                     lock (colType5)
                                     {
                                         colType5.AddLast(asType5);
                                     }
                                 }
                                 else
                                 {
                                     var asType6 = item as Type6;
                                     if (asType6 != null)
                                     {
                                         lock (colType6)
                                         {
                                             colType6.AddLast(asType6);
                                         }
                                     }
                                     else
                                     {
                                         var asType7 = item as Type7;
                                         if (asType7 != null)
                                         {
                                             lock (colType7)
                                             {
                                                 colType7.AddLast(asType7);
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                     }
                 }
             }

         );
            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>, ICollection<Type7>>(colType1, colType2, colType3, colType4, colType5, colType6, colType7);
        }

        #region Android

        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>> SplitToSubtypesAndroid<BaseType, Type1, Type2, Type3, Type4, Type5>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();
            var colType4 = new LinkedList<Type4>();
            var colType5 = new LinkedList<Type5>();


            foreach (var item in collectionToSplit)
            {

                var asType1 = item as Type1;
                if (asType1 != null)
                {
                    lock (colType1)
                    {
                        colType1.AddLast(asType1);
                    }
                }
                else
                {
                    var asType2 = item as Type2;
                    if (asType2 != null)
                    {
                        lock (colType2)
                        {
                            colType2.AddLast(asType2);
                        }
                    }
                    else
                    {
                        var asType3 = item as Type3;
                        if (asType3 != null)
                        {
                            lock (colType3)
                            {
                                colType3.AddLast(asType3);
                            }
                        }
                        else
                        {
                            var asType4 = item as Type4;
                            if (asType4 != null)
                            {
                                lock (colType4)
                                {
                                    colType4.AddLast(asType4);
                                }
                            }
                            else
                            {
                                var asType5 = item as Type5;
                                if (asType5 != null)
                                {
                                    lock (colType5)
                                    {
                                        colType5.AddLast(asType5);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>>(colType1, colType2, colType3, colType4, colType5);
        }



        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>> SplitToSubtypesAndroid<BaseType, Type1, Type2, Type3, Type4, Type5, Type6>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
            where Type6 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();
            var colType4 = new LinkedList<Type4>();
            var colType5 = new LinkedList<Type5>();
            var colType6 = new LinkedList<Type6>();


            foreach (var item in collectionToSplit)
            {
                var asType1 = item as Type1;
                if (asType1 != null)
                {
                    lock (colType1)
                    {
                        colType1.AddLast(asType1);
                    }
                }
                else
                {
                    var asType2 = item as Type2;
                    if (asType2 != null)
                    {
                        lock (colType2)
                        {
                            colType2.AddLast(asType2);
                        }
                    }
                    else
                    {
                        var asType3 = item as Type3;
                        if (asType3 != null)
                        {
                            lock (colType3)
                            {
                                colType3.AddLast(asType3);
                            }
                        }
                        else
                        {
                            var asType4 = item as Type4;
                            if (asType4 != null)
                            {
                                lock (colType4)
                                {
                                    colType4.AddLast(asType4);
                                }
                            }
                            else
                            {
                                var asType5 = item as Type5;
                                if (asType5 != null)
                                {
                                    lock (colType5)
                                    {
                                        colType5.AddLast(asType5);
                                    }
                                }
                                else
                                {
                                    var asType6 = item as Type6;
                                    if (asType6 != null)
                                    {
                                        lock (colType6)
                                        {
                                            colType6.AddLast(asType6);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>>(colType1, colType2, colType3, colType4, colType5, colType6);
        }


        public static AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>, ICollection<Type7>> SplitToSubtypesAndroid<BaseType, Type1, Type2, Type3, Type4, Type5, Type6, Type7>(this IEnumerable<BaseType> collectionToSplit)
            where Type1 : class, BaseType
            where Type2 : class, BaseType
            where Type3 : class, BaseType
            where Type4 : class, BaseType
            where Type5 : class, BaseType
            where Type6 : class, BaseType
            where Type7 : class, BaseType
        {
            var colType1 = new LinkedList<Type1>();
            var colType2 = new LinkedList<Type2>();
            var colType3 = new LinkedList<Type3>();
            var colType4 = new LinkedList<Type4>();
            var colType5 = new LinkedList<Type5>();
            var colType6 = new LinkedList<Type6>();
            var colType7 = new LinkedList<Type7>();


            foreach (var item in collectionToSplit)
            {
                var asType1 = item as Type1;
                if (asType1 != null)
                {
                    lock (colType1)
                    {
                        colType1.AddLast(asType1);
                    }
                }
                else
                {
                    var asType2 = item as Type2;
                    if (asType2 != null)
                    {
                        lock (colType2)
                        {
                            colType2.AddLast(asType2);
                        }
                    }
                    else
                    {
                        var asType3 = item as Type3;
                        if (asType3 != null)
                        {
                            lock (colType3)
                            {
                                colType3.AddLast(asType3);
                            }
                        }
                        else
                        {
                            var asType4 = item as Type4;
                            if (asType4 != null)
                            {
                                lock (colType4)
                                {
                                    colType4.AddLast(asType4);
                                }
                            }
                            else
                            {
                                var asType5 = item as Type5;
                                if (asType5 != null)
                                {
                                    lock (colType5)
                                    {
                                        colType5.AddLast(asType5);
                                    }
                                }
                                else
                                {
                                    var asType6 = item as Type6;
                                    if (asType6 != null)
                                    {
                                        lock (colType6)
                                        {
                                            colType6.AddLast(asType6);
                                        }
                                    }
                                    else
                                    {
                                        var asType7 = item as Type7;
                                        if (asType7 != null)
                                        {
                                            lock (colType7)
                                            {
                                                colType7.AddLast(asType7);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new AgO<ICollection<Type1>, ICollection<Type2>, ICollection<Type3>, ICollection<Type4>, ICollection<Type5>, ICollection<Type6>, ICollection<Type7>>(colType1, colType2, colType3, colType4, colType5, colType6, colType7);
        }

        #endregion


        public static void InitLists<T>(this IList<T>[] listsArray)
        {
            for (int i = 0; i < listsArray.Length; i++)
            {
                listsArray[i] = new List<T>();
            }
        }
    }
}
