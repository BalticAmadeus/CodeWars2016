using System;
using System.Linq;
using Game.ClientCommon.DataContracts;
using Game.ClientCommon.Infrastructure;
using Game.ClientCommon;
using System.Collections.Generic;

namespace Game.DemoClient
{
    public class GameFlowWrapper
    {
        private Random _rnd = new Random();

        public bool CreatePlayer()
        {
            Console.WriteLine(@"Session setup...");
            if (Settings.SessionId > 0)
                Settings.SessionId++;
            else
                Settings.SessionId = (int) (DateTime.Now - new DateTime(2016, 06, 01, 0, 0, 0)).TotalSeconds;
            Settings.SequenceNumber = 1;
            Settings.Turn = 0;
            Console.WriteLine(@"Create player...");
            var req = new CreatePlayerReq
            {
                Auth = new ReqAuth
                {
                    TeamName = Settings.TeamName,
                    AuthCode = Settings.AuthCode,
                    ClientName = Settings.UserName,
                    SequenceNumber = Settings.SequenceNumber,
                    SessionId = Settings.SessionId
                }
            };

            var createPlayerResp = Program.ServiceCallInvoker.InvokeAsync<CreatePlayerReq, CreatePlayerResp>(Settings.ServerUrl.TrimEnd('/') + "/json/CreatePlayer", req).Result;
            Settings.SequenceNumber++;

            if (createPlayerResp.IsOk())
            {
                Settings.PlayerId = createPlayerResp.PlayerId;
                return true;
            }

            return false;
        }

        public bool WaitNextTurn()
        {
            var req = new WaitNextTurnReq
            {
                Auth = new ReqAuth
                {
                    TeamName = Settings.TeamName,
                    AuthCode = Settings.AuthCode,
                    ClientName = Settings.UserName,
                    SequenceNumber = Settings.SequenceNumber,
                    SessionId = Settings.SessionId
                },
                PlayerId = Settings.PlayerId,
                RefTurn = Settings.Turn
            };

            while (true)
            {
                req.Auth.AuthCode = Settings.AuthCode;
                req.Auth.SequenceNumber = Settings.SequenceNumber;
                req.RefTurn = Settings.Turn;

                Console.WriteLine($"WaitNextTurn RefTurn={req.RefTurn}");
                var waitNextTurnResp = Program.ServiceCallInvoker.InvokeAsync<WaitNextTurnReq, WaitNextTurnResp>(Settings.ServerUrl.TrimEnd('/') + "/json/WaitNextTurn", req).Result;
                Settings.SequenceNumber++;

                if (waitNextTurnResp.IsOk())
                {
                    if (waitNextTurnResp.GameFinished)
                        return false;

                    if (waitNextTurnResp.TurnComplete)
                        return true;
                }
                else
                {
                    throw new ApplicationException("WaitNextTurn() Failed");
                }
            }
        }

        public bool GetPlayerView()
        {
            var req = new GetPlayerViewReq
            {
                Auth = new ReqAuth
                {
                    TeamName = Settings.TeamName,
                    AuthCode = Settings.AuthCode,
                    ClientName = Settings.UserName,
                    SequenceNumber = Settings.SequenceNumber,
                    SessionId = Settings.SessionId
                },
                PlayerId = Settings.PlayerId
            };

            var getPlayerViewResp = Program.ServiceCallInvoker.InvokeAsync<GetPlayerViewReq, GetPlayerViewResp>(Settings.ServerUrl.TrimEnd('/') + "/json/GetPlayerView", req).Result;
            Settings.SequenceNumber++;

            if (getPlayerViewResp.IsOk())
            {
                Settings.Turn = getPlayerViewResp.Turn;
                Settings.Map = getPlayerViewResp.Map.Rows.ToArray();
                Settings.MapData = getPlayerViewResp.Map.ToMapData();
                Settings.IAmTecman = (getPlayerViewResp.Mode == "TECMAN");
                Settings.TecmanPosition = getPlayerViewResp.TecmanPosition.ToPoint();
                Settings.GhostPosition = getPlayerViewResp.GhostPositions.Select(p => p.ToPoint()).ToArray();
                Settings.PrevGhostPosition = getPlayerViewResp.PreviousGhostPositions.Select(p => p.ToPoint()).ToArray();
                return true;
            }

            return false;
        }

