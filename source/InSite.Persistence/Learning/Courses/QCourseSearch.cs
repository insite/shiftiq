using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class QCourseSearch : ICourseSearch
    {
        public QCourse GetCourse(Guid courseId)
        {
            using (var db = CreateContext())
                return db.QCourses.Where(x => x.CourseIdentifier == courseId).FirstOrDefault();
        }

        public QUnit GetUnit(Guid unitId)
        {
            using (var db = CreateContext())
                return db.QUnits.Where(x => x.UnitIdentifier == unitId).FirstOrDefault();
        }

        public QModule GetModule(Guid moduleId)
        {
            using (var db = CreateContext())
                return db.QModules.Where(x => x.ModuleIdentifier == moduleId).FirstOrDefault();
        }

        public QActivity GetActivity(Guid activityId)
        {
            using (var db = CreateContext())
                return db.QActivities.Where(x => x.ActivityIdentifier == activityId).FirstOrDefault();
        }

        public QActivity GetActivityBySurveyForm(Guid formId)
        {
            using (var db = CreateContext())
                return db.QActivities.Where(x => x.SurveyFormIdentifier == formId).FirstOrDefault();
        }

        public QActivity GetActivityByGradeItem(Guid gradeItemId)
        {
            using (var db = CreateContext())
                return db.QActivities.Where(x => x.GradeItemIdentifier == gradeItemId).FirstOrDefault();
        }

        public List<QCourseEnrollment> GetEnrollments(Guid courseId)
        {
            using (var db = CreateContext())
                return db.QCourseEnrollments.Where(x => x.CourseIdentifier == courseId).ToList();
        }

        public List<QCoursePrerequisite> GetPrerequisites(Guid courseId)
        {
            using (var db = CreateContext())
                return db.QCoursePrerequisites.Where(x => x.CourseIdentifier == courseId).ToList();
        }

        public List<QActivityCompetency> GetActivityCompetencies(Guid activityId)
        {
            using (var db = CreateContext())
                return db.QActivityCompetencies.Where(x => x.ActivityIdentifier == activityId).ToList();
        }

        public Guid? GetCourseIdByActivityId(Guid activityId)
        {
            using (var db = CreateContext())
            {
                return db.QActivities
                    .Where(x => x.ActivityIdentifier == activityId)
                    .Select(x => (Guid?)x.Module.Unit.CourseIdentifier)
                    .FirstOrDefault();
            }
        }

        public List<QCoursePrerequisite> GetPrerequisitesByTrigger(Guid triggerId)
        {
            using (var db = CreateContext())
                return db.QCoursePrerequisites.Where(x => x.TriggerIdentifier == triggerId).ToList();
        }

        private InternalDbContext CreateContext()
        {
            return new InternalDbContext();
        }
    }
}
