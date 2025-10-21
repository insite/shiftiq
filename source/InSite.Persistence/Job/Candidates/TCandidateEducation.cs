using System;

namespace InSite.Persistence
{
    public class TCandidateEducation
    {
        public Guid EducationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string EducationCity { get; set; }
        public string EducationCountry { get; set; }
        public string EducationInstitution { get; set; }
        public string EducationName { get; set; }
        public string EducationQualification { get; set; }
        public string EducationStatus { get; set; }

        public DateTime EducationDateFrom { get; set; }
        public DateTime? EducationDateTo { get; set; }
        public DateTimeOffset WhenModified { get; set; }

        public virtual User User { get; set; }
    }
}
