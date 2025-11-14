using System;

namespace InSite.Persistence
{
    public class TCandidateLanguageProficiency
    {
        public Guid LanguageProficiencyIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid LanguageItemIdentifier { get; set; }

        public int ProficiencyLevel { get; set; }
        public int Sequence { get; set; }

        public virtual User User { get; set; }
    }
}
