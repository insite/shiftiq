using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionMoved : Change
    {
        public Guid DestinationSet { get; set; }
        public Guid DestinationCompetency { get; set; }
        public int Asset { get; set; }
        public Guid Question { get; set; }

        public QuestionMoved(Guid set, Guid competency, int asset, Guid question)
        {
            DestinationSet = set;
            DestinationCompetency = competency;
            Asset = asset;
            Question = question;
        }
    }
}
