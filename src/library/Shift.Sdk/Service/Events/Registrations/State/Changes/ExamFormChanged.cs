using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ExamFormChanged : Change
    {
        public Guid FromForm { get; set; }
        public Guid ToForm { get; set; }

        public ExamFormChanged(Guid fromForm, Guid toForm)
        {
            FromForm = fromForm;
            ToForm = toForm;
        }
    }
}
