using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class FormOptionListConfiguration : IEntityTypeConfiguration<FormOptionListEntity>
{
    public void Configure(EntityTypeBuilder<FormOptionListEntity> builder) 
    {
        builder.ToTable("QSurveyOptionList", "surveys");
        builder.HasKey(x => new { x.SurveyOptionListIdentifier });
            
        builder.Property(x => x.SurveyOptionListIdentifier).HasColumnName("SurveyOptionListIdentifier").IsRequired();
        builder.Property(x => x.SurveyOptionListSequence).HasColumnName("SurveyOptionListSequence").IsRequired();
        builder.Property(x => x.SurveyQuestionIdentifier).HasColumnName("SurveyQuestionIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}