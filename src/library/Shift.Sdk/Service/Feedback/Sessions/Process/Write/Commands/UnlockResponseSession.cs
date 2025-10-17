﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class UnlockResponseSession : Command
    {
        public UnlockResponseSession(Guid session)
        {
            AggregateIdentifier = session;
        }
    }
}