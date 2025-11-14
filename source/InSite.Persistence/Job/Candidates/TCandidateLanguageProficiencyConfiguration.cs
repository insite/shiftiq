using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCandidateLanguageProficiencyConfiguration : EntityTypeConfiguration<TCandidateLanguageProficiency>
    {
        public TCandidateLanguageProficiencyConfiguration() : this("jobs") { }

        public TCandidateLanguageProficiencyConfiguration(string schema)
        {
            ToTable(schema + ".TCandidateLanguageProficiency");
            HasKey(x => new { x.LanguageProficiencyIdentifier });

            HasRequired(a => a.User).WithMany(b => b.CandidateLanguageProficiencies).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
