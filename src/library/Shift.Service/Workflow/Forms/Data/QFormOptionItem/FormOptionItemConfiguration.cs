using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class FormOptionItemConfiguration : IEntityTypeConfiguration<FormOptionItemEntity>
{
    public void Configure(EntityTypeBuilder<FormOptionItemEntity> builder) 
    {
        builder.ToTable("QSurveyOptionItem", "surveys");
        builder.HasKey(x => new { x.SurveyOptionItemIdentifier });
            
        builder.Property(x => x.SurveyOptionListIdentifier).HasColumnName("SurveyOptionListIdentifier").IsRequired();
        builder.Property(x => x.SurveyOptionItemIdentifier).HasColumnName("SurveyOptionItemIdentifier").IsRequired();
        builder.Property(x => x.SurveyOptionItemSequence).HasColumnName("SurveyOptionItemSequence").IsRequired();
        builder.Property(x => x.BranchToQuestionIdentifier).HasColumnName("BranchToQuestionIdentifier");
        builder.Property(x => x.SurveyOptionItemCategory).HasColumnName("SurveyOptionItemCategory").IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.SurveyOptionItemPoints).HasColumnName("SurveyOptionItemPoints").HasPrecision(7, 2);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}