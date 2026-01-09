using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class QActivityCompetencyConfiguration : EntityTypeConfiguration<QActivityCompetency>
    {
        public QActivityCompetencyConfiguration() : this("courses") { }

        public QActivityCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".QActivityCompetency");
            HasKey(x => new { x.ActivityIdentifier, x.CompetencyStandardIdentifier });

            HasRequired(a => a.Activity).WithMany(b => b.Competencies).HasForeignKey(a => a.ActivityIdentifier).WillCascadeOnDelete(false);
        }
    }
}
