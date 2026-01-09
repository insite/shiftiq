using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AddQuestionTrigger : Command
    {
        public Guid Question { get; set; }
        public int Asset { get; set; }
        public decimal ScoreFrom { get; set; }
        public decimal ScoreThru { get; set; }

        public AddQuestionTrigger(Guid bank, Guid question, int assetNumber, decimal scoreFrom, decimal scoreThru)
        {
            AggregateIdentifier = bank;
            Question = question;
            Asset = assetNumber;
            ScoreFrom = scoreFrom;
            ScoreThru = scoreThru;
        }
    }
}
