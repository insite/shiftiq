﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class DisableAchievementBadgeImage : Command
    {
        public DisableAchievementBadgeImage(Guid achievement)
        {
            AggregateIdentifier = achievement;
        }
    }
}