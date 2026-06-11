using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QExperienceCompetencyConfiguration: EntityTypeConfiguration<QExperienceCompetency>
    {
        public QExperienceCompetencyConfiguration() : this("records") { }

        public QExperienceCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".QExperienceCompetency");
            HasKey(x => new { x.ExperienceIdentifier, x.CompetencyStandardIdentifier });

            HasRequired(a => a.Experience).WithMany(b => b.Competencies).HasForeignKey(c => c.ExperienceIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Competency).WithMany(b => b.ExperienceCompetencies).HasForeignKey(c => c.CompetencyStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
