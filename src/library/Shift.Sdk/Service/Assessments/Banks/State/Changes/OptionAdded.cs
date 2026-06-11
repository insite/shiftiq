using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class OptionAdded : Change
    {
        public Guid Question { get; set; }
        public ContentTitle Content { get; set; }
        public decimal Points { get; set; }
        public bool? IsTrue { get; set; }
        public decimal? CutScore { get; set; }
        public Guid? Standard { get; set; }

        public OptionAdded(Guid question, ContentTitle content, decimal points, bool? isTrue, decimal? cutScore, Guid? standard)
        {
            Question = question;
            Content = content;
            Points = points;
            IsTrue = isTrue;
            CutScore = cutScore;
            Standard = standard;
        }
    }
}
