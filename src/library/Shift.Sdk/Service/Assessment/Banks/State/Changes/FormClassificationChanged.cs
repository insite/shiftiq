using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormClassificationChanged : Change
    {
        public Guid Form { get; set; }
        public string Instrument { get; set; }
        public string Theme { get; set; }

        public FormClassificationChanged(Guid form, string instrument, string theme)
        {
            Form = form;
            Instrument = instrument;
            Theme = theme;
        }
    }
}
