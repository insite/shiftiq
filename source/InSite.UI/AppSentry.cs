using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Humanizer;

using InSite.Common.Web;

using Sentry;
using Sentry.AspNet;

using Shift.Common;
using Shift.Constant;

namespace InSite
{
    public static class AppSentry
    {
        public static MonitoringSettings Settings
            => ServiceLocator.AppSettings.Shift.Api.Telemetry.Monitoring;

        public static readonly string AssemblyVersion
            = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static bool IsDatabaseCommandMonitorEnabled
            => _databaseMonitorInterceptor != null;

        public static string SentryVersion
        {
            get
            {
                var version = AssemblyVersion;

                return version.Substring(0, version.LastIndexOf('.'));
            }
        }

        private static readonly object _databaseCommandMonitorSyncObj = new object();
        private static SentryCommandInterceptor _databaseMonitorInterceptor;

        public static IDisposable Initialize()
        {
            if (!Settings.Enabled)
                return null;

            var environment = ServiceLocator.AppSettings.Environment.Name.ToString();
            var logFolder = ServiceLocator.FilePaths.GetPhysicalPathToEnterpriseFolder("Logs");
            var logFile = System.IO.Path.Combine(logFolder, "InSite.UI.Sentry.log");
            System.IO.Directory.CreateDirectory(logFolder);

            var sdk = SentrySdk.Init(o =>
            {
                o.Debug = Settings.Debug;
                o.DiagnosticLogger = Settings.Debug ? new Sentry.Infrastructure.FileDiagnosticLogger(logFile) : null;
                o.Dsn = Settings.Dsn;
                o.Environment = environment.ToLower();
                o.Release = SentryVersion;
                o.SendDefaultPii = true;
                o.TracesSampleRate = Settings.Rate;
                o.AddAspNet(Sentry.Extensibility.RequestSize.Always);
                o.AddEntityFramework();
            });

            UpdateDatabaseCommandMonitor();

            return sdk;
        }

        public static void UpdateDatabaseCommandMonitor()
        {
            if (!Settings.Enabled)
                return;

            var size = ServiceLocator.Partition.DatabaseMonitorLargeCommandSize;
            var trace = ServiceLocator.Partition.DatabaseMonitorIncludeStackTrace;

            var enabled = size > 0;

            lock (_databaseCommandMonitorSyncObj)
            {
                var interceptor = _databaseMonitorInterceptor;
                var inited = interceptor != null;
                var updated = inited && (
                    interceptor.Threshold != size
                    || interceptor.IncludeStackTrace != trace);

                if (inited && (!enabled || updated))
                {
                    DbInterception.Remove(_databaseMonitorInterceptor);
                    _databaseMonitorInterceptor = null;
                }

                if (enabled && (!inited || updated))
                {
                    _databaseMonitorInterceptor = new SentryCommandInterceptor(size, trace);
                    DbInterception.Add(_databaseMonitorInterceptor);
                }
            }
        }

