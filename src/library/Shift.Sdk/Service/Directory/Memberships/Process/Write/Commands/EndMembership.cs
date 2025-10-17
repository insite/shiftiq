﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write
{
    public class EndMembership : Command
    {
        public EndMembership(Guid membership)
        {
            AggregateIdentifier = membership;
        }
    }
}
