using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class SubmissionAnswerConfiguration : IEntityTypeConfiguration<SubmissionAnswerEntity>
{
    public void Configure(EntityTypeBuilder<SubmissionAnswerEntity> builder) 
    {
        builder.ToTable("QResponseAnswer", "surveys");
        builder.HasKey(x => new { x.ResponseSessionIdentifier, x.SurveyQuestionIdentifier });
            
        builder.Property(x => x.ResponseSessionIdentifier).HasColumnName("ResponseSessionIdentifier").IsRequired();
        builder.Property(x => x.SurveyQuestionIdentifier).HasColumnName("SurveyQuestionIdentifier").IsRequired();
        builder.Property(x => x.RespondentUserIdentifier).HasColumnName("RespondentUserIdentifier").IsRequired();
        builder.Property(x => x.ResponseAnswerText).HasColumnName("ResponseAnswerText").IsUnicode(true);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}