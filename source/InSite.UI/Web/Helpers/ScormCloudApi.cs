using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Com.RusticiSoftware.Cloud.V2.Api;
using Com.RusticiSoftware.Cloud.V2.Client;
using Com.RusticiSoftware.Cloud.V2.Model;

namespace InSite.Web.Helpers
{
    public class ScormCloudApi : IScormCloudApi
    {
        private readonly Configuration _config;
        private readonly CourseApi _courseApi;
        private readonly RegistrationApi _registrationApi;

        public ScormCloudApi(string user, string password)
        {
            _config = new Configuration
            {
                Username = user,
                Password = password
            };

            _registrationApi = new RegistrationApi(_config);

            _courseApi = new CourseApi(_config);
        }

        public string BuildRegistrationLaunchLink(HttpRequest request, Guid activityId, bool preview, string course, Guid scormRegistration, bool scormDispatch)
        {
            var redirectOnExitUrl = $"{request.Url.Scheme}://{request.Url.Host}/ui/lobby/scorm/{activityId}/{scormRegistration}/exit";

            if (!scormDispatch)
            {
                var referrer = request.UrlReferrer;
                redirectOnExitUrl = (referrer == null)
                    ? $"{request.Url.Scheme}://{request.Url.Host}{request.RawUrl}"
                    : referrer.OriginalString;

                var parameter = "sync=scorm-cloud";
                if (!redirectOnExitUrl.Contains(parameter))
                    redirectOnExitUrl = redirectOnExitUrl.Contains("?")
                        ? redirectOnExitUrl + "&" + parameter
                        : redirectOnExitUrl + "?" + parameter;
            }

            if (preview)
            {
                var input = new PreviewLaunchLinkRequestSchema(null, redirectOnExitUrl);
                var launch = _courseApi.BuildCoursePreviewLaunchLink(course, input);
                return launch.LaunchLink;
            }
            else
            {
                var input = new LaunchLinkRequestSchema(null, redirectOnExitUrl);
                var link = _registrationApi.BuildRegistrationLaunchLink(scormRegistration.ToString(), input);

                var settings = new List<SettingsIndividualSchema>
                {
                    new SettingsIndividualSchema("ApiRollupRegistrationPostBackUrl", "https://restbucket.shiftiq.com/simple")
                };

                _registrationApi.SetRegistrationConfiguration(scormRegistration.ToString(), new SettingsPostSchema { Settings = settings });

                return link.LaunchLink;
            }
        }

        public void CreateRegistration(Guid registrationId, string activityHook, Guid learnerId, string learnerEmail, string learnerFirstName, string learnerLastName)
        {
            var learner = new LearnerSchema(learnerId.ToString(), learnerEmail, learnerFirstName, learnerLastName);

            var registration = new CreateRegistrationSchema(
                activityHook,
                learner,
                registrationId.ToString()
                );

            _registrationApi.CreateRegistration(registration, null);
        }

        public string CreateUploadAndImportCourseJob(string courseId, bool mayCreateNewVersion, string postbackUrl, string uploadedContentType, string contentMetadata, Stream stream)
        {
            return _courseApi.CreateUploadAndImportCourseJob(courseId, mayCreateNewVersion, postbackUrl, uploadedContentType, contentMetadata, stream).Result;
        }

        public ScormImport GetImportJobStatus(string jobId)
        {
            var status = new ScormImport();

            var data = _courseApi.GetImportJobStatus(jobId);

            status.CourseId = data.ImportResult.Course.Id;
            status.Message = data.Message;

            status.IsComplete = data.Status == ImportJobResultSchema.StatusEnum.COMPLETE;
            status.IsError = data.Status == ImportJobResultSchema.StatusEnum.ERROR;
            status.IsRunning = data.Status == ImportJobResultSchema.StatusEnum.RUNNING;

            return status;
        }

        public int? GetLastScormRegistrationInstance(Guid registration)
        {
            try
            {
                var list = _registrationApi.GetRegistrationInstances(registration.ToString());
                if (list.Registrations.Count > 0)
                    return list.Registrations.OrderBy(x => x.Instance).Last().Instance;
            }
            catch (ApiException ex)
            {
                if (IsRegistrationNotFound(ex))
                    return null;
                else
                    throw;
            }
            return null;

            bool IsRegistrationNotFound(ApiException x)
                => x.Message.IndexOf("could not find registration", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public string GetRegistrationId(string courseId, string learnerId)
        {
            var registrations = _registrationApi.GetRegistrations(courseId, learnerId);

            if (registrations.Registrations.Count > 0)
                return registrations.Registrations.OrderBy(x => x.CreatedDate).First().Id;

            return null;
        }

        public ScormProgress GetRegistrationInstanceProgress(Guid registration, int? instance)
        {
            var scormRegistration = _registrationApi.GetRegistrationInstanceProgress(registration.ToString(), instance);

            var progress = new ScormProgress();

            progress.CompletedDate = scormRegistration.CompletedDate;
            progress.FirstAccessDate = scormRegistration.FirstAccessDate;
            progress.LastAccessDate = scormRegistration.LastAccessDate;

            progress.RegistrationCompletion = $"{scormRegistration.RegistrationCompletion}";
            progress.RegistrationSuccess = $"{scormRegistration.RegistrationSuccess}";
            progress.ScoreScaled = (decimal?)scormRegistration.Score?.Scaled / 100.0M;
            progress.TotalSecondsTracked = (int?)scormRegistration?.TotalSecondsTracked;

            return progress;
        }
    }
}