using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Integration.Scoop;

namespace InSite.Web.Integration
{
    /// <summary>
    /// What Scoop holds under a base SCORM Package ID, cached.
    /// </summary>
    /// <remarks>
    /// This sits on a hot path. LoadLinkPanelScorm runs on every non-postback
    /// render of a SCORM activity page, and Next, Previous and Done each end in
    /// a redirect, so it is one fresh GET per activity page view. Everything
    /// here exists to keep that from becoming one HTTP call per page view.
    /// </remarks>
    public static class ScoopPackageResolver
    {
        /// <summary>
        /// How long a successful answer is trusted. A package uploaded after a
        /// lesson was already launched reaches learners within this window;
        /// an administrator sees it at once, because the Link Setup badge
        /// bypasses the cache.
        /// </summary>
        public const int FoundCacheSeconds = 300;

        /// <summary>
        /// How long a failure is remembered. Short, because the cost of being
        /// wrong is a learner in the wrong language, and long enough that an
        /// Scoop outage costs one timeout per package per half minute
        /// rather than one per page view.
        /// </summary>
        public const int FailureCacheSeconds = 30;

        private static readonly MemoryCache<string, ScoopPackageSet> Cache = new MemoryCache<string, ScoopPackageSet>();

        // A cohort starting the same module at the same time would otherwise
        // send one request each into a cold cache.
        private static readonly KeyedLock<string> Locks = new KeyedLock<string>(TimeSpan.FromSeconds(10));

        /// <summary>
        /// The Scoop organization handle for a Shift organization. This is
        /// the OWNER of the course, which for a partition-hosted global module
        /// is not the organization the current user is signed in to.
        /// </summary>
        public static string GetTenantHandle(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return null;

            var organization = OrganizationSearch.Select(organizationId);

            return organization?.Code;
        }

        /// <summary>
        /// The handle to launch for this learner, or the base handle when the
        /// lesson is not multilingual.
        /// </summary>
        public static string ResolveLaunchHandle(string tenantHandle, string baseHandle, bool isMultilingual, string language)
        {
            // Both guards keep a single-language lesson off the network and out
            // of the cache entirely, which is what makes this safe to deploy
            // before anybody turns the feature on.
            if (!isMultilingual || string.IsNullOrWhiteSpace(baseHandle))
                return baseHandle;

            var packages = GetPackages(tenantHandle, baseHandle);

            return ScoopPackageNaming.Resolve(baseHandle, language, true, packages.IsAvailable ? packages.Handles : null);
        }

        /// <summary>
        /// What Scoop reports under this base handle.
        /// </summary>
        /// <param name="tenantHandle"></param>
        /// <param name="baseHandle"></param>
        /// <param name="refresh">
        /// True to ignore a cached answer. The Link Setup badge passes this so
        /// an administrator who has just uploaded a package sees it, and warms
        /// the cache for the learners who follow.
        /// </param>
        public static ScoopPackageSet GetPackages(string tenantHandle, string baseHandle, bool refresh = false)
        {
            if (string.IsNullOrWhiteSpace(tenantHandle) || string.IsNullOrWhiteSpace(baseHandle))
                return ScoopPackageSet.Unavailable();

            // Scoop answers anything that is not a handle with a 400. Asking anyway reported one
            // Sentry event per badge render for a Package ID saved as an enrol link.
            var isHandle = ScoopPackageNaming.IsHandle(baseHandle.Trim());
            if (!isHandle)
                return ScoopPackageSet.Unavailable();

            var key = BuildCacheKey(tenantHandle, baseHandle);

            ScoopPackageSet cached;
            if (!refresh && Cache.TryGet(key, out cached))
                return cached;

            using (Locks.Lock(key))
            {
                // Another render may have filled it while this one waited.
                if (!refresh && Cache.TryGet(key, out cached))
                    return cached;

                var settings = ServiceLocator.AppSettings.Engine.Api.Scoop;

                // No key configured is the state of any box that never received
                // one, including every developer's. Skip the call and say so,
                // without reporting it: this is a deployment fact, not an
                // incident, and it degrades to the English package by design.
                //
                // "Configured" means a real partner key, not merely a non-empty
                // string. The placeholder that ships in AppSettings.json is
                // prose, and an emptiness check passes it straight through:
                // every lookup on an environment that never received a key
                // would then send that prose to Scoop as a bearer token,
                // once per package per cache window, and log the refusal.
                if (settings == null || !ScoopCourseClient.IsConfiguredKey(settings.ApiKey))
                {
                    var missing = ScoopPackageSet.Unavailable();
                    Cache.Add(key, missing, FailureCacheSeconds, true);
                    return missing;
                }

                try
                {
                    var client = new ScoopCourseClient(settings);

                    var courses = client.GetCourses(tenantHandle, baseHandle);

                    var found = ScoopPackageSet.From(courses);

                    Cache.Add(key, found, FoundCacheSeconds, true);

                    return found;
                }
                catch (Exception ex)
                {
                    // Reported here, where a fetch actually happened, and never
                    // when a cached failure is served: one broken package on a
                    // busy course would otherwise fill Sentry with one event
                    // per page view.
                    //
                    // CaptureException rather than SentryError, because
                    // SentryError drops OperationCanceledException and a
                    // timeout arrives as exactly that.
                    AppSentry.CaptureException(new InvalidOperationException(
                        $"Scoop course lookup failed for organization '{tenantHandle}' and package '{baseHandle}'.", ex));

                    var failed = ScoopPackageSet.Unavailable();

                    Cache.Add(key, failed, FailureCacheSeconds, true);

                    return failed;
                }
            }
        }

        /// <summary>
        /// Drops what is cached for one organization, so a newly uploaded
        /// package is visible without waiting out the window.
        /// </summary>
        public static void Clear(string tenantHandle)
        {
            if (string.IsNullOrWhiteSpace(tenantHandle))
                return;

            var prefix = tenantHandle.Trim().ToLowerInvariant() + "|";

            Cache.Remove(k => k != null && k.StartsWith(prefix, StringComparison.Ordinal));
        }

        // A string key rather than a tuple so the prefix removal above stays
        // possible. One cache entry, and so one expiry timer, per distinct
        // organization and base handle; that is why this is never keyed per
        // learner or per activity.
        private static string BuildCacheKey(string tenantHandle, string baseHandle)
        {
            return tenantHandle.Trim().ToLowerInvariant() + "|" + baseHandle.Trim().ToLowerInvariant();
        }
    }

    /// <summary>
    /// The answer to "what does Scoop hold under this handle", including
    /// the case where the question could not be asked.
    /// </summary>
    public class ScoopPackageSet
    {
        private ScoopPackageSet(bool isAvailable, IList<ScoopCourse> courses)
        {
            IsAvailable = isAvailable;
            Courses = courses ?? new List<ScoopCourse>();
            Handles = new HashSet<string>(
                Courses.Where(c => !string.IsNullOrWhiteSpace(c.Handle)).Select(c => c.Handle),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// False when Scoop could not be asked. Distinct from an empty
        /// Courses list, which means it answered and holds nothing: the first
        /// launches English, the second launches the base handle.
        /// </summary>
        public bool IsAvailable { get; private set; }

        public IList<ScoopCourse> Courses { get; private set; }

        public HashSet<string> Handles { get; private set; }

        public static ScoopPackageSet From(IList<ScoopCourse> courses)
        {
            return new ScoopPackageSet(true, courses);
        }

        public static ScoopPackageSet Unavailable()
        {
            return new ScoopPackageSet(false, null);
        }
    }
}
