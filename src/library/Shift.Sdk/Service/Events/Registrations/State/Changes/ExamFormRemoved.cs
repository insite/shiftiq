using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ExamFormRemoved : Change
    {
        public Guid Form { get; set; }

        public ExamFormRemoved(Guid form)
        {
            Form = form;
        }
    }
}
