﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class HideProgress : Command
    {
        public HideProgress(Guid progress)
        {
            AggregateIdentifier = progress;
        }
    }
}
