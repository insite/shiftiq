using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class QAttemptSolutionConfiguration : EntityTypeConfiguration<QAttemptSolution>
    {
        public QAttemptSolutionConfiguration() : this("assessments") { }

        public QAttemptSolutionConfiguration(string schema)
        {
            ToTable(schema + ".QAttemptSolution");
            HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier, x.SolutionIdentifier });

            Property(x => x.SolutionOptionsOrder).IsRequired().HasMaxLength(512).IsUnicode(false);
            Property(x => x.SolutionPoints).HasPrecision(7, 2);
            Property(x => x.SolutionCutScore).HasPrecision(5, 4);

            HasRequired(a => a.Attempt).WithMany(b => b.Solutions).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);
        }
    }
}
