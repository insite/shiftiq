using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shift.Common
{
    public interface IMonitor
    {
        ILog Log { get; }

        Uri Critical(string message, params object[] args);
        Uri Error(string message, params object[] args);
        Uri Error(Exception exception);
        Uri Warning(string message, params object[] args);
        Uri Information(string message, params object[] args);
        Uri Debug(string message, params object[] args);
        Uri Trace(string message, params object[] args);

        void Flush();
        Task FlushAsync();
    }

    // Base class with no external dependencies
    public abstract class BaseMonitor : IMonitor
    {
        private readonly ILog _log;

        protected readonly MonitoringSettings Settings;

        protected BaseMonitor(ILog log, MonitoringSettings settings)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));

            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public ILog Log => _log;

        public Uri Trace(string message, params object[] args)
            => LogMessage(message, args, LogLevel.Trace, _log.Trace);

        public Uri Debug(string message, params object[] args)
            => LogMessage(message, args, LogLevel.Debug, _log.Debug);

        public Uri Information(string message, params object[] args)
            => LogMessage(message, args, LogLevel.Information, _log.Information);

        public Uri Warning(string message, params object[] args)
            => LogMessage(message, args, LogLevel.Warning, _log.Warning);

        public Uri Error(string message, params object[] args)
            => LogMessage(message, args, LogLevel.Error, _log.Error);

        public Uri Error(Exception exception)
            => OnLogException(exception);

        public Uri Critical(string message, params object[] args)
            => LogMessage(message, args, LogLevel.Critical, _log.Critical);

        private Uri LogMessage(string message, object[] args, LogLevel level, Action<string, object[]> logAction)
        {
            logAction(message, args);
            return OnLogMessage(message, args, level);
        }

        /// <summary>
        /// Override this method to implement custom logging behavior (e.g., sending to Sentry)
        /// </summary>
        protected abstract Uri OnLogMessage(string message, object[] args, LogLevel level);

        protected abstract Uri OnLogException(Exception exception);

        public abstract void Flush();

        public abstract Task FlushAsync();

        /// <summary>
        /// Enum for log levels - independent of any external library
        /// </summary>
        protected enum LogLevel
        {
            Trace,
            Debug,
            Information,
            Warning,
            Error,
            Critical
        }

        /// <summary>
        /// Creates URI directly to the event detail page
        /// </summary>
        /// <param name="baseUri">URL for your Sentry organization account</param>
        /// <param name="projectId">Your Sentry project slug</param>
        /// <param name="eventId">The event ID returned from SentrySdk.CaptureMessage</param>
        /// <returns>Complete URI to the specific Sentry event</returns>
        public Uri CreateSentryEventUri(Uri baseUri, string projectId, string eventId)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));

            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));

            // TODO: Check for string of zeros
            // if (eventId == SentryId.Empty)
            //    throw new ArgumentException("Event ID cannot be empty", nameof(eventId));

            var organizationId = ExtractSubdomain(baseUri);

            var relativePath = $"organizations/{organizationId}/projects/{projectId}/events/{eventId}/";

            var sentryUri = RemoveSubdomain(baseUri);

            return new Uri(sentryUri, relativePath);
        }

        protected string ExtractSubdomain(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            // Pattern matches: protocol://subdomain.domain.tld/path
            // and replaces with: subdomain

            var pattern = @"^(https?://)([^.]+\.)([^/]+\.[^/]+)(.*)$";

            var replacement = "$2";

            var url = uri.ToString();

            var subdomain = Regex.Replace(url, pattern, replacement, RegexOptions.IgnoreCase);

            return subdomain.TrimEnd('.');
        }

        protected Uri RemoveSubdomain(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            // Pattern matches: protocol://subdomain.domain.tld/path
            // and replaces with: protocol://domain.tld/path

            var pattern = @"^(https?://)([^.]+\.)([^/]+\.[^/]+)(.*)$";

            var replacement = "$1$3$4";

            var url = uri.ToString();

            url = Regex.Replace(url, pattern, replacement, RegexOptions.IgnoreCase);

            return new Uri(url);
        }
    }
}
