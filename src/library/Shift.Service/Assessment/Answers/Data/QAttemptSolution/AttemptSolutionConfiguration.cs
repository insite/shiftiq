using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class AttemptSolutionConfiguration : IEntityTypeConfiguration<AttemptSolutionEntity>
{
    public void Configure(EntityTypeBuilder<AttemptSolutionEntity> builder) 
    {
        builder.ToTable("QAttemptSolution", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier, x.SolutionIdentifier });
            
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.QuestionSequence).HasColumnName("QuestionSequence").IsRequired();
        builder.Property(x => x.SolutionIdentifier).HasColumnName("SolutionIdentifier").IsRequired();
        builder.Property(x => x.SolutionSequence).HasColumnName("SolutionSequence").IsRequired();
        builder.Property(x => x.SolutionOptionsOrder).HasColumnName("SolutionOptionsOrder").IsRequired().IsUnicode(false).HasMaxLength(512);
        builder.Property(x => x.SolutionPoints).HasColumnName("SolutionPoints").IsRequired().HasPrecision(7, 2);
        builder.Property(x => x.SolutionCutScore).HasColumnName("SolutionCutScore").HasPrecision(5, 4);
        builder.Property(x => x.AnswerIsMatched).HasColumnName("AnswerIsMatched");

    }
}