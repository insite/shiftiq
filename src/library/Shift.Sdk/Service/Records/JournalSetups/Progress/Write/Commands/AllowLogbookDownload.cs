using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class AllowLogbookDownload : Command
    {
        public AllowLogbookDownload(Guid journalSetup)
        {
            AggregateIdentifier = journalSetup;
        }
    }
}
