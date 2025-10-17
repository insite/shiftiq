﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class LockEventRegistration : Command
    {
        public LockEventRegistration(Guid eventId)
        {
            AggregateIdentifier = eventId;
        }
    }
}
