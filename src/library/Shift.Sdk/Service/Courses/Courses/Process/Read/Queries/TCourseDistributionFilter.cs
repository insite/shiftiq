using System;

using Shift.Common;

namespace InSite.Application.Courses.Read
{
    [Serializable]
    public class TCourseDistributionFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? ProductIdentifier { get; set; }
        public Guid? CourseIdentifier { get; set; }
        public Guid? ManagerUserIdentifier { get; set; }
        public Guid? LearnerUserIdentifier { get; set; }
        public Guid? CourseEnrollmentIdentifier { get; set; }
        public string DistributionStatus { get; set; }
    }
}
