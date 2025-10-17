﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AnalyzeBank : Command
    {
        public AnalyzeBank(Guid bank)
        {
            AggregateIdentifier = bank;
        }
    }
}