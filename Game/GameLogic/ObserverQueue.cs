using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameLogic
{
    public class ObserverQueue
    {
        public Observer Observer { get; private set; }
        public bool IsLive { get; private set; }

        public int Count
        {
            get
            {
                if (_queue != null)
                    return _queue.Count;
                else
                    return 0;
            }
        }

        private Queue<ObservedTurnInfo> _queue;
        private object _lock;

        public ObserverQueue(Observer observer)
        {
            this.Observer = observer;
            IsLive = true;
            _queue = new Queue<ObservedTurnInfo>(Settings.MaxObserverQueue);
            _lock = new object();
        }

        public bool Push(ObservedTurnInfo turnInfo)
        {
            lock (_lock)
            {
                if (!IsLive)
                    return false;
                if (_queue.Count >= Settings.MaxObserverQueue)
                {
                    IsLive = false;
                    _queue = null;
                    return false;
                }
                _queue.Enqueue(turnInfo);
                if (_queue.Count == 1)
                    Monitor.PulseAll(_lock);
                return true;
            }
        }

        public ObservedTurnInfo Pop(out int queueLength)
        {
            queueLength = -1;
            lock (_lock)
            {
                if (!IsLive)
                    return null;
                if (_queue.Count == 0)
                    Monitor.Wait(_lock, Settings.ObserverPollTimeoutMillis);
                queueLength = _queue.Count;
                if (queueLength == 0)
                    return null;
                queueLength--;
                return _queue.Dequeue();
            }
        }
    }
}
