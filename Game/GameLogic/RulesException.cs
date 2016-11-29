using System;
using System.Runtime.Serialization;

namespace GameLogic
{
    internal class RulesException : Exception
    {
        public RulesException(string message) : base(message)
        {
            // nothing more
        }
    }
}