using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookGroupEnrollmentAdded : Change
    {
        public Guid Enrollment { get; set; }
        public Guid Group { get; set; }

        public GradebookGroupEnrollmentAdded(Guid enrollment, Guid group)
        {
            Enrollment = enrollment;
            Group = group;
        }
    }
}
