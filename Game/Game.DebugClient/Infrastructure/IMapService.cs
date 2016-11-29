using Game.ClientCommon;
using System;
using System.Collections.Generic;

namespace Game.DebugClient.Infrastructure
{
    public interface IMapService
    {
        CellState[,] Map { get; }
        int Width { get; }
        int Height { get; }
        Point Tecman { get; }
        IList<Point> Ghost { get; }
        Point TecmanNext { get; }
        IList<Point> GhostNext { get; }
        IList<Point> GhostOld { get; }
        IList<MapActor> Actors { get; }

        event CellChangedEventHandler CellChanged;
        event MapChangedEventHandler MapChanged;

        //void UpdateCell(int x, int y, string state, Player player);
        void UpdateMap(IList<string> map, Point tecman, IList<Point> ghosts, IList<Point> ghostsOld, bool iAmTecman);
    }

    [Flags]
    public enum CellState : ushort
    {
        Empty = 0, Wall = 1, Cookie = 2,
        Ghost1 = 4, Ghost2 = 8, Ghost3 = 16, Ghost4 = 32, Tecman = 64,
        Ghost1Next = 128, Ghost2Next = 256, Ghost3Next = 512, Ghost4Next = 1024, TecmanNext = 2048,
        Ghost1Old = 4096, Ghost2Old = 8192, Ghost3Old = 16384, Ghost4Old = 32768,

        SceneryMask = 3,
        AllGhosts = Ghost1 | Ghost2 | Ghost3 | Ghost4
    }

    public class MapActor
    {
        public int Index { get; private set; }
        public string Name { get; private set; }
        public Point Position { get; private set; }
        public bool IsTecman { get; private set; }

        public MapActor(int index, string name, Point position, bool isTecman)
        {
            Index = index;
            Name = name;
            Position = position;
            IsTecman = isTecman;
        }
    }

    public delegate void MapChangedEventHandler(object sender, MapChangedEventArgs args);

    public class MapChangedEventArgs
    {
        public CellState[,] Map { get; set; }
    }

    public class CellChangedEvent : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellState State { get; set; }
    }

    public delegate void CellChangedEventHandler(object sender, CellChangedEvent args);
}