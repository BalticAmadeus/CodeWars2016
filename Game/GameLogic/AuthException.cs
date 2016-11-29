using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class AuthException : Exception
    {
        public AuthException(string message)
            : base(message)
        {
            // nothing more
        }
    }
}
