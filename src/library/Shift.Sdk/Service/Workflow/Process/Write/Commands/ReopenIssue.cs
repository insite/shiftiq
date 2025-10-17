﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ReopenIssue : Command
    {
        public ReopenIssue(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
