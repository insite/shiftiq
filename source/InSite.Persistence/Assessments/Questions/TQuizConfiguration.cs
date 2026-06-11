using System.Data.Entity.ModelConfiguration;

using InSite.Application.Quizzes.Read;

namespace InSite.Persistence
{
    public class TQuizConfiguration : EntityTypeConfiguration<TQuiz>
    {
        public TQuizConfiguration() : this("assessments") { }

        public TQuizConfiguration(string schema)
        {
            ToTable(schema + ".TQuiz");
            HasKey(x => x.QuizIdentifier);

            Property(x => x.QuizType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.QuizName).IsRequired().HasMaxLength(100);
            Property(x => x.QuizData).IsRequired().HasMaxLength(TQuiz.MaxQuizDataLength);

            Property(x => x.PassingAccuracy).HasPrecision(3, 2);

            HasRequired(a => a.Gradebook).WithMany(b => b.Quizzes).HasForeignKey(a => a.GradebookIdentifier).WillCascadeOnDelete(false);
        }
    }
}
