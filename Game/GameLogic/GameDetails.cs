using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class GameDetails
    {
        public int GameId;
        public string Label;
        public GameState State;
        public PlayerInfo[] Players;

        public GameDetails(GameModel game)
        {
            GameId = game.GameId;
            Label = game.Label;
            State = game.State;
            Players = game.ListPlayers().Select(p => new PlayerInfo(p)).ToArray();
        }
    }
}