        public bool PerformMove()
        {
            var req = new PerformMoveReq
            {
                Auth = new ReqAuth
                {
                    TeamName = Settings.TeamName,
                    AuthCode = Settings.AuthCode,
                    ClientName = Settings.UserName,
                    SequenceNumber = Settings.SequenceNumber,
                    SessionId = Settings.SessionId
                },
                PlayerId = Settings.PlayerId,
                Positions = GetTurn().Select(p => new EnPoint(p)).ToList()
            };

            var performMoveResp = Program.ServiceCallInvoker.InvokeAsync<PerformMoveReq, PerformMoveResp>(Settings.ServerUrl.TrimEnd('/') + "/json/PerformMove", req).Result;
            Settings.SequenceNumber++;

            if (performMoveResp.IsOk())
            {
                return true;
            }
            return false;
        }

        private Point[] GetTurn()
        {
            if (Settings.IAmTecman)
            {
                return new Point[] { GetTecmanMove() };
            }
            else
            {
                return GetGhostMove();
            }
        }

        private Point GetTecmanMove()
        {
            // Find nearest osbtacle in a line of sight in all directions
            List<GameSight> rays = new List<GameSight>();
            lookRay(rays, -1, 0);
            lookRay(rays, 1, 0);
            lookRay(rays, 0, -1);
            lookRay(rays, 0, 1);
            rays.Sort((a, b) =>
            {
                int diff = weighType(a) - weighType(b);
                if (diff != 0)
                    return diff;
                if (a.Obstacle == TileType.GhostStart)
                    return b.Distance - a.Distance;
                else
                    return a.Distance - b.Distance;
            });
            var options = rays.Where(t => (t.Obstacle == TileType.Wall || t.Obstacle == TileType.Cookie) && !t.Position.Equals(Settings.CameFrom)).ToList();
            if (options.Count > 0)
            {
                Settings.CameFrom = Settings.TecmanPosition;
                return options[_rnd.Next(options.Count)].Position;
            }
            options = rays.Where(t => t.Obstacle == TileType.Wall || t.Obstacle == TileType.Cookie).ToList();
            if (options.Count > 0)
            {
                Settings.CameFrom = Settings.TecmanPosition;
                return options[_rnd.Next(options.Count)].Position;
            }
            if (rays.Count > 0)
            {
                Settings.CameFrom = Settings.TecmanPosition;
                return rays[0].Position;
            }
            return Settings.TecmanPosition;
        }

        private void lookRay(List<GameSight> rays, int rdiff, int cdiff)
        {
            MapData map = Settings.MapData;
            Point tecman = Settings.TecmanPosition;
            int row = tecman.Row + rdiff;
            int col = tecman.Col + cdiff;
            Point look = new Point(row, col);
            int distance = 0;
            bool cookieFound = false;
            int cookieDistance = 0;
            while (row >= 0 && row < map.Height && col >= 0 && col < map.Width)
            {
                if (map[row, col] == TileType.Wall)
                {
                    if (distance > 0)
                    {
                        if (cookieFound)
                        {
                            rays.Add(new GameSight
                            {
                                Position = look,
                                Distance = cookieDistance,
                                Obstacle = TileType.Cookie
                            });
                        }
                        else
                        {
                            rays.Add(new GameSight
                            {
                                Position = look,
                                Distance = distance,
                                Obstacle = TileType.Wall
                            });
                        }
                    }
                    return;
                }
                if (Settings.GhostPosition.Any(g => g.Row == row && g.Col == col))
                {
                    rays.Add(new GameSight
                    {
                        Position = look,
                        Distance = distance,
                        Obstacle = TileType.GhostStart
                    });
                    return;
                }
                if (!cookieFound && map[row, col] == TileType.Cookie)
                {
                    cookieFound = true;
                    cookieDistance = distance;
                }
                distance++;
                row += rdiff;
                col += cdiff;
            }
            if (distance > 0)
            {
                rays.Add(new GameSight
                {
                    Position = look,
                    Distance = distance,
                    Obstacle = TileType.Wall
                });
            }
        }

