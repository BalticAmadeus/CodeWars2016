using System;
using System.Collections.Generic;
using System.Linq;
using Game.ClientCommon;

namespace Game.DebugClient.Infrastructure
{
    public class MapService : IMapService
    {
        public event MapChangedEventHandler MapChanged;
        public event CellChangedEventHandler CellChanged;

        public CellState[,] Map { get; private set; }

        public int Width
        {
            get
            {
                return Map.GetLength(1);
            }
        }

        public int Height
        {
            get
            {
                return Map.GetLength(0);
            }
        }

        public Point Tecman { get; private set; }
        public IList<Point> Ghost { get; private set; }
        public Point TecmanNext { get; private set; }
        public IList<Point> GhostNext { get; private set; }
        public IList<Point> GhostOld { get; private set; }
        public IList<MapActor> Actors { get; private set; }

        //public void UpdateCell(int x, int y, string state, Player player)
        //{
        //    Map[y] = Map[y].Substring(0, x) + Convert.ToChar(state) + Map[y].Substring(x + 1);
        //    Players[player.Index] = player;

        //    CellChanged?.Invoke(this, new CellChangedEvent {State = state, X = x, Y = y});
        //}

        public void UpdateMap(IList<string> map, Point tecman, IList<Point> ghosts, IList<Point> ghostsOld, bool iAmTecman)
        {
            int width = map[0].Length;
            int height = map.Count;
            Map = new CellState[height, width];
            for (int row = 0; row < height; row++)
            {
                ParseRow(row, map[row]);
            }
            Tecman = tecman;
            TecmanNext = tecman;
            SetFlag(tecman, CellState.Tecman);
            SetFlag(tecman, CellState.TecmanNext);
            Ghost = ghosts.ToArray();
            GhostNext = ghosts.ToArray();
            CellState flg = CellState.Ghost1 | CellState.Ghost1Next;
            foreach (Point p in Ghost)
            {
                SetFlag(p, flg);
                flg = (CellState)((ushort)flg * 2);
            }
            GhostOld = ghostsOld.ToArray();
            //flg = CellState.Ghost1Old;
            //foreach (Point p in GhostOld)
            //{
            //    SetFlag(p, flg);
            //    flg = (CellState)((ushort)flg * 2);
            //}
            if (iAmTecman)
            {
                Actors = new MapActor[]
                {
                    new MapActor(0, "Tc", Tecman, true)
                };
            }
            else
            {
                Actors = Ghost.Select((p, i) => new MapActor(i, "G" + (i + 1), p, false)).ToList();
            }
            MapChanged?.Invoke(this, new MapChangedEventArgs { Map = Map });
        }

        private void SetFlag(Point p, CellState flag)
        {
            Map[p.Row, p.Col] |= flag;
        }

        private void ParseRow(int row, string data)
        {
            for (int col = 0; col < data.Length; col++)
            {
                CellState c;
                switch (data[col])
                {
                    case '#':
                        c = CellState.Wall;
                        break;
                    case '.':
                        c = CellState.Cookie;
                        break;
                    default:
                        c = CellState.Empty;
                        break;
                }
                Map[row, col] = c;
            }
        }
    }
}