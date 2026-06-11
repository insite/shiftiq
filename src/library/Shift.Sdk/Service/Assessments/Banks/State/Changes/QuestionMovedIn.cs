using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionMovedIn : Change
    {
        public Guid SourceBank { get; set; }
        public Guid DestinationSet { get; set; }
        public Guid DestinationCompetency { get; set; }
        public int Asset { get; set; }
        public Question Question { get; set; }
        public Comment[] Comments { get; set; }

        public QuestionMovedIn(Guid bank, Guid set, Guid competency, int asset, Question question, Comment[] comments)
        {
            SourceBank = bank;
            DestinationSet = set;
            DestinationCompetency = competency;

            Asset = asset;
            Question = question;
            Comments = comments;
        }
    }
}
