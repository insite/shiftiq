using System;
using System.Collections.Generic;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public partial class QCourseStore
    {
        class TransactionState
        {
            public class CompetencyChanges
            {
                public List<QActivityCompetency> Insert { get; } = new List<QActivityCompetency>();
                public HashSet<Guid> Remove { get; } = new HashSet<Guid>();
            }

            public Guid CourseId { get; }
            
            public EntityState<QCourse> Course { get; set; }
            public Dictionary<Guid, EntityState<QCourseEnrollment>> Enrollments { get; } = new Dictionary<Guid, EntityState<QCourseEnrollment>>();
            public Dictionary<Guid, EntityState<QCoursePrerequisite>> Prerequisites { get; } = new Dictionary<Guid, EntityState<QCoursePrerequisite>>();
            public Dictionary<Guid, CompetencyChanges> Competencies { get; } = new Dictionary<Guid, CompetencyChanges>();
            public Dictionary<Guid, EntityState<QUnit>> Units { get; } = new Dictionary<Guid, EntityState<QUnit>>();
            public Dictionary<Guid, EntityState<QModule>> Modules { get; } = new Dictionary<Guid, EntityState<QModule>>();
            public Dictionary<Guid, EntityState<QActivity>> Activities { get; } = new Dictionary<Guid, EntityState<QActivity>>();

            public TransactionState(Guid courseId)
            {
                CourseId = courseId;
            }
        }
    }
}
