using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace CodeBase
{
    public class FileLogger : IRequestListener
    {

        #region Constructor

        public FileLogger(string directory)
        {
            LogEntryBuffer = new LinkedList<LogEntry>();
            Directory.CreateDirectory(directory);
            logDirectory = directory;
            timer = new Timer(TIMER_TICK);
            lockObject = new object();

            Register();
        }

        #endregion

        #region Properties

        private const int TIMER_TICK = 3000;

        public readonly LinkedList<LogEntry> LogEntryBuffer;
        private string logDirectory;
        private string logPath;
        private Timer timer;
        private object lockObject;
        public IGWContext Context { get; set; }

        #endregion

        #region Methods

        public void Register()
        {
            this.DoRegister<LogRequest>(NewLogRequest);

            BaseProgram.Exit.AddPath(WriteToFile);

            if (timer != null)
            {
                timer.Elapsed += timer_Elapsed;
                timer.AutoReset = true;
                timer.Start();
            }
        }

        public void Unregister()
        {
            this.DoUnregister<LogRequest>(NewLogRequest);

            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= timer_Elapsed;
                timer.Dispose();
            }

            WriteToFile();
        }

        public void WriteToFile()
        {
            try
            {
                lock (lockObject)
                {
                    logPath = logDirectory + "\\GW-LOG_" + DateTime.Now.ToString("dd-MM-yyyy") + ".log";

                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    using (StreamWriter writer = new StreamWriter(logPath, true, Encoding.UTF8))
                    {
                        LinkedList<LogEntry> temp = new LinkedList<LogEntry>(LogEntryBuffer);

                        foreach (LogEntry entry in temp)
                        {
                            string line = String.Format("[{0} {1}]: {2}", DateTime.Now.ToLongTimeString(), entry.Category, entry.Message);
                            writer.WriteLine(line);
                            LogEntryBuffer.Remove(entry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new NotifyUser("Writing Logfile produced an error: " + ex.Message).Request();
            }
        }

        private void NewLogRequest(LogRequest obj)
        {
            LogEntryBuffer.AddLast(new LogEntry(obj.Message, obj.Category));
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WriteToFile();
        }

        #endregion
    }

    public class ConsoleLoggerSafe : IRequestListener
    {

        #region Constructor

        public ConsoleLoggerSafe()
        {
            LogEntryBuffer = new LinkedList<LogEntry>();
            timer = new Timer(TIMER_TICK);
            lockObject = new object();

            Register();
        }

        #endregion

        #region Properties

        private const int TIMER_TICK = 1000;

        public readonly LinkedList<LogEntry> LogEntryBuffer;
        private Timer timer;
        private object lockObject;
        public IGWContext Context { get; set; }

        #endregion

        #region Methods

        public void Register()
        {
            this.DoRegister<LogRequest>(NewLogRequest);

            BaseProgram.Exit.AddPath(WriteToConsole);

            if (timer != null)
            {
                timer.Elapsed += timer_Elapsed;
                timer.AutoReset = true;
                timer.Start();
            }
        }

        public void Unregister()
        {
            this.DoUnregister<LogRequest>(NewLogRequest);

            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= timer_Elapsed;
                timer.Dispose();
            }

            WriteToConsole();
        }

        public void WriteToConsole()
        {
            try
            {
                lock (lockObject)
                {

                    LinkedList<LogEntry> temp = new LinkedList<LogEntry>(LogEntryBuffer);

                    foreach (LogEntry entry in temp)
                    {
                        string line = String.Format("[{0} {1}]: {2}", DateTime.Now.ToLongTimeString(), entry.Category, entry.Message);
                        Console.WriteLine(line);
                        LogEntryBuffer.Remove(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                new NotifyUser("Writing Logfile produced an error: " + ex.Message).Request();
            }
        }

        private void NewLogRequest(LogRequest obj)
        {
            LogEntryBuffer.AddLast(new LogEntry(obj.Message, obj.Category));
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WriteToConsole();
        }

        #endregion
    }
}
