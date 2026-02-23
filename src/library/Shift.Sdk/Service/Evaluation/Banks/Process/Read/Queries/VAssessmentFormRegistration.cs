using System;

namespace InSite.Application.Banks.Read
{
    public class VAssessmentFormRegistration
    {
        public Guid EventIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }

        public string LearnerPersonCode { get; set; }
        public Guid LearnerUserIdentifier { get; set; }

        public Guid AssessmentBankIdentifier { get; set; }
        public Guid AssessmentFormIdentifier { get; set; }
    }
}
