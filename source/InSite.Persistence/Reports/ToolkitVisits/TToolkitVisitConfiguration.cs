using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class TToolkitVisitConfiguration : EntityTypeConfiguration<TToolkitVisit>
    {
        public TToolkitVisitConfiguration() : this("reports") { }

        public TToolkitVisitConfiguration(string schema)
        {
            ToTable(schema + ".TToolkitVisit");
            HasKey(x => new { x.ToolkitVisitIdentifier });
        }
    }
}
