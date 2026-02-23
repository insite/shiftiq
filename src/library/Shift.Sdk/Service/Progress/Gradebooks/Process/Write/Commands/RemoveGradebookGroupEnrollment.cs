using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class RemoveGradebookGroupEnrollment : Command
    {
        public Guid Enrollment { get; set; }

        public RemoveGradebookGroupEnrollment(Guid gradebook, Guid enrollment)
        {
            AggregateIdentifier = gradebook;
            Enrollment = enrollment;
        }
    }
}
