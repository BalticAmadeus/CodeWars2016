using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Game.ClientCommon.DataContracts
{
    [DataContract]
    public class EnMapData
    {
        [DataMember]
        public int Width;

        [DataMember]
        public int Height;

        [DataMember]
        public List<string> Rows;

        public EnMapData()
        {
            // default
        }

        public EnMapData(MapData md)
        {
            Width = md.Width;
            Height = md.Height;
            Rows = new List<string>();
            for (int row = 0; row < Height; row++)
            {
                Rows.Add(BuildRow(md, row));
            }
        }

        public MapData ToMapData()
        {
            MapData md = new MapData();
            md.Width = Width;
            md.Height = Height;
            md.GhostPosition = new Point[4];

            md.Tiles = new TileType[Height, Width];
            for (int row = 0; row < Height; row++)
            {
                ParseRow(row, Rows[row], md);
            }
            return md;
        }

        private void ParseRow(int row, string data, MapData md)
        {
            for (int col = 0; col < Width; col++)
            {
                TileType tileType = parseTile(data[col]);
                switch (tileType)
                {
                    case TileType.TecmanStart:
                        md.TecmanPosition = new Point(row, col);
                        break;
                    case TileType.GhostStart:
                        int p = Array.IndexOf(md.GhostPosition, new Point());
                        if (p >= 0)
                            md.GhostPosition[p] = new Point(row, col);
                        break;
                    default:
                        md.Tiles[row, col] = tileType;
                        break;
                }
            }
        }

        private TileType parseTile(char code)
        {
            switch (code)
            {
                case '#':
                    return TileType.Wall;
                case ' ':
                    return TileType.Empty;
                case '.':
                    return TileType.Cookie;
                case 'C':
                    return TileType.TecmanStart;
                case 'H':
                    return TileType.GhostStart;
                default:
                    throw new ApplicationException(string.Format("Invalid map tile character '{0}'", code.ToString()));
            }
        }

        public static string BuildRow(MapData md, int row)
        {
            StringBuilder sb = new StringBuilder();
            for (int col = 0; col < md.Width; col++)
            {
                sb.Append(BuildTile(md.Tiles[row, col]));
            }
            return sb.ToString();
        }

        public static char BuildTile(TileType tile)
        {
            switch (tile)
            {
                case TileType.Empty:
                    return ' ';
                case TileType.Wall:
                    return '#';
                case TileType.Cookie:
                    return '.';
                default:
                    return ' ';
            }
        }
    }
}
