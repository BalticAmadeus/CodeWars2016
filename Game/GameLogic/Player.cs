using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class Player
    {
        public int PlayerId { get; private set; }
        public Team Team { get; private set; }
        public string Name { get; private set; }
        public GameModel Game { get; set; }

        public Player(int playerId, Team team, string name)
        {
            PlayerId = playerId;
            Team = team;
            Name = name;
        }
    }
}
