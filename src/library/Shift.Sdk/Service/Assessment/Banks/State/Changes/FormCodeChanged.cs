using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormCodeChanged : Change
    {
        public Guid Form { get; set; }
        public string Code { get; set; }
        public string Source { get; set; }
        public string Origin { get; set; }

        public FormCodeChanged(Guid form, string code, string source, string origin)
        {
            Form = form;
            Code = code;
            Source = source;
            Origin = origin;
        }
    }
}
