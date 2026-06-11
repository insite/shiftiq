using System;
using System.Linq;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class Course1Search : ICourseObjectSearch
    {
        public QActivity FindActivityByAssessmentForm(Guid form)
        {
            return CourseSearch.BindActivityFirst(x => x, x => x.AssessmentFormIdentifier == form);
        }

        public QActivity FindActivityBySurveyForm(Guid form)
        {
            var first = CourseSearch.BindActivityFirst(x => x.ActivityIdentifier, x => x.SurveyFormIdentifier == form);
            if (first == null)
                return null;
            return CourseSearch.SelectActivity(first, x => x.Module.Unit.Course, x => x.GradeItem);
        }

        public Guid? GetActivityGradebookIdBySurveyForm(Guid form)
        {
            return CourseSearch.BindActivityFirst(
                x => x.Module.Unit.Course.GradebookIdentifier,
                x => x.SurveyFormIdentifier == form);
        }

        public QActivity FindActivityByGradeItem(Guid gradeItem)
        {
            return CourseSearch.BindActivityFirst(x => x, x => x.GradeItemIdentifier == gradeItem);
        }

        public QCourse FindCourseByGradebook(Guid gradebook)
        {
            using (var db = new InternalDbContext())
            {
                return db.QCourses
                    .AsNoTracking()
                    .Where(x => x.GradebookIdentifier == gradebook)
                    .FirstOrDefault();
            }
        }

        public bool IsActivityCompleted(Guid activityId, Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                var activity = db.QActivities
                    .Where(x => x.ActivityIdentifier == activityId)
                    .FirstOrDefault();

                if (activity == null)
                    return false;

                if (activity.GradeItemIdentifier == null)
                    return true;

                var progress = db.QProgresses
                    .Where(x =>
                        x.GradeItemIdentifier == activity.GradeItemIdentifier
                        && x.UserIdentifier == userId
                    )
                    .FirstOrDefault();

                return progress != null
                    && !string.Equals(progress.ProgressPassOrFail, "Fail", StringComparison.OrdinalIgnoreCase)
                    && (progress.ProgressPassOrFail != null || progress.ProgressPercent == null || progress.ProgressPercent == 1);
            }
        }

        public QCourseEnrollment GetCourseEnrollment(Guid course, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.QCourseEnrollments
                    .Where(x => x.CourseIdentifier == course && x.LearnerUserIdentifier == user)
                    .FirstOrDefault();
            }
        }

        public QCourse GetCourse(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                return db.QCourses
                    .Where(x => x.CourseIdentifier == course)
                    .FirstOrDefault();
            }
        }

        public string GetOrganizationCode(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.Organizations.AsNoTracking().AsQueryable()
                    .Where(x => x.OrganizationIdentifier == organization)
                    .Select(x => x.OrganizationCode)
                    .FirstOrDefault();
            }
        }
    }
}