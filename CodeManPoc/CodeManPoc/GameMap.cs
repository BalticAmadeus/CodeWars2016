using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeManPoc
{
    public enum MapTileType
    {
        EMPTY,
        WALL,
        POINT,
        PACMAN,
        GHOST1,
        GHOST2,
        GHOST3,
        GHOST4
    }

    public struct MapPoint
    {
        public int Row;
        public int Col;

        public MapPoint(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override bool Equals(object obj)
        {
            MapPoint other = (MapPoint)obj;
            return other.Row == this.Row && other.Col == this.Col;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Col.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", Row, Col);
        }
    }

    public class GameMap
    {
        public int Width;
        public int Height;
        public MapTileType[,] Tiles;
        public MapPoint Pacman;
        public MapPoint[] Ghost;

        public MapTileType this[int row, int col]
        {
            get
            {
                return Tiles[row, col];
            }
            set
            {
                Tiles[row, col] = value;
            }
        }

        public MapTileType this[MapPoint point]
        {
            get
            {
                return Tiles[point.Row, point.Col];
            }
            set
            {
                Tiles[point.Row, point.Col] = value;
            }
        }

        public GameMap(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new MapTileType[height, width];
            Ghost = new MapPoint[4];
        }

        public GameMap(string fileName)
        {
            string[] line = File.ReadAllLines(fileName);
            Width = line[0].Length;
            Height = line.Length;
            Tiles = new MapTileType[Height, Width];
            Ghost = new MapPoint[4];
            for (int i = 0; i < line.Length; i++)
            {
                ParseRow(i, line[i]);
            }
        }

        private void ParseRow(int row, string line)
        {
            for (int col = 0; col < Width; col++)
            {
                MapTileType tile = parseCell(line[col]);
                switch (tile)
                {
                    case MapTileType.PACMAN:
                        Pacman.Row = row;
                        Pacman.Col = col;
                        tile = MapTileType.EMPTY;
                        break;
                    case MapTileType.GHOST1:
                        Ghost[0].Row = row;
                        Ghost[0].Col = col;
                        tile = MapTileType.EMPTY;
                        break;
                    case MapTileType.GHOST2:
                        Ghost[1].Row = row;
                        Ghost[1].Col = col;
                        tile = MapTileType.EMPTY;
                        break;
                    case MapTileType.GHOST3:
                        Ghost[2].Row = row;
                        Ghost[2].Col = col;
                        tile = MapTileType.EMPTY;
                        break;
                    case MapTileType.GHOST4:
                        Ghost[3].Row = row;
                        Ghost[3].Col = col;
                        tile = MapTileType.EMPTY;
                        break;
                }
                Tiles[row, col] = tile;
            }
        }

        private MapTileType parseCell(char cell)
        {
            switch (cell)
            {
                case ' ':
                    return MapTileType.EMPTY;
                case '#':
                    return MapTileType.WALL;
                case '.':
                    return MapTileType.POINT;
                case 'C':
                    return MapTileType.PACMAN;
                case '1':
                    return MapTileType.GHOST1;
                case '2':
                    return MapTileType.GHOST2;
                case '3':
                    return MapTileType.GHOST3;
                case '4':
                    return MapTileType.GHOST4;
                default:
                    return MapTileType.EMPTY;
            }
        }
    }
}
