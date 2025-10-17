﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class DeleteEvent : Command
    {
        public DeleteEvent(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
