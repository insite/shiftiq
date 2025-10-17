﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class DeletePage : Command
    {
        public DeletePage(Guid page)
        {
            AggregateIdentifier = page;
        }
    }
}
