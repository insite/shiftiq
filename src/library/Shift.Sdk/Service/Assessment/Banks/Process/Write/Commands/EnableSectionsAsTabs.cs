﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class EnableSectionsAsTabs : Command
    {
        public Guid Specification { get; set; }

        public EnableSectionsAsTabs(Guid bank, Guid spec)
        {
            AggregateIdentifier = bank;

            Specification = spec;
        }
    }
}
