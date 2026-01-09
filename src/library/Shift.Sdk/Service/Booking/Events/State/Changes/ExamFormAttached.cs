using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class ExamFormAttached : Change
    {
        public Guid Form { get; set; }

        public ExamFormAttached(Guid form)
        {
            Form = form;
        }
    }
}
