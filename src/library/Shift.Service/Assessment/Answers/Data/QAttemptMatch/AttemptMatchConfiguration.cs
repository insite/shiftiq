using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class AttemptMatchConfiguration : IEntityTypeConfiguration<AttemptMatchEntity>
{
    public void Configure(EntityTypeBuilder<AttemptMatchEntity> builder) 
    {
        builder.ToTable("QAttemptMatch", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier, x.MatchSequence, x.QuestionIdentifier });
            
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.QuestionSequence).HasColumnName("QuestionSequence").IsRequired();
        builder.Property(x => x.MatchSequence).HasColumnName("MatchSequence").IsRequired();
        builder.Property(x => x.MatchLeftText).HasColumnName("MatchLeftText").IsRequired().IsUnicode(true);
        builder.Property(x => x.MatchRightText).HasColumnName("MatchRightText").IsRequired().IsUnicode(true);
        builder.Property(x => x.AnswerText).HasColumnName("AnswerText").IsUnicode(true);
        builder.Property(x => x.MatchPoints).HasColumnName("MatchPoints").IsRequired().HasPrecision(7, 2);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}