using System;
using System.Linq;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class Course1Search : ICourseObjectSearch
    {
        public TActivity FindActivityByAssessmentForm(Guid form)
        {
            return CourseSearch.BindActivityFirst(x => x, x => x.AssessmentFormIdentifier == form);
        }

        public TActivity FindActivityBySurveyForm(Guid form)
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

        public TActivity FindActivityByGradeItem(Guid gradeItem)
        {
            return CourseSearch.BindActivityFirst(x => x, x => x.GradeItemIdentifier == gradeItem);
        }

        public TCourse FindCourseByGradebook(Guid gradebook)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCourses
                    .AsNoTracking()
                    .Where(x => x.GradebookIdentifier == gradebook)
                    .FirstOrDefault();
            }
        }

        public bool IsActivityCompleted(Guid activityId, Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                var activity = db.TActivities
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

        public TCourseUser GetCourseUser(Guid course, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCourseUsers
                    .Where(x => x.CourseIdentifier == course && x.UserIdentifier == user)
                    .FirstOrDefault();
            }
        }

        public TCourse GetCourse(Guid course)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCourses
                    .Where(x => x.CourseIdentifier == course)
                    .FirstOrDefault();
            }
        }

        public LegacyPrerequisite[] GetLegacyPrerequisites()
        {
            var sql = @"
    SELECT PrerequisiteCondition, CourseIdentifier, ActivityIdentifier, PrerequisiteActivityIdentifier AS TriggerActivityIdentifier
    FROM courses.TActivity AS a
    INNER JOIN courses.TModule AS m ON a.ModuleIdentifier = m.ModuleIdentifier
    INNER JOIN courses.TUnit AS u ON m.UnitIdentifier = u.UnitIdentifier
    WHERE PrerequisiteCondition IS NOT NULL AND PrerequisiteCondition <> 'None'
";
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<LegacyPrerequisite>(sql).ToArray();
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