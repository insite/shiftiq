using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class ChangeExperienceMediaEvidence : Command
    {
        public Guid Experience { get; }
        public string Name { get; }
        public string Type { get; }
        public Guid? FileIdentifier { get; set; }

        public ChangeExperienceMediaEvidence(Guid journal, Guid experience, string name, string type, Guid? fileIdentifier)
        {
            AggregateIdentifier = journal;
            Experience = experience;
            Name = name;
            Type = type;
            FileIdentifier = fileIdentifier;
        }
    }
}
