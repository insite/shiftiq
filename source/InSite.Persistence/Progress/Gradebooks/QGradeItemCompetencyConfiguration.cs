using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QGradeItemCompetencyConfiguration : EntityTypeConfiguration<QGradeItemCompetency>
    {
        public QGradeItemCompetencyConfiguration() : this("records") { }

        public QGradeItemCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".QGradeItemCompetency");
            HasKey(x => new { x.GradebookIdentifier, x.GradeItemIdentifier, x.CompetencyIdentifier });

            HasRequired(a => a.Gradebook).WithMany(b => b.Competencies).HasForeignKey(c => c.GradebookIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.GradeItem).WithMany(b => b.Standards).HasForeignKey(c => c.GradeItemIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Standard).WithMany(b => b.GradeItemCompetencies).HasForeignKey(c => c.CompetencyIdentifier).WillCascadeOnDelete(false);

            Property(x => x.CompetencyIdentifier).IsRequired();
            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.GradeItemIdentifier).IsRequired();
        }
    }
}
