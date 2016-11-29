using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    /// <summary>
    /// Server access restrictions for server development
    /// </summary>
    public class AccessManager : IAccessManager
    {
        private const string SHARED_SECRET = "SekPgijIL0zzWBQpiZJVRyXw/Gz4T3Ef";

        private object _lock;
        private Dictionary<string, AccessToken> _tokens;
        private RandomNumberGenerator _rng;

        public AccessManager()
        {
            _lock = new object();
            _tokens = new Dictionary<string, AccessToken>();
            _rng = RandomNumberGenerator.Create();
        }

        public void CheckAccess(string address)
        {
            AccessToken token;
            lock (_lock)
            {
                if (!_tokens.TryGetValue(address, out token))
                    token = null;
            }
            if (token == null || !token.IsValid)
                throw new AuthException("Access forbidden");
        }

        public AccessToken GetAccessToken(string address)
        {
            lock (_lock)
            {
                AccessToken token;
                if (_tokens.TryGetValue(address, out token))
                    return token;
                else
                    return null;
            }
        }

        private void InitChallenge(AccessToken token)
        {
            byte[] data = new byte[8];
            _rng.GetBytes(data);
            string challenge = BitConverter.ToString(data).Replace("-", "");
            token.SetupChallenge(challenge);
        }

        private bool CheckChallenge(AccessToken token, string request)
        {
            if (!token.IsChallengeValid)
                return false;
            string text = SHARED_SECRET + token.Challenge + SHARED_SECRET;
            string expected;
            using (var dig = HashAlgorithm.Create("SHA1"))
            {
                byte[] sig = Encoding.UTF8.GetBytes(text);
                expected = BitConverter.ToString(dig.ComputeHash(sig)).Replace("-", "").ToLower();
            }
            return expected == request;
        }

        public string RequestAccess(string address)
        {
            lock (_lock)
            {
                AccessToken token;
                if (!_tokens.TryGetValue(address, out token))
                {
                    token = new AccessToken(address);
                    _tokens[address] = token;
                }
                InitChallenge(token);
                return token.Challenge;
            }
        }

        public AccessToken GrantAccess(string address, string request)
        {
            lock (_lock)
            {
                AccessToken token;
                if (!_tokens.TryGetValue(address, out token))
                    return null;
                if (CheckChallenge(token, request))
                {
                    token.Grant();
                }
                return token;
            }
        }

        public string RequestRevoke(string address)
        {
            lock (_lock)
            {
                AccessToken token;
                if (!_tokens.TryGetValue(address, out token))
                    return "";
                InitChallenge(token);
                return token.Challenge;
            }
        }

        public bool RevokeAccess(string address, string request)
        {
            lock (_lock)
            {
                AccessToken token;
                if (!_tokens.TryGetValue(address, out token))
                    return true;
                if (!token.IsValid)
                    return true;
                if (CheckChallenge(token, request))
                {
                    token.Revoke();
                    return true;
                }
                else
                    return false;
            }
        }
    }

    public class AccessToken
    {
        public string ClientAddress { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public string Challenge { get; private set; }
        public DateTime ChallengeDate { get; private set; }

        public bool IsValid
        {
            get
            {
                return ExpirationDate > DateTime.UtcNow;
            }
        }

        public bool IsChallengeValid
        {
            get
            {
                return !string.IsNullOrEmpty(Challenge)
                    && ChallengeDate.AddSeconds(60) > DateTime.UtcNow;
            }
        }

        public AccessToken(string clientAddress)
        {
            ClientAddress = clientAddress;
        }

        public void SetupChallenge(string challenge)
        {
            Challenge = challenge;
            ChallengeDate = DateTime.UtcNow;
        }

        public void Grant()
        {
            ExpirationDate = DateTime.UtcNow.AddHours(1);
            Challenge = null;
        }

        public void Revoke()
        {
            ExpirationDate = DateTime.UtcNow;
            Challenge = null;
        }

        protected void MakePermanent()
        {
            ExpirationDate = new DateTime(3000, 1, 1);
        }
    }
}
