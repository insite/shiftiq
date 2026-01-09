using System;

namespace InSite.Application.Courses.Read
{
    [Serializable]
    public class QCoursePrerequisite
    {
        public Guid CoursePrerequisiteIdentifier { get; set; }
        public Guid CourseIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid TriggerIdentifier { get; set; }

        public string ObjectType { get; set; }
        public string TriggerChange { get; set; }
        public string TriggerType { get; set; }

        public int? TriggerConditionScoreFrom { get; set; }
        public int? TriggerConditionScoreThru { get; set; }

        public virtual QCourse Course { get; set; }
    }
}
