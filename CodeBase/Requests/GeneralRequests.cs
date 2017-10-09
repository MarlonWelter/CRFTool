using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class Imported<T> : IHas<IRequestLogic<Imported<T>>>
    {
        public Imported(T item)
        {
            Item = item;
        }

        public T Item { get; set; }

        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        private RequestLogic<Imported<T>> logic = new RequestLogic<Imported<T>>();
        public IRequestLogic<Imported<T>> Logic
        {
            get { return logic; }
        }

    }

    public class Notify<T> : IHas<IRequestLogic<Notify<T>>>
    {
        public Notify(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        private RequestLogic<Notify<T>> logic = new RequestLogic<Notify<T>>();
        public IRequestLogic<Notify<T>> Logic
        {
            get { return logic; }
        }
    }

    public class DoExport<T> : IHas<IRequestLogic<DoExport<T>>>
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public DoExport(T element, string location)
        {
            Element = element;
            Location = location;
        }
        public DoExport(T element)
        {
            Element = element;
        }
        public T Element { get; set; }
        public string Location { get; set; }

        private RequestLogic<DoExport<T>> logic = new RequestLogic<DoExport<T>>();
        public IRequestLogic<DoExport<T>> Logic
        {
            get { return logic; }
        }

    }

    public class DoImport<T> : IHas<IRequestLogic<DoImport<T>>>
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public DoImport(string location)
        {
            Location = location;
        }
        public T Element { get; set; }
        public string Location { get; set; }

        private RequestLogic<DoImport<T>> logic = new RequestLogic<DoImport<T>>();
        public IRequestLogic<DoImport<T>> Logic
        {
            get { return logic; }
        }

    }

    public class ChooseLocation : IHas<IRequestLogic<ChooseLocation, string>>
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public ChooseLocation(ChooseLocationType type)
        {
            Type = type;
        }
        public ChooseLocation(ChooseLocationType type, IEnumerable<string> fileEndings)
        {
            Type = type;
            PossibleFileEndings = fileEndings;
        }
        public ChooseLocation(ChooseLocationType type, string defaultExtension)
        {
            Type = type;
            DefaultExtension = defaultExtension;
            PossibleFileEndings = new LinkedList<string>(defaultExtension.ToIEnumerable());
        }
        public string DefaultExtension { get; set; }
        public ChooseLocationType Type { get; set; }

        public IEnumerable<string> PossibleFileEndings { get; set; }

        private RequestLogic<ChooseLocation, string> logic = new RequestLogic<ChooseLocation, string>();
        public IRequestLogic<ChooseLocation, string> Logic
        {
            get { return logic; }
        }
    }
    public enum ChooseLocationType
    {
        Load,
        Save
    }

}
