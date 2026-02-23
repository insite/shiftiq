using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormInvigilationChanged : Change
    {
        public Guid Form { get; set; }
        public FormInvigilation Invigilation { get; set; }

        public FormInvigilationChanged(Guid form, FormInvigilation invigilation)
        {
            Form = form;
            Invigilation = invigilation;
        }
    }
}
