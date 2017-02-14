using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameLogic
{
    public class GameModel : IDisposable
    {
        public int GameId { get; private set; }
        public Team Owner { get; private set; }
        public long GameUid { get; private set; }
        
        private GameState _state;
        private readonly List<Player> _players;
        private readonly List<ObserverQueue> _observers;
        private MapData _map;
        private readonly GameProtocol _proto;
        private Point _lastTecmanPosition;
        private Point[] _lastGhostPosition;

        private object _liveLock;
        private Dictionary<int, int> _indexes;
        private PlayerState[] _playerStates;
        private int _gameTurnEnded;
        private int _gameTurnStarted;
        private DateTime _turnStart;
        private DateTime _turnEnd;
        private readonly int _turnDuration;

        public GameModel(long uidBase, int gameId, Team owner)
        {
            GameId = gameId;
            Owner = owner;
            GameUid = uidBase + GameId;
            State = GameState.Setup;
            _players = new List<Player>();
            _observers = new List<ObserverQueue>();
            _proto = new GameProtocol(this);
            _turnDuration = Settings.DefaultGameTurnDurationMillis;
        }

        public GameState State
        {
            get
            {
                return _state;
            }

            private set
            {
                _state = value;
                _proto?.LogGameState(_state);
            }
        }

        public string Label
        {
            get
            {
                var label = new StringBuilder($"Game #{GameId} [{Owner}]");

                if (_players != null && _players.Any())
                {
                    label.Append(": ");
                    bool first = true;
                    bool sameTeam = !_players.Any(p => !p.Team.Equals(_players[0].Team));
                    if (Owner.PowerTeam && sameTeam)
                        label.Append("[").Append(_players[0].Team.Name).Append("] ");
                    foreach (Player p in _players)
                    {
                        if (!first)
                            label.Append(" vs ");

                        if (Owner.PowerTeam && !sameTeam)
                            label.Append(p.Team.Name);
                        else
                            label.Append(p.Name);

                        first = false;
                    }
                }

                return label.ToString();
            }
        }

        public void Dispose()
        {
            _proto.Dispose();
        }

        private void CheckSetupState()
        {
            if (State != GameState.Setup)
                throw new ApplicationException("Game must be in SETUP state");
        }

        public void CheckRunState()
        {
            if (State == GameState.Setup)
                throw new ApplicationException("Game must be started");
        }

        public void CheckPlayState()
        {
            if (State != GameState.Play && State != GameState.Pause)
                throw new ApplicationException("Game must be playing");
        }

        public void CheckGameDeletable()
        {
            if (State == GameState.Setup)
                return;
            lock (_liveLock)
            {
                if (State != GameState.Finish)
                    throw new ApplicationException("Game must be finished");
                //if (_playerStates.Any(p => p.IsPresent)) // Should never happen but just as a precaution
                //    throw new ApplicationException("All players must leave");
            }
        }

        public void AddPlayer(Player player)
        {
            CheckSetupState();
            if (player.Game != null)
                throw new ApplicationException("Player already in a game");
            _players.Add(player);
            player.Game = this;
        }

        public void RemovePlayer(Player player)
        {
            CheckSetupState();
            if (player.Game != this)
                throw new ApplicationException("Player is not in this game");
            _players.Remove(player);
            player.Game = null;
        }

        public IEnumerable<Player> ListPlayers()
        {
            return _players;
        }

        public void SetMap(MapData mapData)
        {
            CheckSetupState();
            _map = mapData;
        }

        public void Start()
        {
            CheckSetupState();
            if (_players.Count != 2)
                throw new ApplicationException("Number of players in the game must be 2");
            if (_map == null)
                throw new ApplicationException("Map is not loaded");
            if (_map.GhostPosition.Length < 1)
                throw new ApplicationException("There are no ghosts on the map");
            if (_map.GhostPosition.Length > 4)
                throw new ApplicationException("There are more than 4 ghosts on the map");

            _indexes = _players.Select((p, i) => Tuple.Create(p.PlayerId, i)).ToDictionary(k => k.Item1, v => v.Item2);

            // Setup player initial states
            _playerStates = new PlayerState[_players.Count];
            for (int p = 0; p < _playerStates.Length; p++)
            {
                _playerStates[p] = new PlayerState
                {
                    Index = p,
                    PlayerId = _players[p].PlayerId,
                    Condition = PlayerCondition.Play,
                    IsPresent = true,
                    TurnFinTime = default(DateTime),
                    PenaltyPoints = 0,
                    BonusPoints = 0,
                    OvertimeTurnMsec = 0,
                    OvertimeTurnTurn = -1,
                    PenaltyThresholdReachedTurn = -1,
                    Score = (p == 1) ? Settings.GameTurnLimit : 0
                };
            }
            _lastTecmanPosition = _map.TecmanPosition;
            _lastGhostPosition = (Point[])_map.GhostPosition.Clone();

            _gameTurnEnded = 0;
            _gameTurnStarted = 0;
            PrepareTurnTimings();
            _liveLock = new object();

            // Start paused
            State = GameState.Pause;
            _proto.LogGameStart(this);
        }

        private void PrepareTurnTimings()
        {
            _turnStart = DateTime.Now;
            _turnEnd = _turnStart.AddMilliseconds(_turnDuration);
        }

        private int FindPlayer(int playerId)
        {
            int index;
            if (!_indexes.TryGetValue(playerId, out index))
                throw new ApplicationException("Player is not in this game");
            return index;
        }

        private ObserverQueue FindObserver(int observerId)
        {
            ObserverQueue q = _observers.FirstOrDefault(p => p.Observer.ObserverId == observerId);
            if (q == null)
                throw new ApplicationException("This observer does not watch this game");
            return q;
        }

        public GameViewInfo GetGameView(int playerId)
        {
            lock (_liveLock)
            {
                var gv = new GameViewInfo();

                gv.GameUid = GameUid;

                if (playerId > 0)
                {
                    int p = FindPlayer(playerId);
                    if (!_playerStates[p].IsActive || _playerStates[p].TurnCompleted >= _gameTurnStarted)
                        throw new WaitException();
                    gv.PlayerIndex = p;
                }

                gv.GameState = State;
                gv.Turn = _gameTurnStarted;
                gv.PlayerStates = _playerStates.Select(s => new PlayerStateInfo(s)).ToArray();
                gv.Map = (MapData)_map.Clone();
                gv.PreviousTecmanPosition = _lastTecmanPosition;
                gv.PreviousGhostPosition = (Point[])_lastGhostPosition.Clone();
                return gv;
            }
        }

        public void PerformMove(int playerId, Point[] positions)
        {
            lock (_liveLock)
            {
                CheckPlayState();
                int p = FindPlayer(playerId);
                if (!_playerStates[p].IsActive)
                    throw new ApplicationException("You cannot make more moves");

                if (_playerStates[p].TurnCompleted >= _gameTurnStarted)
                    throw new WaitException();

                // Just record the move
                _playerStates[p].TurnPositions = positions;
                _proto.LogMove(p, playerId, positions);
            }
        }

        private bool PlayerExitTest(WaitTurnInfo wi)
        {
            int p = wi.PlayerIndex;
            if (!_playerStates[p].IsActive)
            {
                wi.TurnComplete = true;
                wi.GameFinished = true;
                wi.FinishCondition = _playerStates[p].Condition;
                wi.FinishComment = _playerStates[p].Comment;
                return true;
            }
            return false;
        }

        public WaitTurnInfo CompletePlayerTurn(int playerId, int refTurn, DateTime callTimestamp)
        {
            lock (_liveLock)
            {
                WaitTurnInfo wi = new WaitTurnInfo();
                int player = FindPlayer(playerId);
                if (_playerStates[player].IsPresent == false)
                    throw new ApplicationException("Player was dropped from the game");
                wi.PlayerIndex = player;

                if (refTurn == 0)
                {
                    // Crash recovery logic
                    if (_playerStates[player].TurnCompleted < _gameTurnStarted)
                    {
                        wi.TurnComplete = true;
                        return wi;
                    }
                    else
                    {
                        wi.Turn = _playerStates[player].TurnCompleted;
                        return wi;
                    }
                }

                if (_playerStates[player].TurnCompleted == refTurn)
                {
                    wi.Turn = refTurn;
                    return wi;
                }

                if (PlayerExitTest(wi))
                    return wi;

                if (_playerStates[player].TurnCompleted != refTurn - 1)
                    throw new ApplicationException($"Player is confusing turns: completed={_playerStates[player].TurnCompleted} refTurn={refTurn}");
                if (refTurn > _gameTurnStarted)
                    throw new ApplicationException($"Player skipping ahead of game progress: gameTurnStarted={_gameTurnStarted} refTurn={refTurn}");

                _playerStates[player].TurnCompleted = _gameTurnStarted;
                _playerStates[player].TurnFinTime = callTimestamp;

                #region Penalty logic
                int totalMsec = (int)(_playerStates[player].TurnFinTime - _turnStart).TotalMilliseconds;
                if (totalMsec > Settings.TurnResponseThresholdMillis)
                {
                    _playerStates[player].PenaltyPoints += (totalMsec - Settings.TurnResponseThresholdMillis) / 100;
                    // TODO OvertimeTurn stuff is of questionable value to us. Ditto Penalty Threshold, but the latter is at least understandable.
                    _playerStates[player].OvertimeTurnMsec = totalMsec;
                    _playerStates[player].OvertimeTurnTurn = _playerStates[player].TurnCompleted;
                    if (_playerStates[player].PenaltyPoints > Settings.PenaltyPointsThreshold && _playerStates[player].PenaltyThresholdReachedTurn < 0)
                        _playerStates[player].PenaltyThresholdReachedTurn = _playerStates[player].TurnCompleted;
                }
                else
                {
                    _playerStates[player].BonusPoints += (Settings.TurnResponseThresholdMillis - totalMsec) / 100;
                }
                #endregion

                _proto.LogPlayerTurnComplete(_playerStates[player], _turnStart);

                CompleteTurn();

                if (PlayerExitTest(wi))
                    return wi;

                wi.Turn = refTurn;
                return wi;
            }
        }

        private bool StartNextTurnMaybe()
        {
            if (_gameTurnStarted > _gameTurnEnded)
                return true;
            if (State != GameState.Play)
                return false;
            if ((_turnEnd - DateTime.Now).TotalMilliseconds > Settings.GameTurnDurationAccuracyMillis)
                return false;
            _gameTurnStarted = _gameTurnEnded + 1;
            PrepareTurnTimings();
            Monitor.PulseAll(_liveLock);
            _proto.LogGameTurnStart(_gameTurnStarted);
            return true;
        }

        private void CompleteTurn()
        {
            if (StartNextTurnMaybe() == false)
                return;
            if (_playerStates.Any(t => t.IsActive && t.TurnCompleted < _gameTurnStarted))
                return;

            var mapChanges = new List<MapChange>();
            Point oldTecmanPos = _map.TecmanPosition;
            Point[] oldGhostPos = (Point[])_map.GhostPosition.Clone();

            // Check individual actor moves
            for (int p = 0; p < _playerStates.Length; p++)
            {
                if (_playerStates[p].IsActive == false) continue;

                try
                {
                    if (p == 0)
                    {
                        // --- Tecman checks
                        // Position reporting
                        if (_playerStates[p].TurnPositions == null || _playerStates[p].TurnPositions.Length != 1)
                            throw new RulesException("Tecman position specified incorrectly");
                        Point position = _playerStates[p].TurnPositions[0];
                        // Movement distance and target
                        MovementDirection direction = GetMovementDirection(oldTecmanPos, position);
                        if (direction == MovementDirection.Invalid)
                            throw new RulesException("Invalid movement distance or direction");
                        if (!_map.InBounds(position))
                            throw new RulesException("Move outside of map");
                        if (_map[position] == TileType.Wall)
                            throw new RulesException("Move into an obstacle");
                        // Move is possible so we move
                        _lastTecmanPosition = _map.TecmanPosition;
                        _map.TecmanPosition = position;
                        // Eat cookie
                        if (_map[position] == TileType.Cookie)
                        {
                            _map[position, mapChanges] = TileType.Empty;
                            _playerStates[p].Score++;
                        }
                    }
                    else
                    {
                        // --- Ghost checks
                        // Position reporting
                        if (_playerStates[p].TurnPositions == null || _playerStates[p].TurnPositions.Length != oldGhostPos.Length)
                            throw new RulesException("Ghosts positions specified incorrectly");
                        Point[] position = _playerStates[p].TurnPositions;
                        // Check each ghost individually
                        for (int ghost = 0; ghost < oldGhostPos.Length; ghost++)
                        {
                            // Movement distance and target
                            MovementDirection direction = GetMovementDirection(oldGhostPos[ghost], position[ghost]);
                            if (direction == MovementDirection.Invalid)
                                throw new RulesException($"Invalid movement distance or direction for ghost {ghost}");
                            if (direction == MovementDirection.Stay)
                                throw new RulesException($"Ghosts must move but ghost {ghost} stayed");
                            if (!_map.InBounds(position[ghost]))
                                throw new RulesException($"Move outside of map for ghost {ghost}");
                            if (_map[position[ghost]] == TileType.Wall)
                                throw new RulesException($"Move into an obstacle for ghost {ghost}");
                            // Movement direction
                            if (position[ghost].Equals(_lastGhostPosition[ghost]))
                            {
                                // Going back is only possible in a dead-end
                                int exits = 0;
                                for (int dr = -1; dr <= 1; dr++)
                                {
                                    for (int dc = -1; dc <= 1; dc++)
                                    {
                                        if (dr * dc != 0 || dr + dc == 0)
                                            continue;
                                        Point around = new Point((_map.GhostPosition[ghost].Row + dr + _map.Height) % _map.Height,
                                                                 (_map.GhostPosition[ghost].Col + dc + _map.Width) % _map.Width);
                                        if (_map[around] == TileType.Empty)
                                            exits++;
                                    }
                                }
                                if (exits > 1)
                                    throw new RulesException($"Illegal direction reverse for ghost {ghost}");
                            }
                            _lastGhostPosition[ghost] = _map.GhostPosition[ghost];
                            // Move is possible so we move
                            _map.GhostPosition[ghost] = position[ghost];
                        }
                        // If ghosts survived, award them points
                        _playerStates[p].Score = Settings.GameTurnLimit - _gameTurnStarted;
                    }
                }
                catch (RulesException re)
                {
                    _playerStates[p].Condition = PlayerCondition.Draw;
                    _playerStates[p].Comment = re.Message;
                    _proto.LogPlayerCondition(_playerStates[p]);
                    continue;
                }
            }

            // Check fair game finish conditions
            if (_playerStates.Where(p => p.IsActive).Count() > 1)
            {
                // Tecman losing conditions
                if (_playerStates[0].IsActive)
                {
                    // Direct capture of Tecman by sharing final position with a ghost
                    if (_map.GhostPosition.Any(p => p.Equals(_map.TecmanPosition)))
                    {
                        _playerStates[0].Condition = PlayerCondition.Draw;
                        _playerStates[0].Comment = "Tecman captured by ghosts";
                        _proto.LogPlayerCondition(_playerStates[0]);
                    }
                    else
                    {
                        // "In flight" capture when passing through each other
                        for (int ghost = 0; ghost < oldGhostPos.Length; ghost++)
                        {
                            if (oldGhostPos[ghost].Equals(_map.TecmanPosition)
                                && oldTecmanPos.Equals(_map.GhostPosition[ghost]))
                            {
                                _playerStates[0].Condition = PlayerCondition.Draw;
                                _playerStates[0].Comment = "Tecman captured by ghosts when passing";
                                _proto.LogPlayerCondition(_playerStates[0]);
                                break;
                            }
                        }
                    }
                }

                // Ghost losing conditions
                if (_playerStates[1].IsActive)
                {
                    // All cookies have been eaten
                    bool haveCookies = false;
                    foreach (TileType t in _map.Tiles)
                    {
                        if (t == TileType.Cookie)
                        {
                            haveCookies = true;
                            break;
                        }
                    }
                    if (!haveCookies)
                    {
                        _playerStates[1].Condition = PlayerCondition.Draw;
                        _playerStates[1].Comment = "All cookies have been eaten";
                        _proto.LogPlayerCondition(_playerStates[1]);
                    }
                }
            }

            // Handle game finish condition
            var activePlayers = _playerStates.Where(p => p.IsActive);
            if (activePlayers.Count() <= 1)
            {
                // Game finishes
                State = GameState.Finish;
                if (activePlayers.Count() == 1)
                {
                    // We have a winner
                    PlayerState winner = activePlayers.Single();
                    winner.Condition = PlayerCondition.Won;
                    winner.Comment = "Congratulations !";
                    _proto.LogPlayerCondition(winner);
                    foreach (PlayerState p in _playerStates)
                    {
                        if (p.Condition == PlayerCondition.Draw)
                        {
                            p.Condition = PlayerCondition.Lost;
                            _proto.LogPlayerCondition(p);
                        }
                    }
                }
                // else we are in a draw
            }
            else if (_gameTurnStarted >= Settings.GameTurnLimit)
            {
                // Game finishes in a draw because of turn limit
                State = GameState.Finish;
                foreach (PlayerState p in _playerStates)
                {
                    if (!p.IsActive)
                        continue;
                    p.Condition = PlayerCondition.Draw;
                    p.Comment = "Game turn limit reached";
                    _proto.LogPlayerCondition(p);
                }

            }

            // Complete the turn
            _gameTurnEnded = _gameTurnStarted;
            _proto.LogGameTurnEnd(_gameTurnEnded);

            // Notify observers
            ObservedTurnInfo ot = new ObservedTurnInfo
            {
                Turn = _gameTurnEnded,
                GameState = State,
                PlayerStates = _playerStates.Select(p => new PlayerStateInfo(p)).ToArray(),
                MapChanges = mapChanges.ToArray(),
                TecmanPosition = _map.TecmanPosition,
                GhostPositions = (Point[])_map.GhostPosition.Clone()
            };

            foreach (ObserverQueue queue in _observers)
            {
                queue.Push(ot);
            }

            // Continue
            StartNextTurnMaybe();
        }

        private MovementDirection GetMovementDirection(Point from, Point to)
        {
            int rowDiff = to.Row - from.Row;
            int colDiff = to.Col - from.Col;
            if (rowDiff == 0 && colDiff == 0)
                return MovementDirection.Stay;
            else if (rowDiff != 0 && colDiff != 0)
                return MovementDirection.Invalid;
            else if (rowDiff == 1 || rowDiff == 1 - _map.Height)
                return MovementDirection.Down;
            else if (rowDiff == -1 || rowDiff == _map.Height - 1)
                return MovementDirection.Up;
            else if (colDiff == 1 || colDiff == 1 - _map.Width)
                return MovementDirection.Right;
            else if (colDiff == -1 || colDiff == _map.Width - 1)
                return MovementDirection.Left;
            else
                return MovementDirection.Invalid;
        }

        public void WaitNextTurn(WaitTurnInfo wi)
        {
            lock (_liveLock)
            {
                StartNextTurnMaybe();

                if (PlayerExitTest(wi))
                    return;
                if (_gameTurnStarted > wi.Turn)
                {
                    wi.TurnComplete = true;
                    return;
                }

                CheckRunState();
                DateTime dtNow = DateTime.Now;
                if (_turnEnd > dtNow)
                {
                    int waitMillis = (int)(_turnEnd - dtNow).TotalMilliseconds;
                    if (waitMillis > Settings.NextTurnPollTimeoutMillis)
                        waitMillis = Settings.NextTurnPollTimeoutMillis;
                    else if (waitMillis < Settings.MinimumSleepMillis)
                        waitMillis = Settings.MinimumSleepMillis;
                    Monitor.Wait(_liveLock, waitMillis);
                }
                else
                {
                    Monitor.Wait(_liveLock, Settings.NextTurnPollTimeoutMillis);
                }

                StartNextTurnMaybe();

                if (PlayerExitTest(wi))
                    return;
                if (_gameTurnStarted > wi.Turn)
                    wi.TurnComplete = true;
            }
        }

        public void DropPlayer(Player player, string reason)
        {
            lock (_liveLock)
            {
                CheckRunState();
                int p = FindPlayer(player.PlayerId);
                if (!_playerStates[p].IsPresent)
                    return; // It can't happen but we should be ok anyway
                if (_playerStates[p].IsActive)
                {
                    _playerStates[p].Condition = PlayerCondition.Lost;
                    _playerStates[p].Comment = string.Format("Dropped from the game ({0})", reason);
                    _proto.LogPlayerCondition(_playerStates[p]);
                    CompleteTurn();
                }
                _playerStates[p].IsPresent = false;
                _proto.LogPlayerDrop(p, player.PlayerId, reason);
                player.Game = null;
            }
        }

        public void Pause()
        {
            lock (_liveLock)
            {
                CheckPlayState();
                State = GameState.Pause;
            }
        }

        public void Resume()
        {
            lock (_liveLock)
            {
                CheckPlayState();
                State = GameState.Play;
                Monitor.PulseAll(_liveLock);
            }
        }

        private void RemoveObserver(int observerId)
        {
            _observers.RemoveAll(q => q.Observer.ObserverId == observerId);
        }

        private GameViewInfo AddObserver(Observer observer)
        {
            _observers.Add(new ObserverQueue(observer));
            return GetGameView(-1);
        }

        public GameViewInfo StartObserving(Observer observer)
        {
            lock (_liveLock)
            {
                CheckRunState();
                RemoveObserver(observer.ObserverId);
                return AddObserver(observer);
            }
        }

        public ObservedGameInfo ObserveNextTurn(Observer observer)
        {
            ObserverQueue queue;
            List<int> lags = new List<int>();
            DateTime now = DateTime.Now;
            lock (_liveLock)
            {
                CheckRunState();
                queue = FindObserver(observer.ObserverId);
                for (int i = 0; i < _playerStates.Length; i++)
                {
                    if (_playerStates[i].TurnCompleted >= _gameTurnStarted)
                        continue;
                    double delayMillis = (now - _turnStart).TotalSeconds;
                    if (delayMillis > Settings.SlowTurnIntervalSeconds)
                        lags.Add(i);
                }
            }
            int queueLength;
            ObservedTurnInfo ot = queue.Pop(out queueLength); // This has its own blocking
            ObservedGameInfo gi = new ObservedGameInfo
            {
                GameId = GameId,
                GameState = State,
                QueuedTurns = queueLength,
                TurnInfo = ot,
                SlowPlayers = lags
            };
            return gi;
        }

        public GameLiveInfo GetLiveInfo()
        {
            lock (_liveLock)
            {
                CheckRunState();
                GameLiveInfo gi = new GameLiveInfo
                {
                    GameState = State,
                    Turn = _gameTurnStarted,
                    TurnStartTime = _turnStart
                };

                gi.PlayerStates = _playerStates.Select(p => new PlayerLiveInfo
                {
                    PlayerId = p.PlayerId,
                    Team = _players[p.Index].Team,
                    Name = _players[p.Index].Name,
                    Condition = p.Condition,
                    Comment = p.Comment,
                    TurnCompleted = p.TurnCompleted,
                    TurnFinTime = p.TurnFinTime,
                    PenaltyPoints = p.PenaltyPoints,
                    BonusPoints = p.BonusPoints,
                    OvertimeTurnMsec = p.OvertimeTurnMsec,
                    OvertimeTurnTurn = p.OvertimeTurnTurn,
                    PenaltyThresholdReachedTurn = p.PenaltyThresholdReachedTurn,
                    Score = p.Score

                }).ToArray();

                return gi;
            }
        }
    }

    public enum GameState
    {
        Setup, Play, Pause, Finish
    }

    public class PlayerState
    {
        public int Index;
        public int PlayerId;
        public PlayerCondition Condition;
        public bool IsActive => Condition == PlayerCondition.Play;
        public bool IsPresent;
        public string Comment;
        public int TurnCompleted;
        public Point[] TurnPositions;
        public PlayerMode Mode => (Index == 0) ? PlayerMode.TECMAN : PlayerMode.GHOSTS;

        public DateTime TurnFinTime;
        public int PenaltyPoints;
        public int BonusPoints;
        public int OvertimeTurnMsec;
        public int OvertimeTurnTurn;
        public int PenaltyThresholdReachedTurn;

        public int Score;
    }

    public enum PlayerCondition
    {
        Play, Won, Lost, Draw
    }

    internal enum MovementDirection
    {
        Invalid, Up, Down, Left, Right, Stay
    }
}
