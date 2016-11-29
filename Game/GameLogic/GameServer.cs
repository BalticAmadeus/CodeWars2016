using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class GameServer
    {
        private static GameServer _instance;
        private static object _instanceLock = new object();

        public static GameServer Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new GameServer();
                    return _instance;
                }
            }
        }

        public TeamRegistry TeamRegistry { get; private set; }
        public SessionManager SessionManager { get; private set; }
        public GameManager GameManager { get; private set; }
        public IAccessManager AccessManager { get; private set; }

        public GameServer()
        {
            Settings.Load();
            TeamRegistry = new TeamRegistry();
            TeamRegistry.Load();
            SessionManager = new SessionManager(TeamRegistry);
            GameManager = new GameManager();
            if (Settings.IsProductionMode)
                AccessManager = new OpenAccessManager();
            else
                AccessManager = new AccessManager();
        }
    }
}
