using System.Collections.Generic;

namespace Game.AdminClient.Models
{
    public class Map
    {
        public Map()
        {
            Rows = new List<string>();
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public IList<string> Rows { get; set; }
        public Point TecmanPosition { get; set; }
        public Point[] GhostPositions { get; set; }
    }
}