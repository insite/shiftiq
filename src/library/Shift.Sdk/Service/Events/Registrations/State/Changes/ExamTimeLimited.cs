using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ExamTimeLimited : Change
    {
        public Guid? Form { get; set; }
        public int? Minutes { get; set; }

        public ExamTimeLimited(Guid? form, int? minutes)
        {
            Form = form;
            Minutes = minutes;
        }
    }
}
