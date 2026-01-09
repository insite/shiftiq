using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Sentry.Infrastructure;

using Shift.Common;

namespace Engine.Common
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

                // If Sentry's Debug setting is enabled with a FileDiagnosticLogger then the logger
                // sometimes throws an unhandled exception from here:
                // Sentry.Integrations.AppDomainProcessExitIntegration.HandleProcessExit.

                // The exception is a System.ObjectDisposedException with Message = "Cannot write to a
                // closed TextWriter". We need to disable Sentry's default automatic behaviour and
                // manually flush any queued events before application shutdown.

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

                var sentryLevel = MapToSentryLevel(level);

                var eventId = SentrySdk.CaptureMessage(message, sentryLevel);

                return CreateSentryEventUri(new Uri(Settings.BaseUrl), Settings.Project, eventId.ToString());
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
