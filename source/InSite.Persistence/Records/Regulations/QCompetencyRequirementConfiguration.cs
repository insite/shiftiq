using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QCompetencyRequirementConfiguration : EntityTypeConfiguration<QCompetencyRequirement>
    {
        public QCompetencyRequirementConfiguration() : this("records") { }

        public QCompetencyRequirementConfiguration(string schema)
        {
            ToTable(schema + ".QCompetencyRequirement");
            HasKey(x => new { x.JournalSetupIdentifier, x.CompetencyStandardIdentifier });

            HasRequired(a => a.JournalSetup).WithMany(b => b.CompetencyRequirements).HasForeignKey(c => c.JournalSetupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Competency).WithMany(b => b.CompetencyRequirements).HasForeignKey(c => c.CompetencyStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
