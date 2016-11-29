using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class GameLiveInfo
    {
        public GameState GameState;
        public int Turn;
        public DateTime TurnStartTime;
        public PlayerLiveInfo[] PlayerStates;
    }
}
