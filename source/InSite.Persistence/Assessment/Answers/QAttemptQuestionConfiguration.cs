using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class QAttemptQuestionConfiguration : EntityTypeConfiguration<QAttemptQuestion>
    {
        public QAttemptQuestionConfiguration() : this("assessments") { }

        public QAttemptQuestionConfiguration(string schema)
        {
            ToTable(schema + ".QAttemptQuestion");
            HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier });

            HasRequired(a => a.Attempt).WithMany(b => b.Questions).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);

            Property(x => x.AnswerOptionKey).IsOptional();
            Property(x => x.AnswerOptionSequence).IsOptional();
            Property(x => x.AnswerPoints).IsOptional();
            Property(x => x.AnswerText).IsOptional().IsUnicode(true);
            Property(x => x.AttemptIdentifier).IsRequired();
            Property(x => x.CompetencyAreaCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.CompetencyAreaIdentifier).IsOptional();
            Property(x => x.CompetencyAreaLabel).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.CompetencyAreaTitle).IsOptional().IsUnicode(false).HasMaxLength(300);
            Property(x => x.CompetencyItemCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.CompetencyItemIdentifier).IsOptional();
            Property(x => x.CompetencyItemLabel).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.CompetencyItemTitle).IsOptional().IsUnicode(false).HasMaxLength(300);
            Property(x => x.QuestionCalculationMethod).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.QuestionCutScore).IsOptional();
            Property(x => x.QuestionIdentifier).IsRequired();
            Property(x => x.QuestionMatchDistractors).IsOptional().IsUnicode(true);
            Property(x => x.QuestionPoints).IsOptional();
            Property(x => x.QuestionSequence).IsRequired();
            Property(x => x.QuestionText).IsOptional().IsUnicode(true);
            Property(x => x.QuestionType).IsRequired().IsUnicode(false).HasMaxLength(21);
            Property(x => x.HotspotImage).IsUnicode(false).HasMaxLength(512);
            Property(x => x.QuestionTopLabel).IsOptional().IsUnicode(true);
            Property(x => x.QuestionBottomLabel).IsOptional().IsUnicode(true);
            Property(x => x.RubricRatingPoints).IsOptional().IsUnicode(false).HasMaxLength(512);
        }
    }
}
