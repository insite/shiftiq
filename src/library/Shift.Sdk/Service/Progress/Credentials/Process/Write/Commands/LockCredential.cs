﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class LockCredential : Command
    {
        public LockCredential(Guid credential)
        {
            AggregateIdentifier = credential;
        }
    }
}
