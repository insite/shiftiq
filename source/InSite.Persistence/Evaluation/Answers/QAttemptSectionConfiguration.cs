using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class QAttemptSectionConfiguration : EntityTypeConfiguration<QAttemptSection>
    {
        public QAttemptSectionConfiguration() : this("assessments") { }

        public QAttemptSectionConfiguration(string schema)
        {
            ToTable(schema + ".QAttemptSection");
            HasKey(x => new { x.AttemptIdentifier, x.SectionIndex });

            Property(x => x.TimerType).IsUnicode(false).HasMaxLength(8);

            HasRequired(a => a.Attempt).WithMany(b => b.Sections).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);
        }
    }
}