        /// <summary>
        /// Returns true for invalid WebResource.axd requests. In both these cases ASP.NET returns an HTTP 404 response,
        /// by default, which is the behaviour we want.
        /// </summary>
        public static bool IsInvalidWebResourceRequestException(Exception ex)
        {
            if (ex is HttpException)
            {
                if (ex.Message == "This is an invalid webresource request.")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true for File Not Found errors.
        /// </summary>
        public static bool IsUrlRoutingException(Exception ex, out string file)
        {
            file = null;

            if (ex is HttpException)
            {
                var match = Regex.Match(ex.Message, "The file '(.+)' does not exist.");
                if (match.Success)
                {
                    file = match.Groups[1].Value;
                    return true;
                }
                match = Regex.Match(ex.Message, "The controller for path '(.+)' was not found or does not implement IController");
                if (match.Success)
                {
                    file = match.Groups[1].Value;
                    return true;
                }
            }

            return false;
        }

        public static void CaptureException(Exception error)
        {
            if (!Settings.Enabled)
                return;

            if (IsIntercepted(error))
                return;

            var isViewStateException = IsViewStateException(error);
            if (!isViewStateException)
                SentrySdk.CaptureException(error, SentryScope);
        }

        public static bool IgnoreTransaction(HttpRequest request)
        {
            if (!Settings.Enabled)
                return true;

            return request.Url.AbsolutePath.StartsWith("/api/commands/")
                || request.Url.AbsolutePath.EndsWith(".axd")
                || request.Url.AbsolutePath.EndsWith(".ico");
        }

        public static bool IsIntercepted(Exception error)
        {
            if (error is InSite.Persistence.Integration.DirectAccess.DirectAccessException)
            {
                ServiceLocator.AlertMailer.Send(OrganizationIdentifiers.SkilledTradesBC, new Domain.Messages.AlertUnhandledExceptionIntercepted { ExceptionMessage = error.Message });
                return true;
            }
            return false;
        }

        public static bool IsViewStateException(Exception error)
        {
            if (error is System.Web.UI.ViewStateException)
                return true;

            if (error.InnerException is System.Web.UI.ViewStateException)
                return true;

            var rootCause = error.GetBaseException();

            if (rootCause != null)
            {
                if (rootCause is HttpException && rootCause.Message.Contains("Failed to load viewstate"))
                    return true;

                if (rootCause is System.Web.UI.ViewStateException)
                    return true;
            }

            return false;
        }

        public static void SentryError(Exception ex)
        {
            if (!Settings.Enabled)
                return;

            // Response.Redirect(url, true) throws the ThreadAbortException to abort the thread. Therefore this exception can be ignored. For more
            // information, see https://stackoverflow.com/questions/2777105/why-response-redirect-causes-system-threading-threadabortexception
            if (ex is System.Threading.ThreadAbortException)
                return;

            if (ex is OperationCanceledException)
                return;

            SentrySdk.CaptureException(ex);
        }

        public static void SentryError(string error)
        {
            if (!Settings.Enabled)
                return;
            SentrySdk.CaptureMessage(error, SentryLevel.Error);
        }

        public static void SentryError(string error, string source)
        {
            if (!Settings.Enabled)
                return;
            SentrySdk.CaptureMessage(error + $"  (Source: {source})", SentryLevel.Error);
        }

        public static void SentryInfo(string info)
        {
            if (!Settings.Enabled)
                return;
            SentrySdk.CaptureMessage(info, SentryLevel.Info);
        }

        public static void SentryScope(Scope scope)
        {
            try
            {
                var raw = HttpContext.Current.Request.RawUrl;
                if (raw != null)
                    scope.SetTag("url-raw", raw);
            }
            catch (HttpException)
            {
                // If the attempt to access HttpContext.Current.Request throws an HttpException then ignore it. This
                // simply means we cannot include the raw URL in the scope for this error.
            }
        }

        public static void SentryWarning(string warning)
        {
            if (!Settings.Enabled)
                return;
            SentrySdk.CaptureMessage(warning, SentryLevel.Warning);
        }

        public static void SentryWarning(string warning, string source)
        {
            SentryWarning(warning + $"  (Source: {source})");
        }

        public static void SentryWarning(Exception ex)
        {
            var curEx = ex;
            var message = new StringBuilder();

            while (curEx != null)
            {
                message.Append(curEx.GetType().FullName).Append(": ").Append(curEx.Message);
                message.AppendLine();
                message.Append(curEx.StackTrace);
                message.AppendLine().AppendLine();

                curEx = curEx.InnerException;
            }

            if (message.Length > 0)
                SentryWarning(message.ToString());
        }

        internal static void StartTransaction(HttpContext context)
        {
            if (!IgnoreTransaction(context.Request))
            {
                var tx = context.StartSentryTransaction();
                tx.Name = $"{context.Request.HttpMethod} {HttpRequestHelper.GetRawUrl(context.Request)}";
                var x = new
                {
                    Name = tx.Name
                };
            }
        }

        internal static void FinishTransaction(HttpContext context)
        {
            if (!IgnoreTransaction(context.Request))
                context.FinishSentryTransaction();
        }
    }

    public class SentryCommandInterceptor : IDbCommandInterceptor
    {
        public int Threshold => _threshold;
        public bool IncludeStackTrace => _includeStackTrace;

        private readonly int _threshold;
        private readonly bool _includeStackTrace;

        public SentryCommandInterceptor(int threshold, bool includeStackTrace)
        {
            _threshold = threshold;
            _includeStackTrace = includeStackTrace;
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {

        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
            => Log(command);

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {

        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
            => Log(command);

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {

        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
            => Log(command);

        private void Log(DbCommand command)
        {
            var commandText = command.CommandText;
            int commandSize = Encoding.UTF8.GetByteCount(commandText);

            if (commandSize <= _threshold)
                return;

            string url = null, user = null;

            var context = HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                    url = $"{request.Url.Scheme}://{request.Url.Host}{request.RawUrl}";

                user = context.User?.Identity?.Name;
            }

            var info = new StringBuilder();
            info.AppendFormat("Large ({0}) SQL query executed", commandSize.Bytes().Humanize("KB"))
                .AppendLine();
            info.AppendFormat("URL: {0}", url.IfNullOrEmpty("N/A"))
                .AppendLine();
            info.AppendFormat("User: {0}", user.IfNullOrEmpty("N/A"))
                .AppendLine();

            if (_includeStackTrace)
            {
                var stackTrace = GetStackTrace(true);

                info.AppendLine()
                    .Append("Stack trace:")
                    .AppendLine()
                    .Append(stackTrace)
                    .AppendLine();
            }

            info.AppendLine()
                .Append("Query:")
                .AppendLine()
                .Append(commandText);

            AppSentry.SentryInfo(info.ToString());
        }

        private string GetStackTrace(bool truncate)
        {
            var stackTrace = System.Environment.StackTrace;

            if (truncate)
            {
                var cutIndex = stackTrace.IndexOf("System.Data.Entity.Internal.LazyEnumerator`1.MoveNext()");
                if (cutIndex == -1)
                    cutIndex = stackTrace.IndexOf("InSite.SentryCommandInterceptor.ReaderExecuted");

                if (cutIndex != -1)
                {
                    cutIndex = stackTrace.IndexOf("\r\n", cutIndex);
                    if (cutIndex != -1)
                        stackTrace = "   ...\r\n" + stackTrace.Substring(cutIndex + 2);
                }

                cutIndex = stackTrace.IndexOf("\r\n   at System.Web.HttpApplication.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()\r\n");
                if (cutIndex != -1)
                    stackTrace = stackTrace.Substring(0, cutIndex) + "\r\n   ...";
            }

            return stackTrace;
        }
    }
}