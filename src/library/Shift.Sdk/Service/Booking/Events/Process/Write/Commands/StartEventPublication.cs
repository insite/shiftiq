﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class StartEventPublication : Command
    {
        public StartEventPublication(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
