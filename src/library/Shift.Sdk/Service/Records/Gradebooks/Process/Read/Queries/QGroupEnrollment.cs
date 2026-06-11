using System;
using System.Collections.Generic;
using System.Text;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class QGroupEnrollment
    {
        public Guid GradebookIdentifier { get; set; }
        public Guid GroupEnrollmentIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public DateTimeOffset? EnrollmentStarted { get; set; }

        public virtual QGradebook Gradebook { get; set; }
        public virtual QGroup Group { get; set; }
    }
}
