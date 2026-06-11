using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormNameChanged : Change
    {
        public Guid Form { get; set; }
        public string Name { get; set; }

        public FormNameChanged(Guid form, string name)
        {
            Form = form;
            Name = name;
        }
    }
}
