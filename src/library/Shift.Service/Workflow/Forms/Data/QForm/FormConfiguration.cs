using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class FormConfiguration : IEntityTypeConfiguration<FormEntity>
{
    public void Configure(EntityTypeBuilder<FormEntity> builder) 
    {
        builder.ToTable("QSurveyForm", "surveys");
        builder.HasKey(x => new { x.SurveyFormIdentifier });
            
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.SurveyFormIdentifier).HasColumnName("SurveyFormIdentifier").IsRequired();
        builder.Property(x => x.SurveyFormStatus).HasColumnName("SurveyFormStatus").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.SurveyFormLanguage).HasColumnName("SurveyFormLanguage").IsUnicode(false).HasMaxLength(2);
        builder.Property(x => x.SurveyFormLanguageTranslations).HasColumnName("SurveyFormLanguageTranslations").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.SurveyFormName).HasColumnName("SurveyFormName").IsRequired().IsUnicode(false).HasMaxLength(256);
        builder.Property(x => x.UserFeedback).HasColumnName("UserFeedback").IsRequired().IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.RequireUserAuthentication).HasColumnName("RequireUserAuthentication").IsRequired();
        builder.Property(x => x.RequireUserIdentification).HasColumnName("RequireUserIdentification").IsRequired();
        builder.Property(x => x.AssetNumber).HasColumnName("AssetNumber").IsRequired();
        builder.Property(x => x.ResponseLimitPerUser).HasColumnName("ResponseLimitPerUser");
        builder.Property(x => x.SurveyFormOpened).HasColumnName("SurveyFormOpened");
        builder.Property(x => x.SurveyFormClosed).HasColumnName("SurveyFormClosed");
        builder.Property(x => x.SurveyMessageInvitation).HasColumnName("SurveyMessageInvitation");
        builder.Property(x => x.SurveyMessageResponseStarted).HasColumnName("SurveyMessageResponseStarted");
        builder.Property(x => x.SurveyMessageResponseCompleted).HasColumnName("SurveyMessageResponseCompleted");
        builder.Property(x => x.SurveyMessageResponseConfirmed).HasColumnName("SurveyMessageResponseConfirmed");
        builder.Property(x => x.SurveyFormLocked).HasColumnName("SurveyFormLocked");
        builder.Property(x => x.SurveyFormDurationMinutes).HasColumnName("SurveyFormDurationMinutes");
        builder.Property(x => x.SurveyFormHook).HasColumnName("SurveyFormHook").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.PageCount).HasColumnName("PageCount").IsRequired();
        builder.Property(x => x.QuestionCount).HasColumnName("QuestionCount").IsRequired();
        builder.Property(x => x.BranchCount).HasColumnName("BranchCount").IsRequired();
        builder.Property(x => x.ConditionCount).HasColumnName("ConditionCount").IsRequired();
        builder.Property(x => x.EnableUserConfidentiality).HasColumnName("EnableUserConfidentiality").IsRequired();
        builder.Property(x => x.DisplaySummaryChart).HasColumnName("DisplaySummaryChart").IsRequired();
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime").IsRequired();
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsRequired();
        builder.Property(x => x.HasWorkflowConfiguration).HasColumnName("HasWorkflowConfiguration").IsRequired();

    }
}