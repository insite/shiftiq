﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class DeleteJournal : Command
    {
        public DeleteJournal(Guid journal)
        {
            AggregateIdentifier = journal;
        }
    }
}
