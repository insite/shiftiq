using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class AttemptPinConfiguration : IEntityTypeConfiguration<AttemptPinEntity>
{
    public void Configure(EntityTypeBuilder<AttemptPinEntity> builder) 
    {
        builder.ToTable("QAttemptPin", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier, x.PinSequence, x.QuestionIdentifier });
            
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.QuestionSequence).HasColumnName("QuestionSequence").IsRequired();
        builder.Property(x => x.OptionKey).HasColumnName("OptionKey");
        builder.Property(x => x.OptionPoints).HasColumnName("OptionPoints").HasPrecision(7, 2);
        builder.Property(x => x.OptionSequence).HasColumnName("OptionSequence");
        builder.Property(x => x.OptionText).HasColumnName("OptionText").IsUnicode(true);
        builder.Property(x => x.PinSequence).HasColumnName("PinSequence").IsRequired();
        builder.Property(x => x.PinX).HasColumnName("PinX").IsRequired();
        builder.Property(x => x.PinY).HasColumnName("PinY").IsRequired();

    }
}