using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class QAttemptPinConfiguration : EntityTypeConfiguration<QAttemptPin>
    {
        public QAttemptPinConfiguration() : this("assessments") { }

        public QAttemptPinConfiguration(string schema)
        {
            ToTable(schema + ".QAttemptPin");
            HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier, x.PinSequence });

            HasRequired(a => a.Attempt).WithMany(b => b.Pins).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);

            Property(x => x.OptionPoints).HasPrecision(7, 2);
            Property(x => x.OptionText).IsOptional().IsUnicode(true);
        }
    }
}
