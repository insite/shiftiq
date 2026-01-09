using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class QuizConfiguration : IEntityTypeConfiguration<QuizEntity>
{
    public void Configure(EntityTypeBuilder<QuizEntity> builder) 
    {
        builder.ToTable("TQuiz", "assessments");
        builder.HasKey(x => new { x.QuizIdentifier });
            
        builder.Property(x => x.QuizIdentifier).HasColumnName("QuizIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.GradebookIdentifier).HasColumnName("GradebookIdentifier").IsRequired();
        builder.Property(x => x.QuizType).HasColumnName("QuizType").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.QuizName).HasColumnName("QuizName").IsRequired().IsUnicode(true).HasMaxLength(100);
        builder.Property(x => x.QuizData).HasColumnName("QuizData").IsRequired().IsUnicode(true).HasMaxLength(4000);
        builder.Property(x => x.TimeLimit).HasColumnName("TimeLimit").IsRequired();
        builder.Property(x => x.AttemptLimit).HasColumnName("AttemptLimit").IsRequired();
        builder.Property(x => x.PassingScore).HasColumnName("PassingScore").HasPrecision(5, 4);
        builder.Property(x => x.MaximumPoints).HasColumnName("MaximumPoints").HasPrecision(7, 2);
        builder.Property(x => x.PassingPoints).HasColumnName("PassingPoints").HasPrecision(7, 2);
        builder.Property(x => x.PassingAccuracy).HasColumnName("PassingAccuracy").IsRequired().HasPrecision(3, 2);
        builder.Property(x => x.PassingWpm).HasColumnName("PassingWpm").IsRequired();
        builder.Property(x => x.PassingKph).HasColumnName("PassingKph").IsRequired();

    }
}