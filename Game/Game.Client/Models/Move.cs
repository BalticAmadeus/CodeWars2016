using System.Collections.Generic;

namespace Game.AdminClient.Models
{
    public class Turn
    {
        public Turn()
        {
            PlayerStates = new List<PlayerState>();
        }

        public Game Game { get; set; }
        public int NumberOfQueuedTurns { get; set; }
        public IList<int> SlowPlayers { get; set; }
        public int TurnNumber { get; set; }
        public IList<PlayerState> PlayerStates { get; set; }
        public IList<MapChange> MapChanges { get; set; }
        public Point TecmanPosition { get; set; }
        public IList<Point> GhostPositions { get; set; }
    }

    public class MapChange
    {
        public Point Position { get; set; }
        public string Value { get; set; }
    }

    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("({0};{1})", X, Y);
        }
    }
}