using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceMediaEvidenceChanged : Change
    {
        public Guid Experience { get; }
        public string Name { get; }
        public string Type { get; }
        public Guid? FileIdentifier { get; set; }

        public ExperienceMediaEvidenceChanged(Guid experience, string name, string type, Guid? fileIdentifier)
        {
            Experience = experience;
            Name = name;
            Type = type;
            FileIdentifier = fileIdentifier;
        }
    }
}
