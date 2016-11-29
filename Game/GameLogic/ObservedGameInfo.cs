using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class ObservedGameInfo
    {
        public int GameId;
        public GameState GameState;
        public int QueuedTurns;
        public ObservedTurnInfo TurnInfo;
        public List<int> SlowPlayers;
    }
}
