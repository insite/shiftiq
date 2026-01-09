using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class JournalSetupAreaHoursModified : Change
    {
        public Guid Area { get; }
        public decimal? Hours { get; }

        public JournalSetupAreaHoursModified(Guid area, decimal? hours)
        {
            Area = area;
            Hours = hours;
        }
    }
}
