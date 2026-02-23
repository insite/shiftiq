using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Banks.Write
{
    public class ChangeOption : Command
    {
        public Guid Question { get; set; }
        public int Number { get; set; }
        public ContentTitle Content { get; set; }
        public decimal Points { get; set; }
        public bool? IsTrue { get; set; }
        public decimal? CutScore { get; set; }
        public Guid Standard { get; set; }

        public ChangeOption(Guid bank, Guid question, int number, ContentTitle content, decimal points, bool? isTrue, decimal? cutScore, Guid standard)
        {
            AggregateIdentifier = bank;
            Question = question;
            Number = number;
            Content = content;
            Points = points;
            IsTrue = isTrue;
            CutScore = cutScore;
            Standard = standard;
        }
    }
}
