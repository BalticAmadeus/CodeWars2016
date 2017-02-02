using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game.ClientHandler
{
    class Program
    {
        static Random _rnd = new Random();

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("USAGE: Game.ClientHandler input.json output.json");
                return;
            }
            // Read player view
            string inputStr = File.ReadAllText(args[0], Encoding.UTF8);
            var playerView = JsonConvert.DeserializeObject<PlayerView>(inputStr);
            // Do the logic
            var playerMove = PerformMove(playerView);
            // Write move
            string outputStr = JsonConvert.SerializeObject(playerMove);
            File.WriteAllText(args[1], outputStr, Encoding.UTF8);
        }

        static PlayerMove PerformMove(PlayerView view)
        {
            if (view.Mode == "TECMAN")
                return PerformTecmanMove(view);
            else
                return PerformGhostMove(view);
        }

        static PlayerMove PerformTecmanMove(PlayerView view)
        {
            // Find the nearest cookie
            EnPoint direction;
            ShortestPathToCookie(view.Map, view.TecmanPosition, out direction);
            return new PlayerMove
            {
                Positions = new EnPoint[] { direction ?? view.TecmanPosition }.ToList()
            };
        }

        static int ShortestPathToCookie(EnMapData map, EnPoint from, out EnPoint direction)
        {
            direction = null;
            if (map.Rows[from.Row][from.Col] == '.')
            {
                return 0;
            }
            Tuple<int, EnPoint>[,] crumbs = new Tuple<int, EnPoint>[map.Height, map.Width];
            for (int row = 0; row < map.Height; row++)
            {
                for (int col = 0; col < map.Width; col++)
                {
                    crumbs[row, col] = new Tuple<int, EnPoint>(1000000, null);
                }
            }
            var front = new Queue<EnPoint>();
            crumbs[from.Row, from.Col] = new Tuple<int, EnPoint>(0, null);
            front.Enqueue(from);
            while (front.Count > 0)
            {
                EnPoint x = front.Dequeue();
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr * dc != 0 || dr + dc == 0)
                            continue;
                        var y = new EnPoint { Row = x.Row + dr, Col = x.Col + dc };
                        if (y.Row < 0)
                            y.Row = map.Height - 1;
                        else if (y.Row >= map.Height)
                            y.Row = 0;
                        if (y.Col < 0)
                            y.Col = map.Width - 1;
                        else if (y.Col >= map.Width)
                            y.Col = 0;
                        if (map.Rows[y.Row][y.Col] == '#')
                            continue;
                        Tuple<int, EnPoint> t = crumbs[x.Row, x.Col];
                        int v = t.Item1 + 1;
                        if (crumbs[y.Row, y.Col].Item1 <= v)
                            continue;
                        crumbs[y.Row, y.Col] = new Tuple<int, EnPoint>(v, t.Item2 ?? y);
                        if (map.Rows[y.Row][y.Col] == '.')
                        {
                            direction = t.Item2 ?? y;
                            return v;
                        }
                        front.Enqueue(y);
                    }
                }
            }
            return 2000000;
        }

        static PlayerMove PerformGhostMove(PlayerView view)
        {
            var move = new PlayerMove
            {
                Positions = new List<EnPoint>()
            };
            // Move all ghosts randomly
            for (int i = 0; i < view.GhostPositions.Count; i++)
            {
                var options = new List<EnPoint>();
                addGhostOption(options, view, i, 0, 1);
                addGhostOption(options, view, i, 0, -1);
                addGhostOption(options, view, i, 1, 0);
                addGhostOption(options, view, i, -1, 0);
                switch (options.Count)
                {
                    case 0:
                        move.Positions.Add(view.PreviousGhostPositions[i]);
                        break;
                    case 1:
                        move.Positions.Add(options[0]);
                        break;
                    default:
                        move.Positions.Add(options[_rnd.Next(options.Count)]);
                        break;
                }
            }
            return move;
        }

        static void addGhostOption(List<EnPoint> options, PlayerView view, int ghost, int drow, int dcol)
        {
            var target = new EnPoint { Row = view.GhostPositions[ghost].Row + drow, Col = view.GhostPositions[ghost].Col + dcol };

            if (target.Row < 0)
                target.Row = view.Map.Height - 1;
            else if (target.Row >= view.Map.Height)
                target.Row = 0;

            if (target.Col < 0)
                target.Col = view.Map.Width - 1;
            else if (target.Col >= view.Map.Width)
                target.Col = 0;

            if (view.Map.Rows[target.Row][target.Col] != '#'
                && (target.Row != view.PreviousGhostPositions[ghost].Row || target.Col != view.PreviousGhostPositions[ghost].Col))
            {
                options.Add(target);
            }
        }
    }

    public class PlayerView
    {
        public int Turn;
        public string Mode;
        public EnMapData Map;
        public EnPoint TecmanPosition;
        public List<EnPoint> GhostPositions;
        public List<EnPoint> PreviousGhostPositions;
    }

    public class EnPoint
    {
        public int Row;
        public int Col;
    }

    public class EnMapData
    {
        public int Width;
        public int Height;
        public List<string> Rows;
    }

    public class PlayerMove
    {
        public List<EnPoint> Positions;
    }

}
