using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public interface Startable
    {
        StartableState State { get;}
        void Start();
        void Stop();
    }
    public enum StartableState
    {
        Offline, 
        Online,
        Running,
        CancelRequested
    }

    public class GWAgent
    {
        public int Id { get; set; }
        public MEvent<StartableMessage> Message { get; } = new MEvent<StartableMessage>();
        public void SendMessage(StartableMessage message)
        {
            Message.Enter(message);
        }
    }

    public enum StartableMessage
    {
        Start,
        Stop
    }
}
