using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Humanizer;

using InSite.Application.Courses.Read;
using InSite.Application.Progresses.Write;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Scorm;

namespace InSite.Web.Integration
{
    public class ScormIntegrator
    {
        private ScormClient BuildScormClient()
        {
            var options = Organization.Integrations.ScormCloud;

            if (options == null || options.UserName.IsEmpty() || options.Password.IsEmpty())
            {
                var url = ServiceLocator.AppSettings.Engine.Api.Scorm.BaseUrl;

                var org = $"{Organization.CompanyName} ({Organization.OrganizationCode})";

                var user = User.Email;

                var activity = (Activity != null ? Activity.ActivityIdentifier.ToString() : "null");

                var context = $"User = {user} ; Activity = {activity}";

                var error = $"SCORM Cloud integration settings are missing from this organization account: {org}. API calls to {url} will fail with this error: Authorization Required. Additional context: {context}";

                throw new Exception(error);
            }

            var serializer = new JsonSerializer2();

            var client = new ScormClient(new ScormClientFactory(ServiceLocator.AppSettings.Engine.Api.Scorm.BaseUrl, options.UserName, options.Password), serializer);

            return client;
        }

        private class ScormRegistrationState
        {
            public Guid RegistrationId { get; set; }
            public TScormRegistration Local { get; set; }
            public string RemoteId { get; set; }
        }

        public string Language { get; set; }

        public QActivity Activity { get; set; }
        public Domain.Foundations.User User { get; set; }
        public OrganizationState Organization { get; set; }

        public string ActivityHook
        {
            get
            {
                var hook = Activity.ActivityHook;
                if (Activity.ActivityIsMultilingual)
                    hook += $"-{Language}";
                return hook;
            }
        }

        public string ActivityMode
        {
            get
            {
                return Activity.ActivityMode;
            }
        }

        public ScormIntegrator(OrganizationState organization, Domain.Foundations.User user, Guid activityId)
        {
            Organization = organization;

            Activity = CourseSearch.SelectActivity(activityId, x => x.Module.Unit.Course);

            if (Activity != null && TGroupPermissionSearch.IsAccessDenied(Activity.ActivityIdentifier, CurrentSessionState.Identity))
                Activity = null;

            Language = CookieTokenModule.Current.Language;

            User = user;
        }

        public string StartCourseImport(string courseId, bool mayCreateNewVersion, string postbackUrl, string uploadedContentType, string contentMetadata, Stream stream)
        {
            var client = BuildScormClient();

            var result = client.CreateImport(courseId, mayCreateNewVersion, postbackUrl, uploadedContentType, contentMetadata, stream);

            return result;
        }

        public IEnumerable<string> GetCourseHooks(Guid id)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(id);
            var hooks = gradebook.AllItems.Where(x => x.Hook != null).Select(x => x.Hook).Distinct().OrderBy(x => x).ToList();
            return hooks.ToArray();
        }

        public Course RetrieveCourse(string slug)
        {
            var client = BuildScormClient();

            return client.RetrieveCourse(slug);

        }
        public Course[] GetCourses()
        {
            var client = BuildScormClient();

            return client.GetCourses();
        }

        public CourseImport GetCourseImportStatus(string jobId)
        {
            var client = BuildScormClient();

            return client.GetImportStatus(jobId);
        }

        public IEnumerable<TScormRegistration> GetRegistrations(Guid gradebook, Guid? user)
        {
            var hooks = GetCourseHooks(gradebook);

            var activities = GetConditions(gradebook);

            return ScormRegistrationSearch.Select(hooks, activities, user).ToArray();
        }

