using System;

namespace InSite.Domain.Integrations.Prometric
{
    public class SaveEligibilityInput
    {
        public string Client { get; set; }
        public string Action { get; set; }

        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }

        public string LearnerCode { get; set; }
        public string LearnerEmail { get; set; }

        public Guid LearnerIdentifier { get; set; }

        public string ExamEventPassword { get; set; }
        public string AssessmentFormCode { get; set; }
        public string AssessmentFormProgram { get; set; }
        public Guid AssessmentFormIdentifier { get; set; }

        public SaveEligibilityAccommodationItem[] Accommodations { get; set; }
    }
}