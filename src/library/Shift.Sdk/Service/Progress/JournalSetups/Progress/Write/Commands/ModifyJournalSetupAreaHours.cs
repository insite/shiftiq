using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class ModifyJournalSetupAreaHours : Command
    {
        public Guid Area { get; }
        public decimal? Hours { get; }

        public ModifyJournalSetupAreaHours(Guid journalSetup, Guid area, decimal? hours)
        {
            AggregateIdentifier = journalSetup;
            Area = area;
            Hours = hours;
        }
    }
}
