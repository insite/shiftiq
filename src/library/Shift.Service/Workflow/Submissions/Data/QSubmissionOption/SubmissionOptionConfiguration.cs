using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class SubmissionOptionConfiguration : IEntityTypeConfiguration<SubmissionOptionEntity>
{
    public void Configure(EntityTypeBuilder<SubmissionOptionEntity> builder) 
    {
        builder.ToTable("QResponseOption", "surveys");
        builder.HasKey(x => new { x.ResponseSessionIdentifier, x.SurveyOptionIdentifier });
            
        builder.Property(x => x.ResponseSessionIdentifier).HasColumnName("ResponseSessionIdentifier").IsRequired();
        builder.Property(x => x.SurveyOptionIdentifier).HasColumnName("SurveyOptionIdentifier").IsRequired();
        builder.Property(x => x.ResponseOptionIsSelected).HasColumnName("ResponseOptionIsSelected").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");
        builder.Property(x => x.SurveyQuestionIdentifier).HasColumnName("SurveyQuestionIdentifier").IsRequired();
        builder.Property(x => x.OptionSequence).HasColumnName("OptionSequence").IsRequired();

    }
}