﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class ReviewResponseSession : Command
    {
        public ReviewResponseSession(Guid session)
        {
            AggregateIdentifier = session;
        }
    }
}