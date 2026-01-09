using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionMovedOut : Change
    {
        public Guid DestinationBank { get; set; }
        public Guid DestinationSet { get; set; }
        public Guid DestinationCompetency { get; set; }
        public Guid Question { get; set; }

        public QuestionMovedOut(Guid bank, Guid set, Guid competency, Guid question)
        {
            DestinationBank = bank;
            DestinationSet = set;
            DestinationCompetency = competency;
            Question = question;
        }
    }
}
