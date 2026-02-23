using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DuplicateQuestion : Command
    {
        public Guid SourceQuestion { get; set; }
        public Guid DestinationQuestion { get; set; }
        public int DestinationAsset { get; set; }

        public DuplicateQuestion(Guid bank, Guid sourceQuestion, Guid destinationQuestion, int destinationAsset)
        {
            AggregateIdentifier = bank;
            SourceQuestion = sourceQuestion;
            DestinationQuestion = destinationQuestion;
            DestinationAsset = destinationAsset;
        }
    }
}