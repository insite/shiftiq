using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Attempts.Write;
using InSite.Application.Courses.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Application.Responses.Write;

namespace InSite.Persistence
{
    public class ProgressRestarter : IProgressRestarter
    {
        private static readonly object StaticLock = new object();

        private readonly Action<ICommand> _send;
        private readonly IRecordSearch _records;

        public ProgressRestarter(Action<ICommand> send, IRecordSearch records)
        {
            _send = send;
            _records = records;
        }

        public void Restart(Guid learner, Guid course, DateTimeOffset started)
        {
            lock (StaticLock)
            {
                UpdateCourseEnrollmentStatus(learner, course, started);

                var activities = GetActivities(course);

                foreach (var activity in activities)
                {
                    DeleteAssessmentAttempts(learner, activity);
                    DeleteScormRegistration(learner, activity);
                    DeleteSurveyResponses(learner, activity);
                }

                var gradebooks = GetGradebooks(course);

                foreach (var gradebook in gradebooks)
                {
                    var gradeitems = GetGradeItems(gradebook.GradebookIdentifier);

                    foreach (var gradeitem in gradeitems)
                        DeleteGradebookProgress(learner, gradebook.GradebookIdentifier, gradebook.IsLocked, gradeitem);
                }
            }
        }

        private void DeleteAssessmentAttempts(Guid learner, QActivity activity)
        {
            if (activity.AssessmentFormIdentifier == null)
                return;

            List<Guid> attempts = GetAssessmentAttempts(learner, activity.AssessmentFormIdentifier.Value);

            foreach (var attempt in attempts)
                _send(new VoidAttempt(attempt, $"Learner restarted the course"));
        }

        private void DeleteGradebookProgress(Guid learner, Guid gradebook, bool isLocked, Guid gradeitem)
        {
            if (isLocked)
                _send(new UnlockGradebook(gradebook));

            var progress = GetProgressIdentifier(learner, gradebook, gradeitem);

            _send(new DeleteProgress(progress));
            _send(new RestartEnrollment(gradebook, learner, DateTimeOffset.UtcNow));
        }

        private void DeleteSurveyResponses(Guid learner, QActivity activity)
        {
            if (activity.SurveyFormIdentifier == null)
                return;

            var responses = GetSurveyResponses(learner, activity.SurveyFormIdentifier.Value);

            foreach (var response in responses)
                _send(new DeleteResponseSession(response));
        }

        private void DeleteScormRegistration(Guid learner, QActivity activity)
        {
            var isLink = activity.ActivityType == "Link";

            var isScormCloud = activity.ActivityUrlType == "SCORM" && activity.ActivityPlatform == "SCORM Cloud";

            var scormCloudCourseId = activity.ActivityHook;

            if (!isLink || !isScormCloud || string.IsNullOrEmpty(scormCloudCourseId))
                return;

            var registration = ScormRegistrationSearch.Select(scormCloudCourseId, learner);
            if (registration != null)
                ScormRegistrationStore.Delete(registration.ScormRegistrationIdentifier);
        }

        private List<QActivity> GetActivities(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                return db.QUnits
                    .Where(x => x.CourseIdentifier == course)
                    .SelectMany(u => u.Modules.SelectMany(m => m.Activities))
                    .ToList();
            }
        }

        private List<Guid> GetAssessmentAttempts(Guid learner, Guid form)
        {
            using (var db = new InternalDbContext())
            {
                return db.QAttempts
                    .Where(x => x.FormIdentifier == form && x.LearnerUserIdentifier == learner)
                    .Select(x => x.AttemptIdentifier)
                    .ToList();
            }
        }

        private List<QGradebook> GetGradebooks(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                return db.QCourses
                    .Where(x => x.GradebookIdentifier != null && x.CourseIdentifier == course)
                    .Select(x => x.Gradebook)
                    .ToList();
            }
        }

        private List<Guid> GetGradeItems(Guid gradebook)
        {
            using (var db = new InternalDbContext())
            {
                return db.QGradeItems
                    .Where(x => x.GradebookIdentifier == gradebook)
                    .Select(x => x.GradeItemIdentifier)
                    .ToList();
            }
        }

        public Guid GetProgressIdentifier(Guid learner, Guid gradebook, Guid gradeitem)
        {
            var progressId = _records.GetProgressIdentifier(gradebook, gradeitem, learner);
            if (progressId.HasValue)
                return progressId.Value;

            CourseObjectChangeProcessor.EnsureEnrollment(_send, _records, gradebook, learner, DateTimeOffset.UtcNow);

            var command = _records.CreateCommandToAddProgress(null, gradebook, gradeitem, learner);

            _send(command);

            return command.AggregateIdentifier;
        }

        private List<Guid> GetSurveyResponses(Guid learner, Guid form)
        {
            using (var db = new InternalDbContext())
            {
                return db.QResponseSessions
                    .Where(x => x.SurveyFormIdentifier == form && x.RespondentUserIdentifier == learner)
                    .Select(x => x.ResponseSessionIdentifier)
                    .ToList();
            }
        }

        private void UpdateCourseEnrollmentStatus(Guid learner, Guid course, DateTimeOffset started)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QCourseEnrollments
                    .Where(x => x.CourseIdentifier == course && x.LearnerUserIdentifier == learner)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                entity.CourseStarted = started;
                entity.CourseCompleted = null;

                db.SaveChanges();
            }
        }
    }
}