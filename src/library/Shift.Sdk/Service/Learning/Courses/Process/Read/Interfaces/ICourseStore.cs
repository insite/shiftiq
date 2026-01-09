using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Read
{
    public interface ICourseStore
    {
        Guid StartTransaction(Guid courseId);
        void CancelTransaction(Guid transactionId);
        void CommitTransaction(Guid transactionId);

        void InsertCourse(CourseCreated e);
        void ModifyCourse(Change e, ContentContainer content, Action<QCourse> action);
        void RemoveCourse(CourseDeleted e);

        void InsertEnrollment(CourseEnrollmentAdded e);
        void ModifyEnrollment(Change e, Guid enrollmentId, Action<QCourseEnrollment> action);
        void RemoveEnrollment(CourseEnrollmentRemoved e);

        void InsertPrerequisite(Change e, Prerequisite p, Guid objectId, PrerequisiteObjectType objectType);
        void RemovePrerequisite(Change e, Guid prerequisiteId);

        void InsertCompetencies(CourseActivityCompetenciesAdded e);
        void RemoveCompetencies(CourseActivityCompetenciesRemoved e);

        void InsertUnit(CourseUnitAdded e);
        void ModifyUnit(Change e, Guid unitId, ContentContainer content, Action<QUnit> action);
        void RemoveUnit(CourseUnitRemoved e);

        void InsertModule(CourseModuleAdded e);
        void ModifyModule(Change e, Guid moduleId, ContentContainer content, Action<QModule> action);
        void RemoveModule(CourseModuleRemoved e);

        void InsertActivity(CourseActivityAdded e);
        void ModifyActivity(Change e, Guid activityId, ContentContainer content, Action<QActivity> action);
        void RemoveActivity(CourseActivityRemoved e);
    }
}
