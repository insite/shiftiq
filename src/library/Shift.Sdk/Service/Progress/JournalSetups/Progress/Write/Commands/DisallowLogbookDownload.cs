﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class DisallowLogbookDownload : Command
    {
        public DisallowLogbookDownload(Guid journalSetup)
        {
            AggregateIdentifier = journalSetup;
        }
    }
}
