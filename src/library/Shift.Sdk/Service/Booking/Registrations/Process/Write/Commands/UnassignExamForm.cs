﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class UnassignExamForm : Command
    {
        public UnassignExamForm(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
