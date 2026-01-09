using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class AttemptSectionConfiguration : IEntityTypeConfiguration<AttemptSectionEntity>
{
    public void Configure(EntityTypeBuilder<AttemptSectionEntity> builder) 
    {
        builder.ToTable("QAttemptSection", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier, x.SectionIndex });
            
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.SectionIndex).HasColumnName("SectionIndex").IsRequired();
        builder.Property(x => x.SectionIdentifier).HasColumnName("SectionIdentifier");
        builder.Property(x => x.SectionStarted).HasColumnName("SectionStarted");
        builder.Property(x => x.SectionCompleted).HasColumnName("SectionCompleted");
        builder.Property(x => x.SectionDuration).HasColumnName("SectionDuration");
        builder.Property(x => x.TimeLimit).HasColumnName("TimeLimit");
        builder.Property(x => x.TimerType).HasColumnName("TimerType").IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.IsBreakTimer).HasColumnName("IsBreakTimer").IsRequired();
        builder.Property(x => x.ShowWarningNextTab).HasColumnName("ShowWarningNextTab").IsRequired();

    }
}