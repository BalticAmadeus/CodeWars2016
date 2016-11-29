using System;
using Prism.Logging;

namespace Game.DemoClient
{
    public class Logger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            Console.WriteLine(message);
        }
    }
}
