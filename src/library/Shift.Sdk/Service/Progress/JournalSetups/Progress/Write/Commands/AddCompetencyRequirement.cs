﻿using System;

using Common.Timeline.Commands;

namespace InSite.Application.JournalSetups.Write
{
    public class AddCompetencyRequirement : Command
    {
        public Guid Competency { get; }
        public decimal? Hours { get; }
        public int? JournalItems { get; }
        public int? SkillRating { get; }
        public bool IncludeHoursToArea { get; }

        public AddCompetencyRequirement(Guid journalSetup, Guid competency, decimal? hours, int? journalItems, int? skillRating, bool includeHoursToArea)
        {
            AggregateIdentifier = journalSetup;
            Competency = competency;
            Hours = hours;
            JournalItems = journalItems;
            SkillRating = skillRating;
            IncludeHoursToArea = includeHoursToArea;
        }
    }
}
