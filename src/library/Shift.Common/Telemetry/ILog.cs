using Serilog;

namespace Shift.Common
{
    public interface ILog
    {
        void Critical(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Error(string message, params object[] args);
        void Information(string message, params object[] args);
        void Trace(string message, params object[] args);
        void Warning(string message, params object[] args);
    }

    public class Log : ILog
    {
        public readonly ILogger Logger;

        public Log(ILogger logger)
        {
            Logger = logger;
        }

        public void Trace(string message, params object[] args) => Logger.Debug(message, args);
        public void Debug(string message, params object[] args) => Logger.Debug(message, args);
        public void Information(string message, params object[] args) => Logger.Information(message, args);
        public void Warning(string message, params object[] args) => Logger.Warning(message, args);
        public void Error(string message, params object[] args) => Logger.Error(message, args);
        public void Critical(string message, params object[] args) => Logger.Fatal(message, args);
    }
}
