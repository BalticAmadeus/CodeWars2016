using System.Collections.Generic;

namespace Game.AdminClient.Models
{
    public class Game
    {
        public Game()
        {
            Players = new List<Player>();    
        }

        public int GameId { get; set; }
        public string Label { get; set; }
        public string State { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public Map Map { get; set; }
    }
}