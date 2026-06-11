using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookGroupEnrollmentRemoved : Change
    {
        public Guid Enrollment { get; set; }

        public GradebookGroupEnrollmentRemoved(Guid enrollment)
        {
            Enrollment = enrollment;
        }
    }
}
