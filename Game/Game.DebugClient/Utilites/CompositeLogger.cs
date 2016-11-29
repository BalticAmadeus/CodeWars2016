using System.Collections.Generic;
using Prism.Logging;

namespace Game.DebugClient.Utilites
{
    public class CompositeLogger : ILoggerFacade
    {
        private readonly IEnumerable<ILoggerFacade> _loggers;

        public CompositeLogger(IEnumerable<ILoggerFacade> loggers)
        {
            _loggers = loggers;
        }

        public void Log(string message, Category category, Priority priority)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(message, category, priority);
            }
        }
    }
}