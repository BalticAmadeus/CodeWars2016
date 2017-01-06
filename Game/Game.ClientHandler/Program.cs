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
            // Just stay
            return new PlayerMove
            {
                Positions = new EnPoint[] { view.TecmanPosition }.ToList()
            };
        }

        static PlayerMove PerformGhostMove(PlayerView view)
        {
            var move = new PlayerMove
            {
                Positions = new List<EnPoint>(view.GhostPositions)
            };
            // Move all ghosts to the right. It's short-sited but probably valid move.
            for (int i = 0; i < move.Positions.Count; i++)
            {
                move.Positions[i].Col++;
            }
            return move;
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
