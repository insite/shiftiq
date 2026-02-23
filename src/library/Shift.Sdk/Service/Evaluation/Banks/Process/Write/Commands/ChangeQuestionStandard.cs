using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionStandard : Command
    {
        public Guid Question { get; set; }
        public Guid Standard { get; set; }
        public Guid[] SubStandards { get; set; }

        public ChangeQuestionStandard(Guid bank, Guid question, Guid standard, Guid[] subStandards)
        {
            AggregateIdentifier = bank;
            Question = question;
            Standard = standard;
            SubStandards = subStandards.NullIfEmpty();
        }
    }
}
