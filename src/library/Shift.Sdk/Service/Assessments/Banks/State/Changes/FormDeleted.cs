using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormDeleted : Change
    {
        public Guid Form { get; set; }

        public FormDeleted(Guid form)
        {
            Form = form;
        }
    }
}
