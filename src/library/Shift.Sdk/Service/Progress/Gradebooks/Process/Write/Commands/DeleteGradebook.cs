﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class DeleteGradebook : Command
    {

        public DeleteGradebook(Guid record)
        {
            AggregateIdentifier = record;
        }
    }
}