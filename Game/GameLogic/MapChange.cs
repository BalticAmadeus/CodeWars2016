using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class MapChange
    {
        public Point Position;
        public TileType Value;

        public MapChange()
        {
            // default
        }

        public MapChange(Point position, TileType value)
        {
            Position = position;
            Value = value;
        }

        public MapChange(int row, int col, TileType value)
        {
            Position = new Point(row, col);
            Value = value;
        }
    }
}
