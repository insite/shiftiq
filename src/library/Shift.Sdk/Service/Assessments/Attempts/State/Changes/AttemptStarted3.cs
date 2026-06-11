using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Attempts
{
    public class AttemptStarted3 : Change
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid BankIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid AssessorUserIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }
        public string UserAgent { get; set; }

        public AttemptConfiguration Configuration { get; set; }
        public AttemptSection[] Sections { get; set; }
        public AttemptQuestion[] Questions { get; set; }

        public AttemptStarted3(Guid organizationIdentifier, Guid bankIdentifier, Guid formIdentifier, Guid assessorUserIdentifier, Guid learnerUserIdentifier, Guid? registrationIdentifier, string userAgent, AttemptConfiguration configuration, AttemptSection[] sections, AttemptQuestion[] questions)
        {
            OrganizationIdentifier = organizationIdentifier;
            BankIdentifier = bankIdentifier;
            FormIdentifier = formIdentifier;
            AssessorUserIdentifier = assessorUserIdentifier;
            LearnerUserIdentifier = learnerUserIdentifier;
            RegistrationIdentifier = registrationIdentifier;
            UserAgent = userAgent;
            Configuration = configuration;
            Sections = sections;
            Questions = questions;
        }
    }
}
