using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankFormConfiguration : IEntityTypeConfiguration<BankFormEntity>
{
    public void Configure(EntityTypeBuilder<BankFormEntity> builder) 
    {
        builder.ToTable("QBankForm", "banks");
        builder.HasKey(x => new { x.FormIdentifier });
            
        builder.Property(x => x.BankIdentifier).HasColumnName("BankIdentifier").IsRequired();
        builder.Property(x => x.FieldCount).HasColumnName("FieldCount").IsRequired();
        builder.Property(x => x.FormIdentifier).HasColumnName("FormIdentifier").IsRequired();
        builder.Property(x => x.FormAsset).HasColumnName("FormAsset").IsRequired();
        builder.Property(x => x.FormName).HasColumnName("FormName").IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.FormPublicationStatus).HasColumnName("FormPublicationStatus").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.FormTitle).HasColumnName("FormTitle").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.FormType).HasColumnName("FormType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.FormVersion).HasColumnName("FormVersion").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.SpecQuestionLimit).HasColumnName("SpecQuestionLimit").IsRequired();
        builder.Property(x => x.SectionCount).HasColumnName("SectionCount").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.SpecIdentifier).HasColumnName("SpecIdentifier").IsRequired();
        builder.Property(x => x.AttemptStartedCount).HasColumnName("AttemptStartedCount").IsRequired();
        builder.Property(x => x.AttemptPassedCount).HasColumnName("AttemptPassedCount").IsRequired();
        builder.Property(x => x.FormCode).HasColumnName("FormCode").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.FormTimeLimit).HasColumnName("FormTimeLimit");
        builder.Property(x => x.FormPassingScore).HasColumnName("FormPassingScore").HasPrecision(3, 2);
        builder.Property(x => x.BankLevelType).HasColumnName("BankLevelType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.FormSource).HasColumnName("FormSource").IsUnicode(false).HasMaxLength(80);
        builder.Property(x => x.FormAssetVersion).HasColumnName("FormAssetVersion").IsRequired();
        builder.Property(x => x.FormFirstPublished).HasColumnName("FormFirstPublished");
        builder.Property(x => x.FormOrigin).HasColumnName("FormOrigin").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.FormHook).HasColumnName("FormHook").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.FormSummary).HasColumnName("FormSummary").IsUnicode(true);
        builder.Property(x => x.FormIntroduction).HasColumnName("FormIntroduction").IsUnicode(true);
        builder.Property(x => x.FormMaterialsForParticipation).HasColumnName("FormMaterialsForParticipation").IsUnicode(true);
        builder.Property(x => x.FormMaterialsForDistribution).HasColumnName("FormMaterialsForDistribution").IsUnicode(true);
        builder.Property(x => x.FormInstructionsForOnline).HasColumnName("FormInstructionsForOnline").IsUnicode(true);
        builder.Property(x => x.FormInstructionsForPaper).HasColumnName("FormInstructionsForPaper").IsUnicode(true);
        builder.Property(x => x.FormHasDiagrams).HasColumnName("FormHasDiagrams").IsRequired();
        builder.Property(x => x.FormHasReferenceMaterials).HasColumnName("FormHasReferenceMaterials").IsUnicode(false).HasMaxLength(21);
        builder.Property(x => x.FormAttemptLimit).HasColumnName("FormAttemptLimit").IsRequired();
        builder.Property(x => x.AttemptSubmittedCount).HasColumnName("AttemptSubmittedCount").IsRequired();
        builder.Property(x => x.AttemptGradedCount).HasColumnName("AttemptGradedCount").IsRequired();
        builder.Property(x => x.FormThirdPartyAssessmentIsEnabled).HasColumnName("FormThirdPartyAssessmentIsEnabled").IsRequired();
        builder.Property(x => x.FormClassificationInstrument).HasColumnName("FormClassificationInstrument").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GradebookIdentifier).HasColumnName("GradebookIdentifier");
        builder.Property(x => x.WhenAttemptStartedNotifyAdminMessageIdentifier).HasColumnName("WhenAttemptStartedNotifyAdminMessageIdentifier");
        builder.Property(x => x.WhenAttemptCompletedNotifyAdminMessageIdentifier).HasColumnName("WhenAttemptCompletedNotifyAdminMessageIdentifier");

    }
}