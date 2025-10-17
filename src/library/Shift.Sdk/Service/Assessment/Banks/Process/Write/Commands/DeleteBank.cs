﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteBank : Command
    {
        public DeleteBank(Guid bank)
        {
            AggregateIdentifier = bank;
        }
    }
}