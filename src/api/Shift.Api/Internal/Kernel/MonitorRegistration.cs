using Sentry.Infrastructure;

namespace Shift.Api;

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
        private IShiftIdentityService _identityService;

        public SentryMonitor(ILog log, MonitoringSettings settings, IShiftIdentityService identityService)
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

                if (principal != null)
                {
                    if (principal.IPAddress != null)
                        scope.SetTag("principal-ip-address", principal.IPAddress);

                    if (principal.Name != null)
                        scope.SetTag("principal-name", principal.Name);

                    var user = principal.User;

                    if (user != null)
                    {
                        if (user.Email != null)
                            scope.SetTag("principal-user-email", user.Email);

                        if (user.Identifier != Guid.Empty)
                            scope.SetTag("principal-user-id", user.Identifier.ToString());

                        if (user.Name != null)
                            scope.SetTag("principal-user-name", user.Name);
                    }
                }

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
