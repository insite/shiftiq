using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class FormConditionConfiguration : IEntityTypeConfiguration<FormConditionEntity>
{
    public void Configure(EntityTypeBuilder<FormConditionEntity> builder) 
    {
        builder.ToTable("QSurveyCondition", "surveys");
        builder.HasKey(x => new { x.MaskedSurveyQuestionIdentifier, x.MaskingSurveyOptionItemIdentifier });
            
        builder.Property(x => x.MaskingSurveyOptionItemIdentifier).HasColumnName("MaskingSurveyOptionItemIdentifier").IsRequired();
        builder.Property(x => x.MaskedSurveyQuestionIdentifier).HasColumnName("MaskedSurveyQuestionIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}