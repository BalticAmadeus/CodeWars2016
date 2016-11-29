using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class PlayerInfo
    {
        public int PlayerId;
        public Team Team;
        public string Name;
        public int? GameId;

        public PlayerInfo(Player p)
        {
            PlayerId = p.PlayerId;
            Team = p.Team;
            Name = p.Name;
            GameModel game = p.Game;
            if (game != null)
                GameId = game.GameId;
        }
    }
}
