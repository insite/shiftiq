﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class CloseIssue : Command
    {
        public CloseIssue(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
