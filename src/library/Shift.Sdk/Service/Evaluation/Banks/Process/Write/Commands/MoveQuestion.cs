using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class MoveQuestion : Command
    {
        public Guid Bank { get; set; }
        public Guid Set { get; set; }
        public Guid Competency { get; set; }
        public int Asset { get; set; }
        public Guid Question { get; set; }

        public MoveQuestion(Guid sourceBank, Guid destinationBank, Guid destinationSet, Guid destinationCompetency, int asset, Guid question)
        {
            AggregateIdentifier = sourceBank;

            Bank = destinationBank;
            Set = destinationSet;
            Competency = destinationCompetency;
            Asset = asset;
            Question = question;
        }
    }
}
