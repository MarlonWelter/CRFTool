using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class ObservableList<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private List<T> list = new List<T>();


        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            for (int i = index + 1; i < list.Count; i++)
            {
                this.OnCollectionChanged(NotifyCollectionChangedAction.Move, list[i], i, i - 1);
            }
        }

        public void RemoveAt(int index)
        {
            var element = list[index];
            list.RemoveAt(index);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, element, index);

            for (int i = index; i < list.Count; i++)
            {
                this.OnCollectionChanged(NotifyCollectionChangedAction.Move, list[i], i, i + 1);
            }
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public void Add(T item)
        {
            list.Add(item);

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, list.Count-1);
        }

        public void Clear()
        {
            list.Clear();
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            this.OnCollectionReset();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            var index = list.IndexOf(item);
            var returnVal = list.Remove(item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);

            for (int i = index; i < list.Count; i++)
            {
                this.OnCollectionChanged(NotifyCollectionChangedAction.Move, list[i], i, i + 1);
            }
            return returnVal;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (list as IEnumerable).GetEnumerator();
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }
        private void OnCollectionReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
