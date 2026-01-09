using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Read
{
    public class QPeriod
    {
        public Guid PeriodIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string PeriodName { get; set; }

        public DateTimeOffset PeriodEnd { get; set; }
        public DateTimeOffset PeriodStart { get; set; }

        public virtual ICollection<QEnrollment> Enrollments { get; set; } = new HashSet<QEnrollment>();
        public virtual ICollection<QGradebook> Gradebooks { get; set; } = new HashSet<QGradebook>();
    }
}