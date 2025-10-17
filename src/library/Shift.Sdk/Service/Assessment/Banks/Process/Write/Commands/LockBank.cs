﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class LockBank : Command
    {
        public LockBank(Guid bank)
        {
            AggregateIdentifier = bank;
        }
    }
}