using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class ExamFormDetached : Change
    {
        public Guid Form { get; set; }

        public ExamFormDetached(Guid form)
        {
            Form = form;
        }
    }
}
