using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface ICollectionPresentationLogic<DataType> : ILogic
    {
        void Init(IEnumerable<DataType> elements);

        void Next();

        void Previous();

        DataType Current { get; }
    }

    public class CollectionPresentationLogic<DataType> : ICollectionPresentationLogic<DataType>
    {
        private List<DataType> Elements = new List<DataType>();
        private int pointer = 0;

        public void Init(IEnumerable<DataType> elements)
        {
            Elements = (elements).ToList();
            if (Elements.NotNullOrEmpty())
                current = Elements[pointer];
        }

        public void Next()
        {
            pointer = (pointer + 1) % Elements.Count;
            if (Elements.NotNullOrEmpty())
                current = Elements[pointer];
        }

        public void Previous()
        {
            pointer--;
            if (pointer < 0)
                pointer = Math.Max(0, Elements.Count - 1);
            if (Elements.NotNullOrEmpty())
                current = Elements[pointer];
        }

        private DataType current;
        public DataType Current
        {
            get { return current; }
        }

        public Guid GWId
        {
            get;
            set;
        }
    }

    public static class CollectionClassificationX
    {
        public static void Init<DataType>(this  IHas<ICollectionPresentationLogic<DataType>> owner, IEnumerable<DataType> elements)
        {
            owner.Logic.Init(elements);
        }
        public static void Next<DataType>(this  IHas<ICollectionPresentationLogic<DataType>> owner)
        {
            owner.Logic.Next();
        }

        public static void Previous<DataType>(this  IHas<ICollectionPresentationLogic<DataType>> owner)
        {
            owner.Logic.Previous();
        }
        public static DataType Current<DataType>(this  IHas<ICollectionPresentationLogic<DataType>> owner)
        {
            return owner.Logic.Current;
        }
    }
}
