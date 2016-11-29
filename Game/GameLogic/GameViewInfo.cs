using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class GameViewInfo
    {
        public int PlayerIndex;
        public GameState GameState;
        public int Turn;
        public PlayerStateInfo[] PlayerStates;
        public MapData Map;
        public Point PreviousTecmanPosition;
        public Point[] PreviousGhostPosition;
    }
}
