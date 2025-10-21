using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardHierarchyConfiguration : EntityTypeConfiguration<StandardHierarchy>
    {
        public StandardHierarchyConfiguration() : this("standards") { }

        public StandardHierarchyConfiguration(string schema)
        {
            ToTable(schema + ".StandardHierarchy");
            HasKey(x => x.StandardIdentifier);
        }
    }
}
