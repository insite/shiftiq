using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttemptEntity>
{
    public void Configure(EntityTypeBuilder<QuizAttemptEntity> builder) 
    {
        builder.ToTable("TQuizAttempt", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier });
            
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.QuizIdentifier).HasColumnName("QuizIdentifier").IsRequired();
        builder.Property(x => x.LearnerIdentifier).HasColumnName("LearnerIdentifier").IsRequired();
        builder.Property(x => x.AttemptCreated).HasColumnName("AttemptCreated").IsRequired();
        builder.Property(x => x.AttemptStarted).HasColumnName("AttemptStarted");
        builder.Property(x => x.AttemptCompleted).HasColumnName("AttemptCompleted");
        builder.Property(x => x.QuizGradebookIdentifier).HasColumnName("QuizGradebookIdentifier").IsRequired();
        builder.Property(x => x.QuizType).HasColumnName("QuizType").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.QuizName).HasColumnName("QuizName").IsRequired().IsUnicode(true).HasMaxLength(100);
        builder.Property(x => x.QuizData).HasColumnName("QuizData").IsUnicode(true).HasMaxLength(4000);
        builder.Property(x => x.QuizTimeLimit).HasColumnName("QuizTimeLimit").IsRequired();
        builder.Property(x => x.QuizPassingAccuracy).HasColumnName("QuizPassingAccuracy").IsRequired().HasPrecision(3, 2);
        builder.Property(x => x.QuizPassingWpm).HasColumnName("QuizPassingWpm").IsRequired();
        builder.Property(x => x.QuizPassingKph).HasColumnName("QuizPassingKph").IsRequired();
        builder.Property(x => x.AttemptData).HasColumnName("AttemptData").IsUnicode(true).HasMaxLength(4000);
        builder.Property(x => x.AttemptIsPassing).HasColumnName("AttemptIsPassing");
        builder.Property(x => x.AttemptScore).HasColumnName("AttemptScore").HasPrecision(9, 8);
        builder.Property(x => x.AttemptDuration).HasColumnName("AttemptDuration").HasPrecision(9, 3);
        builder.Property(x => x.AttemptMistakes).HasColumnName("AttemptMistakes");
        builder.Property(x => x.AttemptAccuracy).HasColumnName("AttemptAccuracy").HasPrecision(5, 4);
        builder.Property(x => x.AttemptCharsPerMin).HasColumnName("AttemptCharsPerMin");
        builder.Property(x => x.AttemptWordsPerMin).HasColumnName("AttemptWordsPerMin");
        builder.Property(x => x.AttemptKeystrokesPerHour).HasColumnName("AttemptKeystrokesPerHour");
        builder.Property(x => x.AttemptSpeed).HasColumnName("AttemptSpeed").HasPrecision(9, 2);

    }
}