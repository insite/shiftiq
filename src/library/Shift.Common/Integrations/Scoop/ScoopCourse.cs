using System;

using Newtonsoft.Json;

namespace Shift.Common.Integration.Scoop
{
    /// <summary>
    /// One course as the Scoop course lookup reports it.
    /// </summary>
    /// <remarks>
    /// Written against the live payload of GET /api/v1/tenants/{handle}/courses,
    /// not against a specification. CourseKey is a number there, not a string.
    /// </remarks>
    public class ScoopCourse
    {
        [JsonProperty("courseKey")]
        public long CourseKey { get; set; }

        /// <summary>The package handle, as stored, which may carry mixed case.</summary>
        [JsonProperty("handle")]
        public string Handle { get; set; }

        /// <summary>
        /// What follows the base handle and one hyphen, or null for the base
        /// itself. Scoop does not validate it as a language code, so treat
        /// it as a suffix that may or may not be one.
        /// </summary>
        [JsonProperty("variant")]
        public string Variant { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>"scorm" or "xapi".</summary>
        [JsonProperty("standard")]
        public string Standard { get; set; }

        [JsonProperty("shared")]
        public bool Shared { get; set; }

        /// <summary>
        /// The organization that owns the package, which differs from the one
        /// asked about when a shared course resolved across organizations.
        /// </summary>
        [JsonProperty("hostedBy")]
        public string HostedBy { get; set; }

        /// <summary>The launch target Scoop built, already lowercased.</summary>
        [JsonProperty("targetPath")]
        public string TargetPath { get; set; }

        [JsonProperty("uploadedAt")]
        public DateTimeOffset? UploadedAt { get; set; }
    }

    /// <summary>
    /// The envelope the lookup returns: the organization and base handle that
    /// were asked about, and the courses found under them.
    /// </summary>
    public class ScoopCourseSet
    {
        [JsonProperty("tenant")]
        public ScoopTenant Tenant { get; set; }

        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("courses")]
        public ScoopCourse[] Courses { get; set; }
    }

    public class ScoopTenant
    {
        [JsonProperty("handle")]
        public string Handle { get; set; }
    }
}
