using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupAchievementChanged : Change
    {
        public Guid? Achievement { get; }

        public JournalSetupAchievementChanged(Guid? achievement)
        {
            Achievement = achievement;
        }
    }
}
