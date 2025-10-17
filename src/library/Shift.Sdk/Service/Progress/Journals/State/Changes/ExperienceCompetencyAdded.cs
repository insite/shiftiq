﻿using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceCompetencyAdded : Change
    {
        public Guid Experience { get; }
        public Guid Competency { get; }
        public decimal? Hours { get; }

        public ExperienceCompetencyAdded(Guid experience, Guid competency, decimal? hours)
        {
            Experience = experience;
            Competency = competency;
            Hours = hours;
        }
    }
}
