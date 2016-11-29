using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class SessionManager
    {
        private TeamRegistry _teamReg;
        private Dictionary<ClientCode, SessionChallenge> _challenges;
        private Dictionary<ClientCode, ClientSession> _sessions;
        private ReplayDetector _replayDetector;
        private RandomNumberGenerator _rng;
        
        public SessionManager(TeamRegistry teamReg)
        {
            _teamReg = teamReg;
            _challenges = new Dictionary<ClientCode, SessionChallenge>();
            _sessions = new Dictionary<ClientCode, ClientSession>();
            _replayDetector = new ReplayDetector();
            _rng = RandomNumberGenerator.Create();
        }

        public void Authenticate(SessionAuth auth, SessionAuthOptions options)
        {
            Team team = _teamReg.GetTeam(auth.TeamName);
            string authCode = "";
            if (team.Authenticate)
            {
                authCode = checkAuthCode(team, auth);
                if (!options.IsLoginFlow)
                {
                    lock (_replayDetector)
                    {
                        _replayDetector.CheckAndStore(authCode);
                    }
                }
            }
            ClientCode clientCode = auth.GetClientCode();
            if (options.IsLoginFlow)
            {
                if (auth.SessionId != 0 || auth.SequenceNumber != 0)
                    throw new AuthException("For login calls, SessionId and SequenceNumber must be zero.");
            }
            else
            {
                lock (_sessions)
                {
                    ClientSession session;
                    if (!_sessions.TryGetValue(clientCode, out session))
                    {
                        session = new ClientSession(auth.SessionId, clientCode);
                        _sessions[clientCode] = session;
                    }
                    if (session.SessionId != auth.SessionId)
                    {
                        session.Restart(auth.SessionId);
                    }
                    session.Update();
                }
            }
        }

        private string getAuthCode(string payload, string secret)
        {
            payload = payload ?? "";
            // We do not want to use full HMAC to make it easier for implementers
            var dig = HashAlgorithm.Create("SHA1");
            var utf8 = Encoding.UTF8;
            byte[] sig = utf8.GetBytes(payload + secret);
            string hex = BitConverter.ToString(dig.ComputeHash(sig)).Replace("-", "").ToLower();
            return hex;
        }

        private string checkAuthCode(Team team, SessionAuth auth)
        {
            string authCode = (auth.AuthCode != null) ? auth.AuthCode.ToLower() : "";
            string hex = getAuthCode(auth.GetAuthString(), team.Secret);
            if (hex != authCode)
            {
                //log.Warn(string.Format("AuthCode mismatch: expected={0}, auth={1}", hex, auth));
                throw new AuthException("AuthCode mismatch");
            }
            return authCode;
        }

        public SessionChallenge CreateChallenge(ClientCode clientCode)
        {
            clientCode.Validate();
            Team team = _teamReg.GetTeam(clientCode.TeamName);
            lock (_challenges)
            {
                SessionChallenge challenge;
                if (_challenges.TryGetValue(clientCode, out challenge))
                {
                    if (!challenge.IsTimedOut)
                        throw new AuthException("Previous challenge has not timed out. Complete login or wait for timeout.");
                }
                challenge = new SessionChallenge(clientCode);
                _challenges[clientCode] = challenge;
                return challenge;
            }
        }

        private int generateSessionId()
        {
            byte[] data = new byte[4];
            _rng.GetBytes(data);
            return BitConverter.ToInt32(data, 0) & int.MaxValue;
        }

        public ClientSession CreateSession(ClientCode clientCode, string challengeResponse)
        {
            Team team = _teamReg.GetTeam(clientCode.TeamName);
            lock (_challenges)
            {
                SessionChallenge challenge;
                if (!_challenges.TryGetValue(clientCode, out challenge))
                    throw new AuthException("No outstanding challenge for this client. Init login first.");
                if (challenge.IsTimedOut)
                    throw new AuthException("Challenge timed out. Init login again and complete login with less delay.");
                string correctResponse = getAuthCode(getAuthCode(challenge.Challenge, team.Secret), team.Secret);
                if (team.Authenticate && (challengeResponse != correctResponse))
                {
                    // TODO Log
                    throw new AuthException("Challenge response mismatch.");
                }
                _challenges.Remove(clientCode);
            }
            lock (_sessions)
            {
                ClientSession session;
                if (_sessions.TryGetValue(clientCode, out session))
                {
                    session.Restart(generateSessionId());
                }
                else
                {
                    session = new ClientSession(generateSessionId(), clientCode);
                    _sessions[clientCode] = session;
                }
                return session;
            }
        }

    }

    public class SessionChallenge
    {
        public ClientCode ClientCode { get; private set; }
        public string Challenge { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public bool IsTimedOut
        {
            get
            {
                return (DateTime.Now - TimeStamp) > TimeSpan.FromSeconds(10);
            }
        }

        public SessionChallenge(ClientCode clientCode)
        {
            ClientCode = clientCode;
            Challenge = Guid.NewGuid().ToString();
            TimeStamp = DateTime.Now;
        }
    }

    public class ClientSession
    {
        public int SessionId { get; private set; }
        public ClientCode ClientCode { get; private set; }
        public DateTime LastCall { get; private set; }

        private int[] _oldSessions;
        private int _oldSessionCount;

        public ClientSession(int sessionId, ClientCode clientCode)
        {
            SessionId = sessionId;
            ClientCode = clientCode;
            LastCall = DateTime.Now;
            _oldSessions = new int[10];
            _oldSessionCount = 0;
        }

        public void Restart(int sessionId)
        {
            int p = Array.IndexOf(_oldSessions, sessionId, 0, _oldSessionCount);
            if (p >= 0)
            {
                // Bubble up to keep bouncing it longer
                Array.Copy(_oldSessions, p + 1, _oldSessions, p, _oldSessionCount - p - 1);
                _oldSessions[_oldSessionCount - 1] = sessionId;
                throw new AuthException("This session is stale, there is a newer session for this client");
            }
            if (_oldSessionCount < _oldSessions.Length)
            {
                _oldSessions[_oldSessionCount] = SessionId;
                _oldSessionCount++;
            }
            else
            {
                // Drop oldest
                Array.Copy(_oldSessions, 1, _oldSessions, 0, _oldSessionCount - 1);
                _oldSessions[_oldSessionCount - 1] = SessionId;
            }
            SessionId = sessionId;
            LastCall = DateTime.Now;
        }

        public void Update()
        {
            LastCall = DateTime.Now;
        }
    }

}
