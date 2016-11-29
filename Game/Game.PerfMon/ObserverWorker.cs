using Game.PerfMon.AdminService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game.PerfMon
{
    class ObserverWorker
    {
        private Thread _thread;
        private List<LogEntry> _log;
        private DateTime _lastMessage;

        public ObserverWorker()
        {
            _thread = new Thread(ThreadRun);
            _thread.IsBackground = true;
        }

        public void Start()
        {
            _thread.Start();
        }

        private void ThreadRun()
        {
            _lastMessage = DateTime.Now;
            PushMessage("Worker starting");
            try
            {
                var client = new AdminServiceClient();

                int observerId = client.CreateObserver(new CreateObserverReq().Prepare()).ObserverId;
                PushMessage($"Created observer: {observerId}");

                while (true)
                {
                    Thread.Sleep(1000);

                    EnGameInfo[] games = client.ListGames(new ListGamesReq().Prepare()).Games;
                    EnGameInfo running = games.FirstOrDefault(g => g.State == "Play" || g.State == "Pause");
                    if (running == null)
                        continue;

                    PushMessage($"Connecting game {running.GameId} / {running.Label}");

                    string status = client.StartObserving(new StartObservingReq
                    {
                        ObserverId = observerId,
                        GameId = running.GameId
                    }.Prepare()).Status;

                    PushMessage($"StartObserving: {status}");

                    while (true)
                    {
                        var obs = client.ObserveNextTurn(new ObserveNextTurnReq
                        {
                            ObserverId = observerId,
                            GameId = running.GameId
                        }.Prepare());

                        PushMessage($"ObserveNextTurn: {obs.Status} {obs.TurnInfo?.Turn}");

                        if (obs.GameInfo.GameState == "Finish")
                            break;
                    }

                    PushMessage($"Game finished");
                }
            }
            catch (Exception ex)
            {
                PushMessage($"Exception {ex.GetType().FullName}: {ex.Message}");
            }
        }

        private void PushMessage(string message)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message
            };
            entry.Difference = entry.Timestamp - _lastMessage;
            _lastMessage = entry.Timestamp;
            lock (this)
            {
                if (_log == null)
                    _log = new List<LogEntry>();
                _log.Add(entry);
            }
        }

        public List<LogEntry> PullLog()
        {
            lock (this)
            {
                try
                {
                    return _log;
                }
                finally
                {
                    _log = null;
                }
            }
        }
    }
}
