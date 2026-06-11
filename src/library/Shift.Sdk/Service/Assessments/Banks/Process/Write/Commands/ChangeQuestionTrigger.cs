using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionTrigger : Command
    {
        public Guid Question { get; set; }
        public int Index { get; set; }
        public int Asset { get; set; }
        public decimal ScoreFrom { get; set; }
        public decimal ScoreThru { get; set; }

        public ChangeQuestionTrigger(Guid bank, Guid question, int index, int assetNumber, decimal scoreFrom, decimal scoreThru)
        {
            AggregateIdentifier = bank;
            Question = question;
            Index = index;
            Asset = assetNumber;
            ScoreFrom = scoreFrom;
            ScoreThru = scoreThru;
        }
    }
}
