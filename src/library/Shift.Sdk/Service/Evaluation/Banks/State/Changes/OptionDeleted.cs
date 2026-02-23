using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class OptionDeleted : Change
    {
        public Guid Question { get; set; }
        public int Option { get; set; }

        public OptionDeleted(Guid question, int option)
        {
            Question = question;
            Option = option;
        }
    }
}
