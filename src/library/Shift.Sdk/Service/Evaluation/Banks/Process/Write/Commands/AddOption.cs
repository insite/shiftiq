using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Banks.Write
{
    public class AddOption : Command
    {
        public Guid Question { get; set; }
        public ContentTitle Content { get; set; }
        public decimal Points { get; set; }
        public bool? IsTrue { get; set; }
        public decimal? CutScore { get; set; }
        public Guid Standard { get; set; }

        public AddOption(Guid bank, Guid question, ContentTitle content, decimal points, bool? isTrue, decimal? cutScore, Guid standard)
        {
            AggregateIdentifier = bank;
            Question = question;
            Content = content;
            Points = points;
            IsTrue = isTrue;
            CutScore = cutScore;
            Standard = standard;
        }
    }
}
