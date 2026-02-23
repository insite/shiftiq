using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class QAttemptMatchConfiguration : EntityTypeConfiguration<QAttemptMatch>
    {
        public QAttemptMatchConfiguration() : this("assessments") { }

        public QAttemptMatchConfiguration(string schema)
        {
            ToTable(schema + ".QAttemptMatch");
            HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier, x.MatchSequence });

            HasRequired(a => a.Attempt).WithMany(b => b.Matches).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);

            Property(x => x.AnswerText).IsOptional().IsUnicode(true);
            Property(x => x.AttemptIdentifier).IsRequired();
            Property(x => x.MatchLeftText).IsRequired().IsUnicode(true);
            Property(x => x.MatchPoints).IsRequired();
            Property(x => x.MatchRightText).IsRequired().IsUnicode(true);
            Property(x => x.MatchSequence).IsRequired();
            Property(x => x.QuestionIdentifier).IsRequired();
            Property(x => x.QuestionSequence).IsRequired();
        }
    }
}
