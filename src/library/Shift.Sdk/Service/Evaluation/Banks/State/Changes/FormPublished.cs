using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormPublished : Change
    {
        public Guid Form { get; set; }
        public FormPublication Publication { get; set; }

        public FormPublished(Guid form, FormPublication publication)
        {
            Form = form;
            Publication = publication;
        }
    }
}
