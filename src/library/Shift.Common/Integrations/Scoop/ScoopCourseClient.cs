using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shift.Common.Integration.Scoop
{
    public interface IScoopCourseClient
    {
        Task<List<ScoopCourse>> GetCoursesAsync(string tenantHandle, string baseHandle, CancellationToken cancellationToken);
        List<ScoopCourse> GetCourses(string tenantHandle, string baseHandle);
    }

    /// <summary>
    /// Asks Scoop which SCORM packages exist under a base handle.
    /// </summary>
    /// <remarks>
    /// Shift iQ cannot otherwise see what Scoop holds: it signs a relay
    /// token for /{account}/{handle} and redirects, and when the handle does
    /// not resolve the learner is quietly put in the library. This client is
    /// how a launch URL is built from what exists rather than from a guess.
    ///
    /// Shaped after PartitionClient: an async core with a synchronous wrapper
    /// for the WebForms callers, the credential set per request rather than on
    /// the shared client, and a timeout enforced by a cancellation token.
    /// </remarks>
    public class ScoopCourseClient : IScoopCourseClient
    {
        /// <summary>
        /// Used when Scoop:LookupTimeoutSeconds is absent or not positive. The
        /// call sits on a page render, so the budget is small on purpose: a
        /// slow Scoop must cost a learner a moment, never the page.
        /// </summary>
        public const int DefaultTimeoutSeconds = 3;

        /// <summary>
        /// What a partner key starts with. Callers check this before sending a
        /// configured value as a credential, so the placeholder prose in
        /// AppSettings.json is never presented to Scoop as one.
        /// </summary>
        public const string PartnerKeyPrefix = "osp_";

        /// <summary>
        /// Whether a configured value is a partner key rather than the
        /// placeholder prose that ships in AppSettings.json.
        /// </summary>
        /// <remarks>
        /// An emptiness check is not enough: the placeholder is a sentence, so
        /// it passes one, and every lookup on an environment that never
        /// received a key would send that sentence to Scoop as a bearer
        /// token.
        /// </remarks>
        public static bool IsConfiguredKey(string apiKey)
        {
            return !string.IsNullOrWhiteSpace(apiKey)
                && apiKey.Trim().StartsWith(PartnerKeyPrefix, StringComparison.Ordinal);
        }

        private readonly ScoopSettings _settings;

        public ScoopCourseClient(ScoopSettings settings)
        {
            _settings = settings;
        }

        public List<ScoopCourse> GetCourses(string tenantHandle, string baseHandle)
        {
            return TaskRunner.RunSync(() => GetCoursesAsync(tenantHandle, baseHandle, CancellationToken.None));
        }

        public async Task<List<ScoopCourse>> GetCoursesAsync(string tenantHandle, string baseHandle, CancellationToken cancellationToken)
        {
            var url = BuildCoursesUrl(_settings?.BaseUrl, tenantHandle, baseHandle);

            var apiKey = _settings?.ApiKey;
            if (!IsConfiguredKey(apiKey))
                throw new InvalidOperationException(
                    $"Cannot query Scoop courses: Engine.Api.Scoop.ApiKey is not a partner key (it must start with {PartnerKeyPrefix}).");

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                // The shared HttpClient is a process-wide singleton, so the key
                // is set per request rather than on DefaultRequestHeaders,
                // where it would leak onto every unrelated caller.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                using (var timeoutCts = new CancellationTokenSource())
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(GetTimeoutSeconds()));

                    var result = await StaticHttpClient.ClientNoTimeout.SendAsync(request, linkedCts.Token);

                    // An organization Scoop has never heard of is not an
                    // incident. It is what a tenant with no SCORM content
                    // looks like, and it means the same as an empty list.
                    if (result.StatusCode == HttpStatusCode.NotFound)
                        return new List<ScoopCourse>();

                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        var body = await ReadBodyAsync(result);

                        // 401 and 403 land here deliberately. A partner key
                        // that has been revoked or was never deployed must be
                        // loud, not quietly indistinguishable from a package
                        // that does not exist.
                        throw new InvalidOperationException(
                            $"Scoop course lookup failed: HTTP {(int)result.StatusCode} {result.StatusCode} ({result.ReasonPhrase}). {body}");
                    }

                    var json = await result.Content.ReadAsStringAsync();

                    var set = JsonConvert.DeserializeObject<ScoopCourseSet>(json);

                    return set?.Courses?.ToList() ?? new List<ScoopCourse>();
                }
            }
        }

        /// <summary>
        /// The lookup URL for one organization and base handle. Public so the
        /// URL shape can be tested without standing up HTTP.
        /// </summary>
        public static string BuildCoursesUrl(string baseUrl, string tenantHandle, string baseHandle)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Cannot query Scoop courses: Engine.Api.Scoop.BaseUrl is not configured.");

            if (string.IsNullOrWhiteSpace(tenantHandle))
                throw new ArgumentException("An organization handle is required.", nameof(tenantHandle));

            if (string.IsNullOrWhiteSpace(baseHandle))
                throw new ArgumentException("A base package handle is required.", nameof(baseHandle));

            // Scoop:BaseUrl is the application root, the same value
            // ScoopLinkGenerator builds its lobby links from, so this appends
            // only its own segments.
            var root = baseUrl.TrimEnd('/');

            var tenant = Uri.EscapeDataString(tenantHandle.Trim().ToLowerInvariant());
            var handle = Uri.EscapeDataString(baseHandle.Trim().ToLowerInvariant());

            return $"{root}/api/v1/tenants/{tenant}/courses?handle={handle}";
        }

        private int GetTimeoutSeconds()
        {
            var configured = _settings?.LookupTimeoutSeconds ?? 0;
            return configured > 0 ? configured : DefaultTimeoutSeconds;
        }

        private static async Task<string> ReadBodyAsync(HttpResponseMessage response)
        {
            if (response.Content == null)
                return string.Empty;

            try
            {
                var body = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(body) ? string.Empty : body.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
