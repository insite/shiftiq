using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QAreaRequirementConfiguration : EntityTypeConfiguration<QAreaRequirement>
    {
        public QAreaRequirementConfiguration() : this("records") { }

        public QAreaRequirementConfiguration(string schema)
        {
            ToTable("QAreaRequirement", schema);
            HasKey(x => new { x.JournalSetupIdentifier, x.AreaStandardIdentifier });

            Property(x => x.AreaHours).HasPrecision(20, 2);

            HasRequired(a => a.JournalSetup).WithMany(b => b.AreaRequirements).HasForeignKey(c => c.JournalSetupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.AreaStandard).WithMany(b => b.JournalSetupAreaRequirements).HasForeignKey(c => c.AreaStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
