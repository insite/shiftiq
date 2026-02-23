using System.Data.Entity.ModelConfiguration;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;

namespace InSite.Persistence
{
    public class TQuizAttemptConfiguration : EntityTypeConfiguration<TQuizAttempt>
    {
        public TQuizAttemptConfiguration() : this("assessments") { }

        public TQuizAttemptConfiguration(string schema)
        {
            ToTable(schema + ".TQuizAttempt");
            HasKey(x => x.AttemptIdentifier);

            Property(x => x.QuizType).IsRequired().IsUnicode(false).HasMaxLength(20);

            Property(x => x.QuizType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.QuizName).IsRequired().HasMaxLength(100);
            Property(x => x.QuizData).HasMaxLength(TQuiz.MaxQuizDataLength);
            Property(x => x.QuizPassingAccuracy).HasPrecision(3, 2);

            Property(x => x.AttemptData).HasMaxLength(TQuizAttempt.MaxAttemptDataLength);
            Property(x => x.AttemptScore).HasPrecision(9, 8);
            Property(x => x.AttemptDuration).HasPrecision(9, 3);
            Property(x => x.AttemptAccuracy).HasPrecision(5, 4);
            Property(x => x.AttemptSpeed).HasPrecision(9, 2);

            HasRequired(a => a.Quiz).WithMany(b => b.Attempts).HasForeignKey(a => a.QuizIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.LearnerPerson).WithMany(b => b.LearnerQuizAttempts).HasForeignKey(c => new { c.OrganizationIdentifier, c.LearnerIdentifier }).WillCascadeOnDelete(false);
            HasRequired(a => a.LearnerUser).WithMany(b => b.LearnerQuizAttempts).HasForeignKey(c => c.LearnerIdentifier).WillCascadeOnDelete(false);
        }
    }
}
