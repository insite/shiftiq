﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ReleaseGrade : Command
    {
        public ReleaseGrade(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
