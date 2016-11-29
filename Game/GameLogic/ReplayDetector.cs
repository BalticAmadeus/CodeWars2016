using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    /// <summary>
    /// Implementation is NOT synchronized. We rely on SessionManager for proper locking.
    /// </summary>
    public class ReplayDetector
    {
        private class ReplayToken
        {
            public string Hash;
            public DateTime UseTime;

            public ReplayToken(string hash)
            {
                Hash = hash ?? "";
                UseTime = DateTime.Now;
            }

            public override bool Equals(object obj)
            {
                ReplayToken that = obj as ReplayToken;
                if (that == null)
                    return false;
                return this.Hash == that.Hash;
            }

            public override int GetHashCode()
            {
                return Hash.GetHashCode();
            }
        }

        private Queue<ReplayToken> _queue;
        private HashSet<ReplayToken> _set;

        public ReplayDetector()
        {
            _queue = new Queue<ReplayToken>();
            _set = new HashSet<ReplayToken>();
        }

        public void CheckAndStore(string hash)
        {
            ReplayToken token = new ReplayToken(hash);
            if (_set.Contains(token))
                throw new AuthException("Message replay detected.");
            _set.Add(token);
            _queue.Enqueue(token);
            CleanOldTokens();
        }

        private void CleanOldTokens()
        {
            DateTime staleBarrier = DateTime.Now.AddSeconds(-Settings.ReplayDetectionWindowSeconds);
            while (_queue.Count > 0 && _queue.Peek().UseTime < staleBarrier)
            {
                ReplayToken token = _queue.Dequeue();
                _set.Remove(token);
            }
        }
    }
}
