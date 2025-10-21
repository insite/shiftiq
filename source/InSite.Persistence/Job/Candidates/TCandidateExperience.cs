using System;

namespace InSite.Persistence
{
    public class TCandidateExperience
    {
        public Guid ExperienceIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string EmployerDescription { get; set; }
        public string EmployerName { get; set; }
        public string ExperienceCity { get; set; }
        public string ExperienceCountry { get; set; }
        public string ExperienceJobTitle { get; set; }

        public DateTime ExperienceDateFrom { get; set; }
        public DateTime? ExperienceDateTo { get; set; }
        public DateTimeOffset WhenModified { get; set; }

        public virtual User User { get; set; }
    }
}