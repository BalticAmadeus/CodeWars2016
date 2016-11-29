using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.DebugClient.Infrastructure
{
    public class RandomSessionIdGenerator : ISessionIdGenerator
    {
        private Random _rng;

        public RandomSessionIdGenerator()
        {
            _rng = new Random();
        }

        public int NextSessionId()
        {
            return _rng.Next(int.MaxValue - 1) + 1;
        }
    }
}
