using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormUnpublished : Change
    {
        public Guid Form { get; set; }

        public FormUnpublished(Guid form)
        {
            Form = form;
        }
    }
}
