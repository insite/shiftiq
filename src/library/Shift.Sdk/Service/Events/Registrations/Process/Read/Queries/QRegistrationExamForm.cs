using System;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;

namespace InSite.Application.Registrations.Read
{
    public class QRegistrationExamForm
    {
        public Guid RegistrationExamFormIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid ExamFormIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? AttemptIdentifier { get; set; }
        public string Password { get; set; }
        public int? ExamTimeLimit { get; set; }
        public string EligibilityProcess { get; set; }
        public string EligibilityStatus { get; set; }
        public DateTimeOffset? EligibilityUpdated { get; set; }
        public int Sequence { get; set; }

        public virtual QRegistration Registration { get; set; }
        public virtual QBankForm Form { get; set; }
        public virtual QAttempt Attempt { get; set; }
    }
}