        public IEnumerable<xApiCondition> GetConditions(Guid id)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(id);
            var gradeitems = gradebook.AllItems.Select(x => x.Identifier).ToList();
            var conditions = CourseSearch.BindActivities(
                x => new xApiCondition
                {
                    ActivityIdentifier = x.ActivityIdentifier,
                    GradebookIdentifier = x.GradeItem.GradebookIdentifier,
                    GradeItemIdentifier = x.GradeItemIdentifier.Value,
                    StatementWhenVerb = x.GradeItem.StatementWhenVerb,
                    StatementWhenObject = x.GradeItem.StatementWhenObject,
                    StatementThenCommand = x.GradeItem.StatementThenCommand
                },
                x => x.ActivityUrlType == "SCORM" && gradeitems.Any(y => y == x.GradeItemIdentifier));
            return conditions;
        }

        public string GetUrl(HttpRequest request, string callbackUrl)
        {
            var client = BuildScormClient();

            var registration = GetRegistrationIdentifier(ActivityHook, User.Identifier);

            if (registration.Local == null)
                CreateProgression(registration.RegistrationId);

            if (string.IsNullOrEmpty(registration.RemoteId))
                client.CreateRegistration(new RegistrationRequest(registration.RegistrationId, ActivityHook, User.Identifier, User.Email, User.FirstName, User.LastName));

            // If we are in Production then the default launch is Live.

            var environment = ServiceLocator.AppSettings.Environment.Name;

            var isSandbox = environment == EnvironmentName.Sandbox;
            var isDevelopment = environment == EnvironmentName.Development;
            var isLocal = environment == EnvironmentName.Local;

            var defaultMode = isLocal || isDevelopment || isSandbox ? "Preview" : "Normal";
            var launchMode = ActivityMode ?? defaultMode;
            var isPreview = launchMode == "Preview";

            var exitUrl = $"{request.Url.Scheme}://{request.Url.Host}/ui/lobby/scorm/{Activity.ActivityIdentifier}/{registration.RegistrationId}/exit";

            if (!Activity.ActivityIsDispatch)
            {
                var referrer = request.UrlReferrer;
                exitUrl = (referrer == null)
                    ? $"{request.Url.Scheme}://{request.Url.Host}{request.RawUrl}"
                    : referrer.OriginalString;

                var parameter = "sync=scorm-cloud";
                if (!exitUrl.Contains(parameter))
                    exitUrl = exitUrl.Contains("?")
                        ? exitUrl + "&" + parameter
                        : exitUrl + "?" + parameter;
            }

            callbackUrl = callbackUrl.Replace("{activity}", Activity.ActivityIdentifier.ToString());

            return client.GetRegistrationLaunchUrl(registration.RegistrationId, ActivityHook, isPreview, callbackUrl, exitUrl);
        }

        public bool Synchronize(IEnumerable<TScormRegistration> registrations)
        {
            if (registrations == null || registrations.Count() == 0)
                return false;

            Guid id = Guid.Empty;

            bool isNewCompletion = false;

            try
            {
                var client = BuildScormClient();

                foreach (var registration in registrations)
                {
                    if (registration == null)
                        continue;

                    id = registration.ScormRegistrationIdentifier;

                    var scormRegistration = client.GetRegistrationInstance(id);
                    if (scormRegistration == null)
                        continue;

                    var scormProgress = client.GetRegistrationInstanceProgress(id, scormRegistration);
                    if (scormProgress == null)
                        continue;

                    CopyScormProgressToShiftRegistration(scormProgress, registration, scormRegistration);
                    CopyLearnerNameAndEmailToShiftRegistration(registration);
                    SaveChanges(registration);
                    isNewCompletion = isNewCompletion || CheckForScormCourseCompletion(registration);
                }
            }
            catch (Exception ex)
            {
                var error = $"An unexpected error occurred during synchronization of Shift iQ and SCORM Cloud for {"registration".ToQuantity(registrations.Count())}. The error occurred on SCORM Registration {id}. {ex.GetAllMessages()}";
                AppSentry.SentryInfo(error);
            }

            return isNewCompletion;
        }

        private bool CheckForScormCourseCompletion(TScormRegistration registration)
        {
            if (!registration.ScormCompleted.HasValue || registration.Activities == null)
                return false;

            bool isNewCompletion = false;

            foreach (var activity in registration.Activities)
                isNewCompletion = isNewCompletion || ScormCourseCompleted(activity.ActivityIdentifier, registration.LearnerIdentifier, registration.ScormCompleted.Value);

            return isNewCompletion;
        }

        private void SaveChanges(TScormRegistration registration)
        {
            ScormRegistrationStore.Update(registration);
        }

        private void CopyLearnerNameAndEmailToShiftRegistration(TScormRegistration registration)
        {
            var user = UserSearch.Bind(registration.LearnerIdentifier, x => new { x.FullName, x.Email });
            registration.LearnerName = user?.FullName;
            registration.LearnerEmail = user?.Email;
        }

        private void CopyScormProgressToShiftRegistration(RegistrationProgress scormProgress, TScormRegistration shiftRegistration, int? instance)
        {
            shiftRegistration.ScormCompleted = scormProgress.CompletedDate;
            shiftRegistration.ScormAccessedFirst = scormProgress.FirstAccessDate;
            shiftRegistration.ScormAccessedLast = scormProgress.LastAccessDate;

            shiftRegistration.ScormRegistrationCompletion = $"{scormProgress.RegistrationCompletion}";
            shiftRegistration.ScormRegistrationSuccess = $"{scormProgress.RegistrationSuccess}";
            shiftRegistration.ScormRegistrationScore = scormProgress.ScoreScaled / 100.0M;
            shiftRegistration.ScormRegistrationTrackedSeconds = scormProgress?.TotalSecondsTracked;
            shiftRegistration.ScormRegistrationInstance = instance;

            shiftRegistration.ScormSynchronized = DateTimeOffset.Now;
        }

        private ScormRegistrationState GetRegistrationIdentifier(string hook, Guid user)
        {
            var client = BuildScormClient();

            var state = new ScormRegistrationState
            {
                Local = GetProgression(hook, user),
                RemoteId = client.GetRegistrationId(hook, user)
            };

            if (state.Local == null && string.IsNullOrEmpty(state.RemoteId))
                state.RegistrationId = UniqueIdentifier.Create();
            else if (!string.IsNullOrEmpty(state.RemoteId))
                state.RegistrationId = Guid.Parse(state.RemoteId);
            else
                state.RegistrationId = state.Local.ScormRegistrationIdentifier;

            return state;
        }

        private bool ScormCourseCompleted(Guid activityId, Guid userId, DateTimeOffset completed)
        {
            var activity = CourseSearch.SelectActivity(activityId);
            var gradeItem = activity?.GradeItemIdentifier;
            if (gradeItem == null)
                return false;

            var gradeitem = ServiceLocator.RecordSearch.GetGradeItem(activity.GradeItemIdentifier.Value);
            if (gradeitem == null)
                return false;

            return Complete(gradeitem.GradebookIdentifier, gradeitem.GradeItemIdentifier, userId, completed, true);
        }

        private bool Complete(Guid gradebook, Guid gradeitem, Guid user, DateTimeOffset? when, bool requireEnrollment)
        {
            var progress = ServiceLocator.RecordSearch.GetProgress(gradebook, gradeitem, user);
            var isEnrolled = ServiceLocator.RecordSearch.EnrollmentExists(gradebook, user);

            if (requireEnrollment && !isEnrolled)
                return false;

            Guid id;

            if (progress != null)
            {
                id = progress.ProgressIdentifier;
            }
            else
            {
                var command = ServiceLocator.RecordSearch.CreateCommandToAddProgress(null, gradebook, gradeitem, user);

                ServiceLocator.SendCommand(command);

                id = command.AggregateIdentifier;
            }

            if (!progress.ProgressIsCompleted || progress.ProgressCompleted != when)
            {
                ServiceLocator.SendCommand(new CompleteProgress(id, when, null, null, null));
                return true;
            }

            return false;
        }

        private TScormRegistration GetProgression(string courseHook, Guid learner)
        {
            var progression = ScormRegistrationSearch.Select(courseHook, learner);

            if (progression != null)
            {
                progression.ScormLaunchCount++;
                progression.ScormLaunchedLast = DateTimeOffset.UtcNow;
                ScormRegistrationStore.Update(progression);
            }

            return progression;
        }

        private void CreateProgression(Guid id)
        {
            var registration = new TScormRegistration
            {
                ScormPackageHook = ActivityHook,
                ScormLaunchedFirst = DateTimeOffset.UtcNow,
                ScormLaunchCount = 1,
                ScormRegistrationIdentifier = id,
                OrganizationIdentifier = Organization.Identifier,
                LearnerIdentifier = User.UserIdentifier,

            };

            registration.ScormLaunchedLast = registration.ScormLaunchedFirst;

            ScormRegistrationStore.Insert(registration);

            var activity = new TScormRegistrationActivity
            {
                JoinIdentifier = UniqueIdentifier.Create(),
                ScormRegistrationIdentifier = id,
                ActivityIdentifier = Activity.ActivityIdentifier,
                GradeItemIdentifier = Activity.GradeItemIdentifier,
                OrganizationIdentifier = registration.OrganizationIdentifier
            };

            ScormRegistrationStore.Insert(activity);
        }
    }
}