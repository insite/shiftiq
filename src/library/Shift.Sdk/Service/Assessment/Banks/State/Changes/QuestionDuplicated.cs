using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionDuplicated2 : Change
    {
        public Guid SourceQuestion { get; set; }
        public Guid DestinationQuestion { get; set; }
        public int DestinationAsset { get; set; }

        public QuestionDuplicated2(Guid sourceQuestion, Guid destinationQuestion, int destinationAsset)
        {
            SourceQuestion = sourceQuestion;
            DestinationQuestion = destinationQuestion;
            DestinationAsset = destinationAsset;
        }
    }
}
