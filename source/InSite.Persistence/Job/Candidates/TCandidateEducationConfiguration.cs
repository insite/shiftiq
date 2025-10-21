using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCandidateEducationConfiguration : EntityTypeConfiguration<TCandidateEducation>
    {
        public TCandidateEducationConfiguration() : this("jobs") { }

        public TCandidateEducationConfiguration(string schema)
        {
            ToTable(schema + ".TCandidateEducation");
            HasKey(x => new { x.EducationIdentifier });

            Property(x => x.EducationIdentifier).IsRequired();
            Property(x => x.UserIdentifier);
            Property(x => x.EducationCity).IsRequired().IsUnicode(true).HasMaxLength(100);
            Property(x => x.EducationCountry).IsRequired().IsUnicode(true).HasMaxLength(100);
            Property(x => x.EducationInstitution).IsRequired().IsUnicode(false).HasMaxLength(300);
            Property(x => x.EducationName).IsRequired().IsUnicode(false).HasMaxLength(300);
            Property(x => x.EducationQualification).IsUnicode(true).HasMaxLength(100);
            Property(x => x.EducationStatus).IsUnicode(false).HasMaxLength(20);
            Property(x => x.EducationDateFrom).IsRequired();
            Property(x => x.EducationDateTo);
            Property(x => x.WhenModified).IsRequired();

            HasOptional(a => a.User).WithMany(b => b.CandidateEducations).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}