using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradebookAchievement : Command
    {
        public ChangeGradebookAchievement(Guid record, Guid? achievement)
        {
            AggregateIdentifier = record;
            Achievement = achievement;
        }

        public Guid? Achievement { get; set; }
    }
}
