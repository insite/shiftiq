using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeAssessmentHook : Command
    {
        public Guid Form { get; set; }
        public string Hook { get; set; }

        public ChangeAssessmentHook(Guid bank, Guid form, string hook)
        {
            AggregateIdentifier = bank;
            Form = form;
            Hook = hook.NullIfWhiteSpace();
        }
    }
}
