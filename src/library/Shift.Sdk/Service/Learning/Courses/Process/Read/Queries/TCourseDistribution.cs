using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Invoices.Read;

namespace InSite.Application.Courses.Read
{
    public class TCourseDistribution
    {
        public Guid CourseDistributionIdentifier { get; set; }
        public Guid ProductIdentifier { get; set; }
        public Guid? CourseIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }
        public Guid ManagerUserIdentifier { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? CourseEnrollmentIdentifier { get; set; }
        public DateTimeOffset DistributionAssigned { get; set; }
        public string DistributionStatus { get; set; }
        public DateTimeOffset? DistributionRedeemed { get; set; }
        public DateTimeOffset? DistributionExpiry { get; set; }
        public string DistributionComment { get; set; }

        public virtual TProduct Product { get; set; }
        public virtual QCourse Course { get; set; }
        public virtual QCourseEnrollment CourseEnrollment { get; set; }
        public virtual VUser Manager { get; set; }
    }
}
