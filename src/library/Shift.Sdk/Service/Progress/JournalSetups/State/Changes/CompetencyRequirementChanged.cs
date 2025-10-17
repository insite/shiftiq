﻿using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CompetencyRequirementChanged : Change
    {
        public Guid Competency { get; }
        public decimal? Hours { get; }
        public int? JournalItems { get; }
        public int? SkillRating { get; }
        public bool? IncludeHoursToArea { get; }

        public CompetencyRequirementChanged(Guid competency, decimal? hours, int? journalItems, int? skillRating, bool? includeHoursToArea)
        {
            Competency = competency;
            Hours = hours;
            JournalItems = journalItems;
            SkillRating = skillRating;
            IncludeHoursToArea = includeHoursToArea;
        }
    }
}
