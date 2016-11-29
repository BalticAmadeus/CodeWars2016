using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Game.AdminClient.AdminService;
using Game.AdminClient.Infrastructure;
using Game.AdminClient.Models;
using GameLogic.UserManagement;

namespace Game.AdminClient.Infrastructure
{
    public class ConnectionData
    {
        public string Username { get; set; }
        public string TeamName { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
    }

    public class AdministrationServiceGateway : IAdministrationServiceGateway
    {
        public ConnectionData ConnectionData { get; set; }
        public long LastCallTime { get; set; }

        private readonly Stopwatch _stopwatch;

        private int _observerId;
        private int _sessionId;

        private readonly object _lock = new object();

        private int _sequenceNumber;
        private int SequenceNumber
        {
            get
            {
                int value;
                lock (_lock)
                {
                    value = _sequenceNumber++;
                }

                return value;
            }
            set { _sequenceNumber = value; }
        }

        public AdministrationServiceGateway()
        {
            _stopwatch = new Stopwatch();
        }

        public async Task<TeamRole> LoginAsync()
        {
            SequenceNumber = 0;
            _sessionId = 0;

            lock (_lock)
            {

                using (
                    var adminService = new AdminServiceClient(new BasicHttpBinding(),
                        new EndpointAddress(ConnectionData.Url)))
                {
                    int sequenceNumber = SequenceNumber;

                    var initLoginReq = new InitLoginReq
                    {
                        Auth = new ReqAuth
                        {
                            ClientName = ConnectionData.Username,
                            TeamName = ConnectionData.TeamName,
                            AuthCode =
                                GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName,
                                    ConnectionData.Username,
                                    0, 0, ConnectionData.Password)),
                            SessionId = 0,
                            SequenceNumber = 0,
                        }
                    };

                    _stopwatch.Start();

                    var initLoginResp = adminService.InitLoginAsync(initLoginReq).Result;

                    _stopwatch.Stop();
                    LastCallTime = _stopwatch.ElapsedMilliseconds;
                    _stopwatch.Reset();

                    if (initLoginResp.Status != "OK")
                        throw new Exception(initLoginResp.Message);

                    var completeReq = new CompleteLoginReq
                    {
                        Auth = new ReqAuth
                        {
                            ClientName = ConnectionData.Username,
                            TeamName = ConnectionData.TeamName,
                            AuthCode = GetAuthCode(
                                string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username, 0,
                                    0, ConnectionData.Password)),
                            SessionId = 0,
                            SequenceNumber = 0,
                        },
                        ChallengeResponse =
                            GetAuthCode(string.Format("{0}{1}",
                                GetAuthCode(string.Format("{0}{1}", initLoginResp.Challenge, ConnectionData.Password)),
                                ConnectionData.Password)),
                    };

                    sequenceNumber = SequenceNumber;

                    _stopwatch.Start();

                    var completeLoginResp = adminService.CompleteLoginAsync(completeReq).Result;

                    _stopwatch.Stop();
                    LastCallTime = _stopwatch.ElapsedMilliseconds;
                    _stopwatch.Reset();

                    if (completeLoginResp.Status != "OK")
                        throw new Exception(completeLoginResp.Message);

                    _sessionId = completeLoginResp.SessionId;

                    var req = new CreateObserverReq
                    {
                        Auth = new ReqAuth
                        {
                            ClientName = ConnectionData.Username,
                            TeamName = ConnectionData.TeamName,
                            SessionId = _sessionId,
                            SequenceNumber = sequenceNumber,
                            AuthCode =
                                GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName,
                                    ConnectionData.Username,
                                    _sessionId, sequenceNumber, ConnectionData.Password)),
                        },
                    };

                    _stopwatch.Start();

                    var resp = adminService.CreateObserverAsync(req).Result;

                    _stopwatch.Stop();
                    LastCallTime = _stopwatch.ElapsedMilliseconds;
                    _stopwatch.Reset();

                    if (resp.Status != "OK")
                        throw new Exception(resp.Message);

                    _observerId = resp.ObserverId;

