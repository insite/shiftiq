using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormVersionChanged : Change
    {
        public Guid Form { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }

        public FormVersionChanged(Guid form, string major, string minor)
        {
            Form = form;
            Major = major;
            Minor = minor;
        }
    }
}
