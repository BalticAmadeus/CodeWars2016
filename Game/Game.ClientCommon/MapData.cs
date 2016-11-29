using System;

namespace Game.ClientCommon
{
    public class MapData : ICloneable
    {
        public int Width;
        public int Height;
        public TileType[,] Tiles;
        public Point TecmanPosition;
        public Point[] GhostPosition;

        public TileType this[int row, int col]
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

        public TileType this[Point point]
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

        public object Clone()
        {
            return new MapData
            {
                Width = this.Width,
                Height = this.Height,
                Tiles = (TileType[,])this.Tiles.Clone(),
                TecmanPosition = this.TecmanPosition,
                GhostPosition = (Point[])this.GhostPosition.Clone()
            };
        }

        public bool InBounds(Point point)
        {
            return point.Col >= 0
                && point.Row >= 0
                && point.Col < Width
                && point.Row < Height;
        }

        // TODO ToString/Equals/GetHashCode: why do we need this? If we do, use something with better performance?
        public override string ToString()
        {
            var output = string.Empty;
            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    switch (Tiles[row, col])
                    {
                        case TileType.Empty:
                            output += " ";
                            break;
                        case TileType.Wall:
                            output += "#";
                            break;
                        case TileType.Cookie:
                            output += "o";
                            break;
                        case TileType.TecmanStart:
                            output += "C";
                            break;
                        case TileType.GhostStart:
                            output += "E";
                            break;
                        default:
                            output += "X"; // error
                            break;
                    }
                }
                output += "\n";
            }
            return output;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is MapData))
                return false;

            var other = (MapData)obj;

            if (string.Equals(this.ToString(), other.ToString()))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }

    public enum TileType : byte
    {
        Empty = 0,
        Wall,
        Cookie,
        TecmanStart,
        GhostStart
    }
}
