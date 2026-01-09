using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItemMaxPoints : Command
    {
        public Guid Item { get; set; }
        public decimal? MaxPoints { get; set; }

        public ChangeGradeItemMaxPoints(Guid record, Guid item, decimal? maxPoints)
        {
            AggregateIdentifier = record;
            Item = item;
            MaxPoints = maxPoints;
        }
    }
}
