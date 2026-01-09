using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CmdsCompetenciesExpired : Change
    {
        public DateTimeOffset Notified { get; set; }

        public CmdsCompetenciesExpired(DateTimeOffset notified)
        {
            Notified = notified;
        }
    }
}