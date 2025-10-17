﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class ArchiveMessage : Command
    {
        public ArchiveMessage(Guid message)
        {
            AggregateIdentifier = message;
        }
    }
}