                    return resp.Role;
                }
            }
        }

        public async Task<Game.AdminClient.Models.Game> CreateGameAsync()
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var createGameReq = new CreateGameReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                };

                _stopwatch.Start();

                var createGameResp = await adminService.CreateGameAsync(createGameReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (createGameResp.Status != "OK")
                    throw new Exception(createGameResp.Message);

                return new Models.Game
                {
                    GameId = createGameResp.GameInfo.GameId,
                    Label = createGameResp.GameInfo.Label,
                    State = createGameResp.GameInfo.State,
                };
            }
        }

        public async Task<IEnumerable<Models.Game>> ListGamesAsync()
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var listGamesReq = new ListGamesReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                };

                _stopwatch.Start();

                var listGamesResp = await adminService.ListGamesAsync(listGamesReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (listGamesResp.Status != "OK")
                    throw new Exception(listGamesResp.Message);

                var games = listGamesResp.Games.Select(p => new Models.Game
                {
                    GameId = p.GameId,
                    Label = p.Label,
                    State = p.State,
                });

                return games;
            }
        }

        public async Task<Models.Game> GetGameAsync(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var getGameDetailsReq = new GetGameDetailsReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                };

                _stopwatch.Start();

                var getGameDetailsResp = await adminService.GetGameDetailsAsync(getGameDetailsReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (getGameDetailsResp.Status != "OK")
                    throw new Exception(getGameDetailsResp.Message);

                return new Models.Game
                {
                    GameId = getGameDetailsResp.GameDetails.GameId,
                    Label = getGameDetailsResp.GameDetails.Label,
                    State = getGameDetailsResp.GameDetails.State,
                    Players = getGameDetailsResp.GameDetails.Players.Select(p => new Player
                    {
                        GameId = p.GameId,
                        Name = p.Name,
                        Team = p.Team,
                        PlayerId = p.PlayerId,
                    })
                };
            }
        }

        public async Task DeleteGameAsync(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var deleteGameReq = new DeleteGameReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                };

                _stopwatch.Start();

                var deleteGameResp = await adminService.DeleteGameAsync(deleteGameReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (deleteGameResp.Status != "OK")
                    throw new Exception(deleteGameResp.Message);
            }
        }

        public async Task<IEnumerable<Player>> ListPlayersAsync()
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var listPlayersReq = new ListPlayersReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                };

                _stopwatch.Start();

                var listPlayersResp = await adminService.ListPlayersAsync(listPlayersReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (listPlayersResp.Status != "OK")
                    throw new Exception(listPlayersResp.Message);

                var players = listPlayersResp.Players.Select(p => new Player
                {
                    Name = p.Name,
                    GameId = p.GameId,
                    PlayerId = p.PlayerId,
                    Team = p.Team,
                });

                return players;
            }
        }

        public async Task AddPlayerAsync(int gameId, int playerId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var addGamePlayerReq = new AddGamePlayerReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                    PlayerId = playerId,
                };

                _stopwatch.Start();

                var addGamePlayerResp = await adminService.AddGamePlayerAsync(addGamePlayerReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (addGamePlayerResp.Status != "OK")
                    throw new Exception(addGamePlayerResp.Message);
            }
        }

        public async Task RemovePlayerAsync(int gameId, int playerId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var removeGamePlayerReq = new RemoveGamePlayerReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                    PlayerId = playerId,
                };

                _stopwatch.Start();

                var removeGamePlayerResp = await adminService.RemoveGamePlayerAsync(removeGamePlayerReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (removeGamePlayerResp.Status != "OK")
                    throw new Exception(removeGamePlayerResp.Message);
            }
        }

        public async Task SetMapAsync(int gameId, Map map)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var setGameMapReq = new SetGameMapReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                    MapData = new EnMapData
                    {
                        Height = map.Height,
                        Width = map.Width,
                        Rows = map.Rows.ToArray(),
                    }
                };

                _stopwatch.Start();

                var setGameMapResp = await adminService.SetGameMapAsync(setGameMapReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (setGameMapResp.Status != "OK")
                    throw new Exception(setGameMapResp.Message);
            }
        }

        public async Task StartGameAsync(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var getGameDetailsReq = new StartGameReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                };

                _stopwatch.Start();

                var getGameDetailsResp = await adminService.StartGameAsync(getGameDetailsReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (getGameDetailsResp.Status != "OK")
                    throw new Exception(getGameDetailsResp.Message);
            }
        }

        public async Task ResumeGameAsync(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var resumeGameReq = new ResumeGameReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                };

                _stopwatch.Start();

                var resumeGameResp = await adminService.ResumeGameAsync(resumeGameReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (resumeGameResp.Status != "OK")
                    throw new Exception(resumeGameResp.Message);
            }
        }

        public async Task PauseGameAsync(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var pauseGameReq = new PauseGameReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                };

                _stopwatch.Start();

                var pauseGameResp = await adminService.PauseGameAsync(pauseGameReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (pauseGameResp.Status != "OK")
                    throw new Exception(pauseGameResp.Message);
            }
        }

        public async Task<Match> GetMatchAsync(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var req = new StartObservingReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                    ObserverId = _observerId,
                };

                _stopwatch.Start();

                var resp = await adminService.StartObservingAsync(req);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (resp.Status != "OK")
                    throw new Exception(resp.Message);

                return new Match
                {
                    Map = new Map
                    {
                        Rows = resp.MapData.Rows,
                        Width = resp.MapData.Width,
                        Height = resp.MapData.Height,
                        TecmanPosition = new Point(resp.TecmanPosition.Col, resp.TecmanPosition.Row),
                        GhostPositions = resp.GhostPositions.Select(p => new Point(p.Col, p.Row)).ToArray(),
                    },
                    Game = new Models.Game
                    {
                        GameId = resp.GameDetails.GameId,
                        Label = resp.GameDetails.Label,
                        State = resp.GameDetails.State,
                        Players = resp.GameDetails.Players.Select(p => new Player
                        {
                            GameId = p.GameId,
                            Name = p.Name,
                            Team = p.Team,
                            PlayerId = p.PlayerId,
                        })
                    },
                    PlayerStates = resp.PlayerStates.Select(p => new PlayerState
                    {
                        Condition = p.Condition,
                        //Position = new Point(p.Position.Col, p.Position.Row),
                    }).ToList(),
                };
            }
        }

        public async Task<Turn> GetNextTurnAsync(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var req = new ObserveNextTurnReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                    ObserverId = _observerId,
                };

                _stopwatch.Start();

                var resp = await adminService.ObserveNextTurnAsync(req);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (resp.Status != "OK")
                    throw new Exception(resp.Message);

                var turn = new Turn
                {
                    Game = new Models.Game
                    {
                        GameId = resp.GameInfo.GameId,
                        State = resp.GameInfo.GameState,
                    },
                    NumberOfQueuedTurns = resp.GameInfo.QueuedTurns,
                    SlowPlayers = resp.GameInfo.SlowPlayers
                };

                if (resp.TurnInfo == null)
                {
                    turn.TurnNumber = -1;
                    turn.PlayerStates = null;
                }
                else
                {
                    turn.TurnNumber = resp.TurnInfo.Turn;
                    turn.PlayerStates = resp.TurnInfo.PlayerStates.Select(p => new PlayerState
                    {
                        Condition = p.Condition,
                        //Position = new Point(p.Position.Col, p.Position.Row),
                        Score = p.Score

                    }).ToList();

                    turn.MapChanges = resp.TurnInfo.MapChanges.Select(p => new MapChange
                    {
                        Position = new Point(p.Position.Col, p.Position.Row),
                        Value = p.Value,

                    }).ToList();

                    turn.TecmanPosition = new Point(resp.TurnInfo.TecmanPosition.Col, resp.TurnInfo.TecmanPosition.Row);
                    turn.GhostPositions = resp.TurnInfo.GhostPositions.Select(p => new Point(p.Col, p.Row)).ToList();
                }

                return turn;
            }
        }

        public async Task DropPlayer(int gameId, int playerId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var dropPlayerReq = new DropPlayerReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                    PlayerId = playerId,
                };

                _stopwatch.Start();

                var dropPlayerResp = await adminService.DropPlayerAsync(dropPlayerReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (dropPlayerResp.Status != "OK")
                    throw new Exception(dropPlayerResp.Message);
            }
        }

        public async Task<GetLiveInfoResp> GetLiveInfo(int gameId)
        {
            using (var adminService = new AdminServiceClient(new BasicHttpBinding(), new EndpointAddress(ConnectionData.Url)))
            {
                int sequenceNumber = SequenceNumber;

                var getLiveInfoReq = new GetLiveInfoReq
                {
                    Auth = new ReqAuth
                    {
                        ClientName = ConnectionData.Username,
                        TeamName = ConnectionData.TeamName,
                        SessionId = _sessionId,
                        SequenceNumber = sequenceNumber,
                        AuthCode =
                            GetAuthCode(string.Format("{0}:{1}:{2}:{3}{4}", ConnectionData.TeamName, ConnectionData.Username,
                                _sessionId, sequenceNumber, ConnectionData.Password)),
                    },
                    GameId = gameId,
                };

                _stopwatch.Start();

                var getLiveInfoResp = await adminService.GetLiveInfoAsync(getLiveInfoReq);

                _stopwatch.Stop();
                LastCallTime = _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                if (getLiveInfoResp.Status != "OK")
                    throw new Exception(getLiveInfoResp.Message);

                return getLiveInfoResp;
            }
        }

        public static string GetAuthCode(string rawData)
        {
            var sha1Managed = HashAlgorithm.Create("SHA1");
            if (sha1Managed == null)
                throw new ArgumentException("Specified hash algorithm type is not supported.");

            var encoding = Encoding.UTF8;
            var bytes = encoding.GetBytes(rawData);

            return BitConverter.ToString(sha1Managed.ComputeHash(bytes)).Replace("-", "").ToLower();
        }
    }

    public class LiveInfo
    {

    }
}
