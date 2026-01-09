using System;
using System.Data.Entity;
using System.Linq;

using InSite.Application;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Persistence
{
    public class Course1Store : ICourseObjectStore
    {
        private static ICommander _commander;

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        public void InsertCatalog(TCatalog catalog)
        {
            using (var db = new InternalDbContext())
            {
                db.TCatalogs.Add(catalog);
                db.SaveChanges();
            }
        }

        public void UpdateCatalog(TCatalog catalog)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(catalog).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void DeleteCatalog(Guid catalog)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TCatalogs.FirstOrDefault(x => x.CatalogIdentifier == catalog);
                if (entity == null)
                    return;
                
                db.TCatalogs.Remove(entity);
                db.SaveChanges();
            }
        }

        public void InsertCourseUser(Guid course, Guid user)
        {
            _commander.Send(new AddCourseEnrollment(course, user, UniqueIdentifier.Create(), DateTimeOffset.UtcNow));
        }

        public void IncreaseMessageStalledSentCount(Guid course, Guid enrollment)
        {
            _commander.Send(new IncreaseCourseEnrollment(course, enrollment, CourseEnrollmentMessageType.Stalled));
        }

        public void IncreaseMessageCompletedSentCount(Guid course, Guid enrollment)
        {
            _commander.Send(new IncreaseCourseEnrollment(course, enrollment, CourseEnrollmentMessageType.Completed));
        }

        public void CompleteCourse(Guid course, Guid user, DateTimeOffset completed)
        {
            Guid? enrollmentId;

            using (var db = new InternalDbContext())
            {
                enrollmentId = db.QCourseEnrollments
                    .Where(x => x.CourseIdentifier == course && x.LearnerUserIdentifier == user)
                    .FirstOrDefault()?
                    .CourseEnrollmentIdentifier;
            }

            if (enrollmentId.HasValue)
                _commander.Send(new CompleteCourseEnrollment(course, enrollmentId.Value, completed));
        }
    }
}