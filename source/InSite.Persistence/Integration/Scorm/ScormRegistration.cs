using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class TScormRegistration
    {
        public virtual ICollection<TScormRegistrationActivity> Activities { get; set; } = new HashSet<TScormRegistrationActivity>(); 

        public Guid LearnerIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ScormRegistrationIdentifier { get; set; }

        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }
        public string ScormPackageHook { get; set; }
        public string ScormRegistrationCompletion { get; set; }
        public string ScormRegistrationSuccess { get; set; }

        public int ScormLaunchCount { get; set; }
        public int? ScormRegistrationInstance { get; set; }
        public int? ScormRegistrationTrackedSeconds { get; set; }

        public decimal? ScormRegistrationScore { get; set; }

        public DateTimeOffset? ScormAccessedFirst { get; set; }
        public DateTimeOffset? ScormAccessedLast { get; set; }
        public DateTimeOffset? ScormCompleted { get; set; }
        public DateTimeOffset ScormLaunchedFirst { get; set; }
        public DateTimeOffset ScormLaunchedLast { get; set; }
        public DateTimeOffset? ScormSynchronized { get; set; }
    }
}
