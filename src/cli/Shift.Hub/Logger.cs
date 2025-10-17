using System.Reflection;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Shift.Hub
{
    internal static class Logger
    {
        private static ILogger? _instance;
        public static ILogger Instance => _instance ??= ConfigureLogger();

        static Serilog.Core.Logger ConfigureLogger()
        {
            var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(executingAssemblyPath) ?? throw new ArgumentNullException("directoryName");

            var filePath = Path.Combine(directoryPath, "Logs", "Log.txt");

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }
    }
}
