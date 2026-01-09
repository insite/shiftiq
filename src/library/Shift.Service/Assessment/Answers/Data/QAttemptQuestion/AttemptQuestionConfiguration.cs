using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class AttemptQuestionConfiguration : IEntityTypeConfiguration<AttemptQuestionEntity>
{
    public void Configure(EntityTypeBuilder<AttemptQuestionEntity> builder) 
    {
        builder.ToTable("QAttemptQuestion", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier });
            
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.QuestionPoints).HasColumnName("QuestionPoints").HasPrecision(7, 2);
        builder.Property(x => x.QuestionSequence).HasColumnName("QuestionSequence").IsRequired();
        builder.Property(x => x.QuestionText).HasColumnName("QuestionText").IsUnicode(true);
        builder.Property(x => x.AnswerOptionKey).HasColumnName("AnswerOptionKey");
        builder.Property(x => x.AnswerOptionSequence).HasColumnName("AnswerOptionSequence");
        builder.Property(x => x.AnswerPoints).HasColumnName("AnswerPoints").HasPrecision(7, 2);
        builder.Property(x => x.CompetencyItemLabel).HasColumnName("CompetencyItemLabel").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.CompetencyItemCode).HasColumnName("CompetencyItemCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CompetencyItemTitle).HasColumnName("CompetencyItemTitle").IsUnicode(false).HasMaxLength(300);
        builder.Property(x => x.CompetencyItemIdentifier).HasColumnName("CompetencyItemIdentifier");
        builder.Property(x => x.CompetencyAreaLabel).HasColumnName("CompetencyAreaLabel").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.CompetencyAreaCode).HasColumnName("CompetencyAreaCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CompetencyAreaTitle).HasColumnName("CompetencyAreaTitle").IsUnicode(false).HasMaxLength(300);
        builder.Property(x => x.CompetencyAreaIdentifier).HasColumnName("CompetencyAreaIdentifier");
        builder.Property(x => x.QuestionType).HasColumnName("QuestionType").IsRequired().IsUnicode(false).HasMaxLength(21);
        builder.Property(x => x.AnswerText).HasColumnName("AnswerText").IsUnicode(true);
        builder.Property(x => x.QuestionCutScore).HasColumnName("QuestionCutScore").HasPrecision(5, 4);
        builder.Property(x => x.QuestionMatchDistractors).HasColumnName("QuestionMatchDistractors").IsUnicode(true);
        builder.Property(x => x.QuestionCalculationMethod).HasColumnName("QuestionCalculationMethod").IsRequired().IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");
        builder.Property(x => x.ParentQuestionIdentifier).HasColumnName("ParentQuestionIdentifier");
        builder.Property(x => x.PinLimit).HasColumnName("PinLimit");
        builder.Property(x => x.HotspotImage).HasColumnName("HotspotImage").IsUnicode(false).HasMaxLength(512);
        builder.Property(x => x.ShowShapes).HasColumnName("ShowShapes");
        builder.Property(x => x.AnswerTimeLimit).HasColumnName("AnswerTimeLimit");
        builder.Property(x => x.AnswerAttemptLimit).HasColumnName("AnswerAttemptLimit");
        builder.Property(x => x.AnswerRequestAttempt).HasColumnName("AnswerRequestAttempt");
        builder.Property(x => x.AnswerFileIdentifier).HasColumnName("AnswerFileIdentifier");
        builder.Property(x => x.AnswerSolutionIdentifier).HasColumnName("AnswerSolutionIdentifier");
        builder.Property(x => x.QuestionTopLabel).HasColumnName("QuestionTopLabel").IsUnicode(true);
        builder.Property(x => x.QuestionBottomLabel).HasColumnName("QuestionBottomLabel").IsUnicode(true);
        builder.Property(x => x.SectionIndex).HasColumnName("SectionIndex");
        builder.Property(x => x.AnswerSubmitAttempt).HasColumnName("AnswerSubmitAttempt");
        builder.Property(x => x.QuestionNumber).HasColumnName("QuestionNumber");
        builder.Property(x => x.RubricRatingPoints).HasColumnName("RubricRatingPoints").IsUnicode(false).HasMaxLength(512);

    }
}