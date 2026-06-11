using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class SubmissionConfiguration : IEntityTypeConfiguration<SubmissionEntity>
{
    public void Configure(EntityTypeBuilder<SubmissionEntity> builder) 
    {
        builder.ToTable("QResponseSession", "surveys");
        builder.HasKey(x => new { x.ResponseSessionIdentifier });
            
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.SurveyFormIdentifier).HasColumnName("SurveyFormIdentifier").IsRequired();
        builder.Property(x => x.ResponseSessionIdentifier).HasColumnName("ResponseSessionIdentifier").IsRequired();
        builder.Property(x => x.ResponseSessionStatus).HasColumnName("ResponseSessionStatus").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.RespondentUserIdentifier).HasColumnName("RespondentUserIdentifier").IsRequired();
        builder.Property(x => x.RespondentLanguage).HasColumnName("RespondentLanguage").IsUnicode(false).HasMaxLength(2);
        builder.Property(x => x.ResponseIsLocked).HasColumnName("ResponseIsLocked").IsRequired();
        builder.Property(x => x.ResponseSessionCreated).HasColumnName("ResponseSessionCreated");
        builder.Property(x => x.ResponseSessionStarted).HasColumnName("ResponseSessionStarted");
        builder.Property(x => x.ResponseSessionCompleted).HasColumnName("ResponseSessionCompleted");
        builder.Property(x => x.GroupIdentifier).HasColumnName("GroupIdentifier");
        builder.Property(x => x.PeriodIdentifier).HasColumnName("PeriodIdentifier");
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime").IsRequired();
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsRequired();
        builder.Property(x => x.LastAnsweredQuestionIdentifier).HasColumnName("LastAnsweredQuestionIdentifier");

    }
}