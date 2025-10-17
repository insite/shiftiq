﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class UnassignSchool : Command
    {
        public UnassignSchool(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
