using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    // TODO: Confirm no occurrences of this change in the log, then remove this class.
    public class ExamFormReplaced : Change
    {
        public Guid Form { get; set; }

        public ExamFormReplaced(Guid form)
        {
            Form = form;
        }
    }
}
