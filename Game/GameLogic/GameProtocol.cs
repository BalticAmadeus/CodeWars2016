using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class GameProtocol : IDisposable
    {
        private TextWriter _logOut;
        private int _gameId;

        public GameProtocol(GameModel game)
        {
            FileStream logFile = File.Open(string.Format(@"{0}\{1:yyyyMMdd_HHmmss}_{2}_protocol.txt", Settings.GameProtocolDir, DateTime.Now, game.GameId),
                FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            _logOut = new StreamWriter(logFile, Encoding.UTF8);
            _gameId = game.GameId;
            LogMessage("INIT", string.Format("Game initialized (owner={0})", game.Owner));
        }

        public void Dispose()
        {
            _logOut.Dispose();
        }

        public void LogMessage(string opCode, string message)
        {
            string line = string.Format("[{0:o}] {1,5:d} {2,-8} {3}", DateTime.Now, _gameId, opCode, message);
            _logOut.WriteLine(line);
            _logOut.Flush(); //TODO: Log is empty if commented. Should we Flush periodically or on each call? //tmaconko
        }

        public void LogMessage(string opCode, string format, params object[] args)
        {
            LogMessage(opCode, string.Format(format, args));
        }

        public void LogGameStart(GameModel game)
        {
            int i = 0;
            foreach (Player p in game.ListPlayers())
            {
                LogMessage("PLAYER", "{0} - [{1}] {2} ({3})", i++, p.Team, p.Name, p.PlayerId);
            }
            LogMessage("START", "Game started");
        }

        public void LogMove(int index, int playerId, Point[] positions)
        {
            LogMessage("MOVE", "Player {0} (id={1}) {2}", index, playerId, (positions == null) ? "(none)" : string.Concat(positions));
        }

        public void LogPlayerTurnComplete(PlayerState p, DateTime gameTurnStarted)
        {
            LogMessage("PLRTURN", "Player {0} (id={1}) completed turn {2} in {3} ms (penalty={4}, bonus={5}{6}{7})",
                p.Index, p.PlayerId, p.TurnCompleted, (int)(p.TurnFinTime - gameTurnStarted).TotalMilliseconds, p.PenaltyPoints, p.BonusPoints,
                ((p.OvertimeTurnTurn == p.TurnCompleted) ? string.Format(", overtimeturn={0}", p.OvertimeTurnTurn) : ""),
                ((p.PenaltyThresholdReachedTurn > 0) ? string.Format(", penaltyThreasholdReachedTurn={0}", p.PenaltyThresholdReachedTurn) : ""));
        }

        public void LogGameTurnStart(int turn)
        {
            LogMessage("TURNSTRT", "Game turn {0} started", turn);
        }

        public void LogGameTurnEnd(int turn)
        {
            LogMessage("TURNEND", "Game turn {0} ended", turn);
        }

        public void LogPlayerCondition(PlayerState p)
        {
            LogMessage("PLRCOND", "Player {0} (id={1}) condition {2} ({3})", p.Index, p.PlayerId, p.Condition, p.Comment);
        }

        public void LogPlayerDrop(int index, int playerId, string reason)
        {
            LogMessage("DROP", "Player {0} (id={1}) dropped from the game ({2})", index, playerId, reason);
        }

        public void LogGameState(GameState state)
        {
            LogMessage("GAMESTAT", "Game {0} state is {1}", _gameId, state);
        }
    }
}
