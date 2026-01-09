using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class AttemptOptionConfiguration : IEntityTypeConfiguration<AttemptOptionEntity>
{
    public void Configure(EntityTypeBuilder<AttemptOptionEntity> builder) 
    {
        builder.ToTable("QAttemptOption", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier, x.OptionKey, x.QuestionIdentifier });
            
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.QuestionSequence).HasColumnName("QuestionSequence").IsRequired();
        builder.Property(x => x.OptionKey).HasColumnName("OptionKey").IsRequired();
        builder.Property(x => x.OptionPoints).HasColumnName("OptionPoints").HasPrecision(7, 2);
        builder.Property(x => x.OptionSequence).HasColumnName("OptionSequence").IsRequired();
        builder.Property(x => x.OptionText).HasColumnName("OptionText").IsUnicode(true);
        builder.Property(x => x.AnswerIsSelected).HasColumnName("AnswerIsSelected");
        builder.Property(x => x.OptionCutScore).HasColumnName("OptionCutScore").HasPrecision(5, 4);
        builder.Property(x => x.CompetencyItemIdentifier).HasColumnName("CompetencyItemIdentifier");
        builder.Property(x => x.OptionIsTrue).HasColumnName("OptionIsTrue");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");
        builder.Property(x => x.OptionShape).HasColumnName("OptionShape").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.OptionAnswerSequence).HasColumnName("OptionAnswerSequence");
        builder.Property(x => x.CompetencyItemLabel).HasColumnName("CompetencyItemLabel").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.CompetencyItemCode).HasColumnName("CompetencyItemCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CompetencyItemTitle).HasColumnName("CompetencyItemTitle").IsUnicode(false).HasMaxLength(300);
        builder.Property(x => x.CompetencyAreaLabel).HasColumnName("CompetencyAreaLabel").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.CompetencyAreaCode).HasColumnName("CompetencyAreaCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CompetencyAreaTitle).HasColumnName("CompetencyAreaTitle").IsUnicode(false).HasMaxLength(300);
        builder.Property(x => x.CompetencyAreaIdentifier).HasColumnName("CompetencyAreaIdentifier");

    }
}