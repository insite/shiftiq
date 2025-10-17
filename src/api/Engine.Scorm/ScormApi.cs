using Com.RusticiSoftware.Cloud.V2.Api;
using Com.RusticiSoftware.Cloud.V2.Client;
using Com.RusticiSoftware.Cloud.V2.Model;

using Shift.Common.Scorm;

namespace Engine.Scorm
{
    public class ScormApi
    {
        public string UserName { get; } = null!;
        public string Password { get; } = null!;

        private readonly Configuration _config;
        private readonly CourseApi _courseApi;
        private readonly RegistrationApi _registrationApi;

        public ScormApi(string user, string password)
        {
            _config = new Configuration
            {
                Username = user,
                Password = password
            };

            _registrationApi = new RegistrationApi(_config);

            _courseApi = new CourseApi(_config);
        }

        public void CreateRegistration(Guid registrationId, string courseSlug, Guid learnerId, string learnerEmail, string learnerFirstName, string learnerLastName)
        {
            var learner = new LearnerSchema(learnerId.ToString(), learnerEmail, learnerFirstName, learnerLastName);

            var registration = new CreateRegistrationSchema(
                courseSlug,
                learner,
                registrationId.ToString()
                );

            _registrationApi.CreateRegistration(registration, null);
        }

        public Course? GetCourse(string courseSlug)
        {
            try
            {
                var course = _courseApi.GetCourse(courseSlug, includeRegistrationCount: true);

                var item = new Course
                {
                    Id = course.Id,
                    Title = course.Title,
                    Created = course.Created,
                    Updated = course.Updated,
                    CourseLearningStandard = course.CourseLearningStandard.ToString(),
                    Version = course.Version,
                    RegistrationCount = course.RegistrationCount
                };

                return item;
            }
            catch (Com.RusticiSoftware.Cloud.V2.Client.ApiException ex)
            {
                if (ex.ErrorCode == 404)
                    return null;

                throw;
            }
        }

        public Course[] GetCourses()
        {
            var result = _courseApi.GetCourses(includeRegistrationCount: true);

            var list = new List<Course>();

            foreach (var course in result.Courses)
            {
                list.Add(new Course
                {
                    Id = course.Id,
                    Title = course.Title,
                    Created = course.Created,
                    Updated = course.Updated,
                    CourseLearningStandard = course.CourseLearningStandard.ToString(),
                    Version = course.Version,
                    RegistrationCount = course.RegistrationCount
                });
            }

            return list.ToArray();
        }

        public int? GetRegistrationInstance(Guid registration)
        {
            try
            {
                var list = _registrationApi.GetRegistrationInstances(registration.ToString());
                if (list.Registrations.Count > 0)
                    return list.Registrations.OrderBy(x => x.Instance).Last().Instance;
            }
            catch (Com.RusticiSoftware.Cloud.V2.Client.ApiException ex)
            {
                if (IsRegistrationNotFound(ex))
                    return null;
                else
                    throw;
            }
            return null;

            bool IsRegistrationNotFound(Com.RusticiSoftware.Cloud.V2.Client.ApiException x)
                => x.Message.IndexOf("could not find registration", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public string GetRegistrationLaunchUrl(Guid registrationId, string courseSlug, bool preview, string callbackUrl, string exitUrl)
        {
            if (preview)
            {
                var input = new PreviewLaunchLinkRequestSchema(null, exitUrl);
                var launch = _courseApi.BuildCoursePreviewLaunchLink(courseSlug, input);
                return launch.LaunchLink;
            }
            else
            {
                var input = new LaunchLinkRequestSchema(null, exitUrl);
                var link = _registrationApi.BuildRegistrationLaunchLink(registrationId.ToString(), input);

                var settings = new List<SettingsIndividualSchema>
                {
                    new SettingsIndividualSchema("ApiRollupRegistrationPostBackUrl", callbackUrl)
                };

                _registrationApi.SetRegistrationConfiguration(registrationId.ToString(), new SettingsPostSchema { Settings = settings });

                return link.LaunchLink;
            }
        }

        public RegistrationProgress GetRegistrationInstanceProgress(Guid registration, int? instance)
        {
            var scormRegistration = _registrationApi.GetRegistrationInstanceProgress(registration.ToString(), instance);

            var progress = new RegistrationProgress();

            progress.CompletedDate = scormRegistration.CompletedDate;
            progress.FirstAccessDate = scormRegistration.FirstAccessDate;
            progress.LastAccessDate = scormRegistration.LastAccessDate;

            progress.RegistrationCompletion = $"{scormRegistration.RegistrationCompletion}";
            progress.RegistrationSuccess = $"{scormRegistration.RegistrationSuccess}";
            progress.ScoreScaled = (decimal?)scormRegistration.Score?.Scaled / 100.0M;
            progress.TotalSecondsTracked = (int?)scormRegistration?.TotalSecondsTracked;

            return progress;
        }

        public string GetRegistrationId(string courseSlug, Guid learnerId)
        {
            var registrations = _registrationApi.GetRegistrations(courseSlug, learnerId.ToString());

            if (registrations.Registrations.Count > 0)
                return registrations.Registrations.OrderBy(x => x.CreatedDate).First().Id;

            return null!;
        }

        public (Registration[] Registrations, string More) GetRegistrations(string? course = null, string? more = null)
        {
            var result = _registrationApi.GetRegistrations(courseId: course, more: more);

            var list = new List<Registration>();

            foreach (var registration in result.Registrations)
            {
                if (registration == null)
                    continue;

                list.Add(new Registration
                {
                    Id = registration.Id,
                    Instance = registration.Instance,
                    Updated = registration.Updated,
                    RegistrationCompletion = (registration.RegistrationCompletion ?? RegistrationCompletion.UNKNOWN).ToString(),
                    RegistrationSuccess = (registration.RegistrationSuccess ?? RegistrationSuccess.UNKNOWN).ToString(),
                    TotalSecondsTracked = registration.TotalSecondsTracked,
                    FirstAccessDate = registration.FirstAccessDate,
                    LastAccessDate = registration.LastAccessDate,
                    CompletedDate = registration.CompletedDate,
                    CreatedDate = registration.CreatedDate,
                    CourseId = registration.Course.Id,
                    CourseTitle = registration.Course.Title,
                    CourseVersion = registration.Course.Version,
                    LearnerId = registration.Learner.Id,
                    LearnerEmail = registration.Learner.Email,
                    LearnerFirstName = registration.Learner.FirstName,
                    LearnerLastName = registration.Learner.LastName
                });
            }

            return (list.ToArray(), result.More);
        }

        public string CreateImport(string courseSlug, bool mayCreateNewVersion, string postbackUrl, string uploadedContentType, string contentMetadata, Stream stream)
        {
            return _courseApi.CreateUploadAndImportCourseJob(courseSlug, mayCreateNewVersion, postbackUrl, uploadedContentType, contentMetadata, stream).Result;
        }

        public CourseImport GetImportStatus(string importSlug)
        {
            var status = new CourseImport();

            var data = _courseApi.GetImportJobStatus(importSlug);

            status.CourseId = data.ImportResult.Course.Id;
            status.Message = data.Message;

            status.IsComplete = data.Status == ImportJobResultSchema.StatusEnum.COMPLETE;
            status.IsError = data.Status == ImportJobResultSchema.StatusEnum.ERROR;
            status.IsRunning = data.Status == ImportJobResultSchema.StatusEnum.RUNNING;

            return status;
        }
    }
}