        private int weighType(GameSight sight)
        {
            switch (sight.Obstacle)
            {
                case TileType.Cookie:
                    return 1;
                case TileType.Wall:
                    return 1;
                case TileType.GhostStart:
                    return 2;
                default:
                    return 3;
            }
        }

        private int shortestPathToPacman(Point p)
        {
            if (p.Equals(Settings.TecmanPosition))
                return 0;
            MapData map = Settings.MapData;
            int[,] crumbs = new int[map.Height, map.Width];
            for (int row = 0; row < map.Height; row++)
            {
                for (int col = 0; col < map.Width; col++)
                {
                    crumbs[row, col] = 1000000;
                }
            }
            Queue<Point> front = new Queue<Point>();
            crumbs[p.Row, p.Col] = 0;
            front.Enqueue(p);
            while (front.Count > 0)
            {
                Point x = front.Dequeue();
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr * dc != 0 || dr + dc == 0)
                            continue;
                        Point y = new Point(x.Row + dr, x.Col + dc);
                        if (y.Row < 0 || y.Row >= map.Height)
                            continue;
                        if (y.Col < 0 || y.Col >= map.Width)
                            continue;
                        if (map[y] == TileType.Wall)
                            continue;
                        int v = crumbs[x.Row, x.Col] + 1;
                        if (crumbs[y.Row, y.Col] <= v)
                            continue;
                        crumbs[y.Row, y.Col] = v;
                        if (y.Equals(Settings.TecmanPosition))
                            return v;
                        front.Enqueue(y);
                    }
                }
            }
            return 2000000;
        }

        private void checkGhostPoint(List<Point> points, int i, int rdiff, int cdiff)
        {
            Point p = new Point(Settings.GhostPosition[i].Row + rdiff, Settings.GhostPosition[i].Col + cdiff);
            if (p.Row < 0 || p.Row >= Settings.MapData.Height)
                return;
            if (p.Col < 0 || p.Col >= Settings.MapData.Width)
                return;
            if (Settings.PrevGhostPosition != null && p.Equals(Settings.PrevGhostPosition[i]))
                return;
            if (Settings.MapData[p] == TileType.Wall)
                return;
            points.Add(p);
        }

        private Point[] GetGhostMove()
        {
            Point[] move = new Point[4];
            for (int i = 0; i < 4; i++)
            {
                List<Point> points = new List<Point>();
                checkGhostPoint(points, i, -1, 0);
                checkGhostPoint(points, i, 1, 0);
                checkGhostPoint(points, i, 0, -1);
                checkGhostPoint(points, i, 0, 1);
                if (points.Count == 1)
                {
                    move[i] = points[0];
                }
                else if (points.Count > 0)
                {
                    Tuple<int, int>[] values = new Tuple<int, int>[points.Count];
                    for (int j = 0; j < values.Length; j++)
                    {
                        values[j] = Tuple.Create(shortestPathToPacman(points[j]), j);
                    }
                    Array.Sort(values, (a, b) => a.Item1 - b.Item1);
                    int good = values[0].Item1;
                    values = values.Where(t => t.Item1 == good).ToArray();
                    move[i] = points[values[_rnd.Next(values.Length)].Item2];
                }
                else
                {
                    move[i] = Settings.GhostPosition[i]; // Shouldn't happen as we will lose
                }
            }
            return move;
        }
    }
}
