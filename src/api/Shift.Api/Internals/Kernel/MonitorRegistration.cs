using Sentry.Infrastructure;

namespace Shift.Api;

public static class MonitorRegistration
{
    public static void UseMonitoring(this WebApplicationBuilder builder, MonitoringSettings monitoring, ReleaseSettings release)
    {
        builder.Services.AddSingleton<ILog, Log>();

        builder.Services.AddSingleton<IMonitor, SentryMonitor>();

        if (monitoring.Enabled)
            InitSentry(builder, monitoring, release);
    }

    private static void InitSentry(WebApplicationBuilder builder, MonitoringSettings monitoring, ReleaseSettings release)
    {
        var dsn = monitoring.Enabled
            ? monitoring.Dsn
            : SentryConstants.DisableSdkDsnValue;

        builder.WebHost.UseSentry(options =>
        {
            if (Uri.TryCreate(monitoring.Dsn, UriKind.Absolute, out Uri? uriResult))
            {
                if (uriResult.Scheme == Uri.UriSchemeHttp)
                    dsn = uriResult.ToString();
            }

            options.AttachStacktrace = true;
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

            // If Sentry's Debug setting is enabled with a FileDiagnosticLogger then the logger
            // sometimes throws an unhandled exception from here:
            // Sentry.Integrations.AppDomainProcessExitIntegration.HandleProcessExit.

            // The exception is a System.ObjectDisposedException with Message = "Cannot write to a
            // closed TextWriter". We need to disable Sentry's default automatic behaviour and
            // manually flush any queued events before application shutdown.

            options.DisableAppDomainProcessExitFlush();
        });

        builder.Services.AddSentryTunneling();
    }

    public class SentryMonitor : BaseMonitor
    {
        private IPrincipalProvider _identityService;

        public SentryMonitor(ILog log, MonitoringSettings settings, IPrincipalProvider identityService)
            : base(log, settings)
        {
            _identityService = identityService;
        }

        protected override Uri? OnLogMessage(string message, object[] args, LogLevel level)
        {
            if (!Settings.Enabled || IgnoreError(message))
                return null;

            var sentryLevel = MapToSentryLevel(level);

            var eventId = SentrySdk.CaptureMessage(message, sentryLevel);

            return CreateSentryEventUri(new Uri(Settings.BaseUrl), Settings.Project, eventId.ToString());
        }

        protected override Uri? OnLogException(Exception exception)
        {
            if (!Settings.Enabled || IgnoreError(exception.Message))
                return null;

            var eventId = SentrySdk.CaptureException(exception, scope =>
            {
                var principal = _identityService.GetPrincipal();
                if (principal == null)
                    return;

                var user = principal.User;

                scope.User = new SentryUser
                {
                    Id = user != null && user.Identifier != Guid.Empty
                        ? user.Identifier.ToString()
                        : null,

                    Username = user?.Email ?? principal.Name,
                    IpAddress = principal.IPAddress
                };
            });

            return CreateSentryEventUri(new Uri(Settings.BaseUrl), Settings.Project, eventId.ToString());
        }

        private SentryLevel MapToSentryLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => SentryLevel.Debug,
                LogLevel.Debug => SentryLevel.Debug,
                LogLevel.Information => SentryLevel.Info,
                LogLevel.Warning => SentryLevel.Warning,
                LogLevel.Error => SentryLevel.Error,
                LogLevel.Critical => SentryLevel.Fatal,
                _ => SentryLevel.Info
            };
        }

        public override void Flush() => SentrySdk.Flush(TimeSpan.FromSeconds(5));

        public override async Task FlushAsync() => await SentrySdk.FlushAsync(TimeSpan.FromSeconds(5));

        private static bool IgnoreError(string message)
        {
            return !string.IsNullOrEmpty(message) && message.Contains("The client has disconnected");
        }
    }
}
