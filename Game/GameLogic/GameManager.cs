using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameLogic
{
    /// <summary>
    /// Locking strategy for live game access (e.g. by players): _gameLock -> _liveLock or _gameLock, _liveLock
    /// Observer queue is controlled from _liveLock -> _observerQueueLock,
    /// but waiting on empty queue/dequeueing is on _observerQueueLock only
    /// </summary>
    public class GameManager
    {
        private Dictionary<int, GameModel> _games;
        private Dictionary<int, Player> _players;
        private Dictionary<int, Observer> _observers;
        private object _gameLock;
        private int nextGameId;
        private int nextPlayerId;
        private int nextObserverId;
        private long uidBase;

        public GameManager()
        {
            _games = new Dictionary<int, GameModel>();
            _players = new Dictionary<int, Player>();
            _observers = new Dictionary<int, Observer>();
            _gameLock = new object();
            nextGameId = 1;
            nextPlayerId = 1;
            nextObserverId = 1;
            uidBase = DateTime.UtcNow.Ticks / 10000000 * 10000000;
        }

        public GameInfo[] ListGames(Team team)
        {
            lock (_gameLock)
            {

                return _games.Values
                    .Where(g => team.PowerTeam || team.ObserverTeam || g.Owner.Equals(team))
                    .Select(g => new GameInfo(g))
                    .ToArray();
            }
        }

        public GameInfo CreateGame(Team owner)
        {
            lock (_gameLock)
            {
                // Let's not impose any limits as game creation is a manual process
                //if (!owner.PowerTeam)
                //{
                //    int count = _games.Values.Count(g => g.Owner.Equals(owner));
                //    if (count >= Settings.MaxGamesPerTeam)
                //        throw new ApplicationException("Maximum limit of games per owner team has been reached");
                //}
                checkGameCreate(owner);
                var game = new GameModel(uidBase, nextGameId++, owner);
                _games[game.GameId] = game;
                return new GameInfo(game);
            }
        }

        public GameDetails GetGameDetails(int gameId, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameAccess(game, team);
                return new GameDetails(game);
            }
        }

        public PlayerInfo[] ListPlayers(Team team)
        {
            lock (_gameLock)
            {
                return _players.Values
                    .Where(p => team.PowerTeam || p.Team.Equals(team))
                    .Select(p => new PlayerInfo(p))
                    .ToArray();
            }
        }

        public PlayerInfo CreatePlayer(Team team, string name)
        {
            checkPlayerCreate(team);
            lock (_gameLock)
            {
                Player player = _players.Values.FirstOrDefault(p => p.Name == name && p.Team.Equals(team));
                if (player == null)
                {
                    player = new Player(nextPlayerId++, team, name);
                    _players[player.PlayerId] = player;
                }
                return new PlayerInfo(player);
            }
        }

        public ObserverInfo CreateObserver(Team team, string name)
        {
            lock (_gameLock)
            {
                Observer observer = _observers.Values.FirstOrDefault(p => p.Name == name && p.Team.Equals(team));
                if (observer == null)
                {
                    observer = new Observer(nextObserverId++, team, name);
                    _observers[observer.ObserverId] = observer;
                }
                return new ObserverInfo(observer);
            }
        }

        public GameInfo WaitGameStart(int playerId, ClientCode clientCode)
        {
            lock (_gameLock)
            {
                for (int i = 0; i < 2; i++)
                {
                    Player player = getPlayer(playerId);
                    checkPlayerAccess(player, clientCode);
                    GameModel game = player.Game;
                    if (game != null && game.State != GameState.Setup)
                        return new GameInfo(game);
                    if (i == 0)
                        Monitor.Wait(_gameLock, Settings.GameStartPollTimeoutMillis);
                }
                return null;
            }
        }

        public void AddGamePlayer(int gameId, int playerId, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                Player player = getPlayer(playerId);
                checkPlayerAccess(player, team);
                game.AddPlayer(player);
            }
        }

        public void RemoveGamePlayer(int gameId, int playerId, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                Player player = getPlayer(playerId);
                game.RemovePlayer(player);
            }
        }

        private GameModel getGame(int gameId)
        {
            GameModel game;
            if (!_games.TryGetValue(gameId, out game))
                throw new ApplicationException("Game not found");
            return game;
        }

        private Player getPlayer(int playerId)
        {
            Player player;
            if (!_players.TryGetValue(playerId, out player))
                throw new ApplicationException("Player not found");
            // TODO Add access check here as for observer???
            return player;
        }

        private Observer getObserver(int observerId, ClientCode clientCode)
        {
            Observer observer;
            if (!_observers.TryGetValue(observerId, out observer))
                throw new ApplicationException("Observer not found");
            if (observer.Name != clientCode.ClientName || observer.Team.Name != clientCode.TeamName)
                throw new UnauthorizedAccessException();
            return observer;
        }

        /// <summary>
        /// General game access check. Any viewing of a game must first pass this check.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="team"></param>
        private void checkGameAccess(GameModel game, Team team)
        {
            if (!team.PowerTeam && !team.ObserverTeam && !game.Owner.Equals(team))
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Game manipulation check. Changing of game parameters during setup or pause/resume.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="team"></param>
        private void checkGameChange(GameModel game, Team team)
        {
            if (!team.PowerTeam && !game.Owner.Equals(team))
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Checks who can create games.
        /// </summary>
        /// <param name="team"></param>
        private void checkGameCreate(Team team)
        {
            if (team.ObserverTeam)
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Checks if a team can create players.
        /// </summary>
        /// <param name="team"></param>
        private void checkPlayerCreate(Team team)
        {
            if (team.PowerTeam || team.ObserverTeam)
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Ensures the authenticated client is the same as requested player.
        /// TODO Rename to checkPlayerAction or something.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="clientCode"></param>
        private void checkPlayerAccess(Player player, ClientCode clientCode)
        {
            if (player.Name != clientCode.ClientName || player.Team.Name != clientCode.TeamName)
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Checks if a team can perform administrative tasks on this player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="team"></param>
        private void checkPlayerAccess(Player player, Team team)
        {
            if (!team.PowerTeam && !player.Team.Equals(team))
                throw new UnauthorizedAccessException();
        }

        public void SetGameMap(int gameId, MapData mapData, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                game.SetMap(mapData);
            }
        }

        public void StartGame(int gameId, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                game.Start();
                // Notify those blocked on WaitGameStart
                Monitor.PulseAll(_gameLock);
            }
        }

        private GameModel accessLiveGame(int playerId, ClientCode clientCode)
        {
            lock (_gameLock)
            {
                Player player = getPlayer(playerId);
                checkPlayerAccess(player, clientCode);
                GameModel game = player.Game;
                if (game == null)
                    throw new ApplicationException("Player is not in a game");
                game.CheckRunState();
                return game;
            }
        }

        private void accessObservedGame(int observerId, int gameId, ClientCode clientCode, Team team, out Observer observer, out GameModel game)
        {
            lock (_gameLock)
            {
                observer = getObserver(observerId, clientCode);
                game = getGame(gameId);
                checkGameAccess(game, team);
                game.CheckRunState();
            }
        }

        public GameViewInfo GetPlayerView(int playerId, ClientCode clientCode)
        {
            GameModel game = accessLiveGame(playerId, clientCode);
            return game.GetGameView(playerId);
        }

        public void PerformMove(int playerId, Point[] positions, ClientCode clientCode)
        {
            GameModel game = accessLiveGame(playerId, clientCode);
            game.PerformMove(playerId, positions);
        }

        public WaitTurnInfo WaitNextTurn(int playerId, int refTurn, ClientCode clientCode, DateTime callTimestamp)
        {
            while (refTurn == 0)
            {
                // Entering the game
                GameInfo gameInfo = WaitGameStart(playerId, clientCode);
                if (gameInfo == null)
                {
                    // Not yet in a game
                    return new WaitTurnInfo();
                }
                else if (gameInfo.State == GameState.Finish)
                {
                    // In a finished game
                    LeaveGame(playerId, clientCode);
                    continue;
                }
                // In a running game -- fall through
                break;
            }
            GameModel game = accessLiveGame(playerId, clientCode);
            WaitTurnInfo wi = game.CompletePlayerTurn(playerId, refTurn, callTimestamp);

            if (wi.TurnComplete == false)
                game.WaitNextTurn(wi);

            if (wi.GameFinished)
                Thread.Sleep(Settings.LastWaitNextTurnSleepMillis);

            return wi;
        }

        public void DropPlayer(int playerId, int gameId, ClientCode clientCode, Team team)
        {
            lock (_gameLock)
            {
                Player player = getPlayer(playerId);
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                if (player.Game != game)
                    throw new ApplicationException("Player is not in this game");
                game.DropPlayer(player, clientCode.ToString());
            }
        }

        public void LeaveGame(int playerId, ClientCode clientCode)
        {
            lock (_gameLock)
            {
                Player player = getPlayer(playerId);
                checkPlayerAccess(player, clientCode);
                GameModel game = player.Game;
                if (game == null)
                    return;
                game.DropPlayer(player, clientCode.ToString());
            }
        }

        public void PauseGame(int gameId, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                game.Pause();
            }
        }

        public void ResumeGame(int gameId, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                game.Resume();
            }
        }

        public GameViewInfo StartObserving(int observerId, int gameId, ClientCode clientCode, Team team)
        {
            Observer observer;
            GameModel game;
            accessObservedGame(observerId, gameId, clientCode, team, out observer, out game);
            return game.StartObserving(observer);
        }

        public ObservedGameInfo ObserveNextTurn(int observerId, int gameId, ClientCode clientCode, Team team)
        {
            Observer observer;
            GameModel game;
            accessObservedGame(observerId, gameId, clientCode, team, out observer, out game);
            return game.ObserveNextTurn(observer);
        }

        public void DeleteGame(int gameId, Team team)
        {
            lock (_gameLock)
            {
                GameModel game = getGame(gameId);
                checkGameChange(game, team);
                game.CheckGameDeletable();
                _games.Remove(game.GameId);
                game.Dispose();
            }
        }

        public GameLiveInfo GetLiveInfo(int gameId, Team team)
        {
            GameModel game;
            lock (_gameLock)
            {
                game = getGame(gameId);
                checkGameAccess(game, team);
            }
           
            return game.GetLiveInfo();
        }

    }
}
