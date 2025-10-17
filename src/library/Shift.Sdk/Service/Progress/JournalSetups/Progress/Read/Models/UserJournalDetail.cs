using System;

namespace InSite.Application.Records.Read
{
    public class UserJournalDetail
    {
        public Guid JournalSetupIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset? JournalSetupLocked { get; set; }
        public string Title { get; set; }
        public DateTimeOffset JournalSetupCreated { get; set; }
        public int ExperienceCount { get; set; }
    }
}
