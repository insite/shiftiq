using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Shift.Common.Scorm
{
    public class ScormClient
    {
        private readonly ApiClientSynchronous _client;

        public ScormClient(IHttpClientFactory factory, IJsonSerializerBase serializer)
        {
            _client = new ApiClientSynchronous(factory, serializer);
        }

        public void CreateRegistration(RegistrationRequest request)
        {
            var result = _client.HttpPost("registrations", request);
            if (result.Problem != null)
                throw new ApplicationError(result.Problem.Detail);
        }

        public Course RetrieveCourse(string courseSlug)
            => _client.HttpGet<Course>($"courses/{courseSlug}").Data;

        public Course[] GetCourses()
            => _client.HttpGet<Course[]>($"courses").Data;

        public int? GetRegistrationInstance(Guid registrationId)
        {
            var instance = _client.HttpGet<string>($"registrations/{registrationId}/last-instance").Data;

            if (int.TryParse(instance, out var result))
                return result;

            return null;
        }

        public string GetRegistrationLaunchUrl(Guid registrationId, string courseSlug, bool preview, string callbackUrl, string exitUrl)
            => _client.HttpGet<string>($"registrations/{registrationId}/launch-url?courseSlug={courseSlug}&preview={preview}&callbackUrl={callbackUrl}&exitUrl={exitUrl}").Data;

        public RegistrationProgress GetRegistrationInstanceProgress(Guid registrationId, int? instance)
        {
            var url = $"registrations/{registrationId}/progress";
            if (instance.HasValue)
                url += $"?instance={instance}";

            return _client.HttpGet<RegistrationProgress>(url).Data;
        }

        public string GetRegistrationId(string courseSlug, Guid learnerId)
            => _client.HttpGet<string>($"courses/{courseSlug}/learners/{learnerId}/registration").Data;

        public (Registration[] Registrations, string More) GetRegistrations(string course, string more = null)
        {
            var endpoint = "registrations";

            if (!string.IsNullOrEmpty(more))
                endpoint += "?more=" + more;

            else if (!string.IsNullOrEmpty(course))
                endpoint += "?course=" + course;

            var result = _client.HttpGet<Registration[]>(endpoint);

            if (result.Status == HttpStatusCode.NotFound)
                return (default, null);

            if (result.Headers.Contains("x-scorm-more"))
                more = result.Headers.GetValues("x-scorm-more").FirstOrDefault();

            return (result.Data, more);
        }

        public string CreateImport(string courseSlug, bool mayCreateNewVersion, string callbackUrl, string uploadedContentType, string contentMetadata, Stream stream)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(courseSlug), "courseSlug" },
                { new StringContent(mayCreateNewVersion.ToString()), "mayCreateNewVersion" },
                { new StringContent(callbackUrl), "callbackUrl" },
                { new StringContent(uploadedContentType), "uploadedContentType" },
                { new StringContent(contentMetadata), "contentMetadata" },
                { new StreamContent(stream), "file", "uploadFile" }
            };

            return _client.HttpPost<string>("imports", content).Data;
        }

        public CourseImport GetImportStatus(string importSlug)
            => _client.HttpGet<CourseImport>($"imports/{importSlug}").Data;
    }
}