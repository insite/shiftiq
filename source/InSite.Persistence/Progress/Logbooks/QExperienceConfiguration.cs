using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QExperienceConfiguration : EntityTypeConfiguration<QExperience>
    {
        public QExperienceConfiguration() : this("records") { }

        public QExperienceConfiguration(string schema)
        {
            ToTable(schema + ".QExperience");
            HasKey(x => new { x.ExperienceIdentifier });

            Property(x => x.MediaEvidenceName).HasMaxLength(100);
            Property(x => x.MediaEvidenceType).HasMaxLength(5).IsUnicode(false);

            HasRequired(a => a.Journal).WithMany(b => b.Experiences).HasForeignKey(c => c.JournalIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Validator).WithMany(b => b.ValidatorExperiences).HasForeignKey(c => c.ValidatorUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
