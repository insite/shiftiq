using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCandidateExperienceConfiguration : EntityTypeConfiguration<TCandidateExperience>
    {
        public TCandidateExperienceConfiguration() : this("jobs") { }

        public TCandidateExperienceConfiguration(string schema)
        {
            ToTable(schema + ".TCandidateExperience");
            HasKey(x => new { x.ExperienceIdentifier });

            Property(x => x.ExperienceIdentifier).IsRequired();
            Property(x => x.UserIdentifier);
            Property(x => x.EmployerDescription).IsUnicode(false).HasMaxLength(500);
            Property(x => x.EmployerName).IsRequired().IsUnicode(false).HasMaxLength(300);
            Property(x => x.ExperienceCity).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ExperienceCountry).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ExperienceJobTitle).IsUnicode(false).HasMaxLength(200);
            Property(x => x.ExperienceDateFrom).IsRequired();
            Property(x => x.ExperienceDateTo);
            Property(x => x.WhenModified).IsRequired();

            HasOptional(a => a.User).WithMany(b => b.CandidateExperiences).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}