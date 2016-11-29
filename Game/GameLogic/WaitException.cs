using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class WaitException : Exception
    {
        public WaitException()
            : base("Wait for next turn")
        {
            // nothing more
        }
    }
}
