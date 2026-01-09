using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class FormQuestionConfiguration : IEntityTypeConfiguration<FormQuestionEntity>
{
    public void Configure(EntityTypeBuilder<FormQuestionEntity> builder) 
    {
        builder.ToTable("QSurveyQuestion", "surveys");
        builder.HasKey(x => new { x.SurveyQuestionIdentifier });
            
        builder.Property(x => x.SurveyQuestionIdentifier).HasColumnName("SurveyQuestionIdentifier").IsRequired();
        builder.Property(x => x.SurveyFormIdentifier).HasColumnName("SurveyFormIdentifier").IsRequired();
        builder.Property(x => x.SurveyQuestionType).HasColumnName("SurveyQuestionType").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.SurveyQuestionCode).HasColumnName("SurveyQuestionCode").IsUnicode(false).HasMaxLength(4);
        builder.Property(x => x.SurveyQuestionIndicator).HasColumnName("SurveyQuestionIndicator").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.SurveyQuestionSequence).HasColumnName("SurveyQuestionSequence").IsRequired();
        builder.Property(x => x.SurveyQuestionIsRequired).HasColumnName("SurveyQuestionIsRequired").IsRequired();
        builder.Property(x => x.SurveyQuestionListEnableBranch).HasColumnName("SurveyQuestionListEnableBranch").IsRequired();
        builder.Property(x => x.SurveyQuestionListEnableOtherText).HasColumnName("SurveyQuestionListEnableOtherText").IsRequired();
        builder.Property(x => x.SurveyQuestionListEnableRandomization).HasColumnName("SurveyQuestionListEnableRandomization").IsRequired();
        builder.Property(x => x.SurveyQuestionNumberEnableStatistics).HasColumnName("SurveyQuestionNumberEnableStatistics").IsRequired();
        builder.Property(x => x.SurveyQuestionTextCharacterLimit).HasColumnName("SurveyQuestionTextCharacterLimit");
        builder.Property(x => x.SurveyQuestionTextLineCount).HasColumnName("SurveyQuestionTextLineCount");
        builder.Property(x => x.SurveyQuestionIsNested).HasColumnName("SurveyQuestionIsNested").IsRequired();
        builder.Property(x => x.SurveyQuestionSource).HasColumnName("SurveyQuestionSource").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.SurveyQuestionAttribute).HasColumnName("SurveyQuestionAttribute").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");
        builder.Property(x => x.SurveyQuestionListEnableGroupMembership).HasColumnName("SurveyQuestionListEnableGroupMembership").IsRequired();

    }
}