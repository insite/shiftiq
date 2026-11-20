using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Sentry.Infrastructure;

using Shift.Common;

namespace Shift.Hub
{
    public static class MonitorRegistration
    {
        public static IServiceCollection AddMonitoring(this IServiceCollection services, MonitoringSettings monitoring, ReleaseSettings release)
        {
            services.AddSingleton<ILog, Log>();

            services.AddSingleton<IMonitor, SentryMonitor>();

            if (monitoring.Enabled)
                InitSentry(services, monitoring, release);

            return services;
        }

        private static void InitSentry(IServiceCollection services, MonitoringSettings monitoring, ReleaseSettings release)
        {
            var dsn = monitoring.Enabled
                ? monitoring.Dsn
                : SentryConstants.DisableSdkDsnValue;

            SentrySdk.Init(options =>
            {
                if (Uri.TryCreate(monitoring.Dsn, UriKind.Absolute, out Uri? uriResult))
                    if (uriResult.Scheme == Uri.UriSchemeHttp)
                        dsn = uriResult.ToString();

                options.AutoSessionTracking = true;
                options.DiagnosticLevel = SentryLevel.Info;
                options.Dsn = dsn;
                options.Environment = release.Environment.ToString().ToLower();
                options.Release = release.Version;
                options.SendDefaultPii = true;

                if (0.0 <= monitoring.Rate && monitoring.Rate <= 1.0)
                {
                    options.TracesSampleRate = monitoring.Rate;
                    options.ProfilesSampleRate = monitoring.Rate;
                }

                if (monitoring.Debug)
                {
                    var path = ProcessHelper.InitializeMonitoring(monitoring.File);

                    options.Debug = path.IsNotEmpty();

                    if (options.Debug)
                        options.DiagnosticLogger = new FileDiagnosticLogger(path);
                }

                services.AddSentryTunneling();

                // Manually flush Sentry before shutdown to avoid the AppDomain exit hook's
                // ObjectDisposedException on FileDiagnosticLogger.
                options.DisableAppDomainProcessExitFlush();
            });
        }

        public class SentryMonitor : BaseMonitor
        {
            public SentryMonitor(ILog log, MonitoringSettings settings) : base(log, settings) { }

            protected override Uri? OnLogMessage(string message, object[] args, LogLevel level)
            {
                if (!Settings.Enabled)
                    return null;

                // Send the template with its arguments, not the bare template. Sentry keeps grouping
                // on the template and titles each event with the rendered text, so a warning names
                // its method, path and caller instead of showing {Method} {Path} from {Caller}.
                var sentryEvent = new SentryEvent
                {
                    Level = MapToSentryLevel(level),
                    Message = new SentryMessage
                    {
                        Message = message,
                        Params = args,
                        Formatted = Render(message, args)
                    }
                };

                var eventId = SentrySdk.CaptureEvent(sentryEvent);

                return CreateSentryEventUri(new Uri(Settings.BaseUrl), Settings.Project, eventId.ToString());
            }

            // Renders a Serilog message template the same way the log file does, so the Sentry event
            // and the log line read alike. Falls back to the bare template when it cannot be bound.
            private static string Render(string message, object[] args)
            {
                if (args == null || args.Length == 0)
                    return message;

                var bound = Serilog.Log.Logger.BindMessageTemplate(message, args, out var template, out var properties);

                if (!bound || template == null || properties == null)
                    return message;

                return template.Render(properties.ToDictionary(p => p.Name, p => p.Value));
            }

            protected override Uri? OnLogException(Exception exception)
            {
                if (!Settings.Enabled)
                    return null;

                var eventId = SentrySdk.CaptureException(exception);

                return CreateSentryEventUri(new Uri(Settings.BaseUrl), Settings.Project, eventId.ToString());
            }

            private SentryLevel MapToSentryLevel(LogLevel level)
            {
                switch (level)
                {
                    case LogLevel.Trace: return SentryLevel.Debug;
                    case LogLevel.Debug: return SentryLevel.Debug;
                    case LogLevel.Information: return SentryLevel.Info;
                    case LogLevel.Warning: return SentryLevel.Warning;
                    case LogLevel.Error: return SentryLevel.Error;
                    case LogLevel.Critical: return SentryLevel.Fatal;
                    default: return SentryLevel.Info;
                }
            }

            public override void Flush() => SentrySdk.Flush(TimeSpan.FromSeconds(5));

            public override async Task FlushAsync() => await SentrySdk.FlushAsync(TimeSpan.FromSeconds(5));
        }
    }
}
