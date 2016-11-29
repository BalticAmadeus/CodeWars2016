using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeManPoc
{
    public class Game
    {
        public int Turn;
        public MapPoint Pacman;
        public MapPoint CameFrom;
        public MapPoint[] Ghost;
        public MapPoint[] Trail;
        public int PointCount;

        public Game(GameMap map)
        {
            Pacman = map.Pacman;
            Ghost = map.Ghost.ToArray();
            Trail = map.Ghost.ToArray();
            for (int row = 0; row < map.Height; row++)
            {
                for (int col = 0; col < map.Width; col++)
                {
                    if (map[row, col] == MapTileType.POINT)
                        PointCount++;
                }
            }
        }
    }

    public class GameSight
    {
        public MapPoint Position;
        public int Distance;
        public MapTileType Obstacle;
    }

}
