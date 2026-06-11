using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ExamFormAssigned : Change
    {
        public Guid Form { get; set; }

        public Guid? PreviousForm { get; set; }

        public ExamFormAssigned(Guid form, Guid? previousForm)
        {
            Form = form;
            PreviousForm = previousForm;
        }
    }
}
