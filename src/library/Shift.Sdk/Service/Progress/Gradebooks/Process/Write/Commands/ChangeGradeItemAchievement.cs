using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItemAchievement : Command
    {
        public ChangeGradeItemAchievement(Guid record, Guid item, GradeItemAchievement achievement)
        {
            AggregateIdentifier = record;
            Item = item;
            Achievement = achievement;
        }

        public Guid Item { get; set; }
        public GradeItemAchievement Achievement { get; set; }
    }
}
