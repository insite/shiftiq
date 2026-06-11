using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class AddGradebookGroupEnrollment : Command
    {
        public Guid Enrollment { get; set; }
        public Guid Group { get; set; }

        public AddGradebookGroupEnrollment(Guid gradebook, Guid enrollment, Guid group)
        {
            AggregateIdentifier = gradebook;
            Enrollment = enrollment;
            Group = group;
        }
    }
}
