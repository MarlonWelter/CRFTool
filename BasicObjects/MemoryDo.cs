
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class MemoryDo
    {
        private Dictionary<string, ICollection<Action>> actions = new Dictionary<string, ICollection<Action>>();

        public void Add(string trigger, Action action)
        {
            if (actions.ContainsKey(trigger))
            {
                actions[trigger].Add(action);
            }
            else
            {
                actions.Add(trigger, new LinkedList<Action>(action.ToIEnumerable()));
            }
        }
        public void Trigger(string trigger)
        {
            if (actions.ContainsKey(trigger))
            {
                foreach (var entry in actions[trigger])
                {
                    entry();
                }
                actions.Remove(trigger);
            }
            else
                Log.Post("Triggered non-existing trigger", LogCategory.Inconsistency);
        }
        public void Add<S>(string trigger, S parameter, Action<S> action)
        {
            if (actions.ContainsKey(trigger))
            {
                actions[trigger].Add(() => action(parameter));
            }
            else
            {
                var ll = new LinkedList<Action>();
                actions.Add(trigger, ll);
                ll.Add(() => action(parameter));
            }
        }
        public void Add<S, T>(string trigger, S parameter, T parameter2, Action<S, T> action)
        {
            if (actions.ContainsKey(trigger))
            {
                actions[trigger].Add(() => action(parameter, parameter2));
            }
            else
            {
                var ll = new LinkedList<Action>();
                actions.Add(trigger, ll);
                ll.Add(() => action(parameter, parameter2));
            }
        }
    }
}
