using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ExperienceCapturedEvidenceChanged : Change
    {
        public Guid Experience { get; }
        public Guid? ResourceIdentifier { get; }
        public string FriendlyName { get; }
        public bool AudioOnly { get; set; }

        public ExperienceCapturedEvidenceChanged(Guid experience, Guid? resourceIdentifier, string friendlyName, bool audioOnly)
        {
            Experience = experience;
            ResourceIdentifier = resourceIdentifier;
            FriendlyName = friendlyName;
            AudioOnly = audioOnly;
        }
    }
}
