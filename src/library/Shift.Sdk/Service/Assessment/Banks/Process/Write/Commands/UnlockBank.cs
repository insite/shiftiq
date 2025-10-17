﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class UnlockBank : Command
    {
        public UnlockBank(Guid bank)
        {
            AggregateIdentifier = bank;
        }
    }
}