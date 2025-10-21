using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class QAttemptOptionConfiguration : EntityTypeConfiguration<QAttemptOption>
    {
        public QAttemptOptionConfiguration() : this("assessments") { }

        public QAttemptOptionConfiguration(string schema)
        {
            ToTable(schema + ".QAttemptOption");
            HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier, x.OptionKey });

            HasRequired(a => a.Attempt).WithMany(b => b.Options).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);

            Property(x => x.OptionCutScore).HasPrecision(5, 4);
            Property(x => x.OptionPoints).HasPrecision(7, 2);
            Property(x => x.OptionShape).IsUnicode(false).HasMaxLength(32);
            Property(x => x.CompetencyAreaCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.CompetencyAreaLabel).IsUnicode(false).HasMaxLength(40);
            Property(x => x.CompetencyAreaTitle).IsUnicode(false).HasMaxLength(300);
            Property(x => x.CompetencyItemCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.CompetencyItemLabel).IsUnicode(false).HasMaxLength(40);
            Property(x => x.CompetencyItemTitle).IsUnicode(false).HasMaxLength(300);
        }
    }
}
