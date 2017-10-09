
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime;



namespace CodeBase
{
    public class LogMonitorVM : INotifyPropertyChanged, IRequestListener
    {
        public IGWContext Context { get; set; }
        public const int DefaultBufferSize = 250;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Buffer<LogEntry> logBuffer = new Buffer<LogEntry>(DefaultBufferSize);
        public LinkedList<LogEntry> Logs { get { return logBuffer.NonRemovingGetAll(); } }

        public LinkedList<LogEntry> FilteredLogs
        {
            get
            {
                var filteredLogs = new LinkedList<LogEntry>();
                int count = 0;
                foreach (var item in Logs)
                {
                    if (showEntry(item))
                    {
                        filteredLogs.AddLast(item);
                        count++;
                        if (count > LogSize)
                            break;
                    }

                }
                return filteredLogs;
            }
        }

        private int logSize;
        public int LogSize
        {
            get { return logSize; }
            set
            {
                logSize = value;
                NotifyPropertyChanged("LogSize");
            }
        }


        private bool showEntry(LogEntry entry)
        {
            switch (entry.Category)
            {
                case LogCategory.Detail:
                    return ShowDetail;
                case LogCategory.Overview:
                    return ShowOverview;
                case LogCategory.Inconsistency:
                    return ShowInconsistency;
                case LogCategory.Critical:
                    return ShowCritical;
                case LogCategory.UserInput:
                    return ShowUserInput;
                case LogCategory.Technical:
                    return ShowTechnical;
                default:
                    return true;
            }
        }

        public IEnumerable<string> Categories { get { return new LinkedList<string>().NewLinkedList("None", LogCategory.Detail.AsString(), LogCategory.Overview.AsString(), LogCategory.Inconsistency.AsString(), LogCategory.Critical.AsString()); } }

        private LogCategory categoryFilter;

        public LogCategory CategoryFilter
        {
            get { return categoryFilter; }
            set
            {
                categoryFilter = value;
                NotifyPropertyChanged("CategoryFilter");
            }
        }

        private bool showDetail = true;
        public bool ShowDetail
        {
            get { return showDetail; }
            set
            {
                showDetail = value;
                NotifyPropertyChanged("ShowDetail");
            }
        }
        private bool showOverview = true;
        public bool ShowOverview
        {
            get { return showOverview; }
            set
            {
                showOverview = value;
                NotifyPropertyChanged("ShowOverview");
            }
        }
        private bool showInconsistency = true;
        public bool ShowInconsistency
        {
            get { return showInconsistency; }
            set
            {
                showInconsistency = value;
                NotifyPropertyChanged("ShowInconsistency");
            }
        }
        private bool showCritical = true;
        public bool ShowCritical
        {
            get { return showCritical; }
            set
            {
                showCritical = value;
                NotifyPropertyChanged("ShowCritical");
            }
        }
        private bool showUserInput = true;
        public bool ShowUserInput
        {
            get { return showUserInput; }
            set
            {
                showUserInput = value;
                NotifyPropertyChanged("ShowUserInput");
            }
        }
        private bool showTechnical = true;
        public bool ShowTechnical
        {
            get { return showTechnical; }
            set
            {
                showTechnical = value;
                NotifyPropertyChanged("ShowTechnical");
            }
        }

        private void NewLogRequest(LogRequest obj)
        {
            logBuffer.Add(new LogEntry(obj.Message, obj.Category));
        }


        public LogMonitorVM()
        {
            Register();
            logBuffer.ElementAdded += LogBuffer_ElementAdded;
        }

        void LogBuffer_ElementAdded(object sender, SimO<LogEntry> e)
        {
            NotifyPropertyChanged("NewEntry");
        }

        void LogChannel_NewLogEntry(object sender, SimO<LogEntry> e)
        {
            logBuffer.Add(e.Data);
        }

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public void Register()
        {
            this.DoRegister<LogRequest>(NewLogRequest);
        }

        public void Unregister()
        {
            this.DoUnregister<LogRequest>(NewLogRequest);
        }

    }
}
