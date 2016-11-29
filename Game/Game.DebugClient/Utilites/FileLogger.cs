using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Prism.Logging;

namespace Game.DebugClient.Utilites
{
    public class FileLogger : ILoggerFacade
    {
        private readonly LogWriter _logWriter;

        public FileLogger()
        {
            var factory = new LogWriterFactory();
            _logWriter = factory.Create();
        }

        public void Log(string message, Category category, Priority priority)
        {
            int priorityValue;
            switch (priority)
            {
                case Priority.Low:
                    priorityValue = 10;
                    break;
                case Priority.Medium:
                    priorityValue = 50;
                    break;
                case Priority.High:
                    priorityValue = 100;
                    break;
                default:
                    priorityValue = 0;
                    break;
            }

            TraceEventType severity;
            switch (category)
            {
                case Category.Info:
                    severity = TraceEventType.Information;
                    break;
                case Category.Warn:
                    severity = TraceEventType.Warning;
                    break;
                case Category.Exception:
                    severity = TraceEventType.Error;
                    break;
                default:
                    severity = TraceEventType.Verbose;
                    break;
            }

            _logWriter.Write(message, string.Empty, priorityValue, default(int), severity);
        }
    }
}