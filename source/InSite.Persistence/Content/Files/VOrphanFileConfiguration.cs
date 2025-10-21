using System.Data.Entity.ModelConfiguration;

using InSite.Application.Files.Read;

namespace InSite.Persistence
{
    public class VOrphanFileConfiguration : EntityTypeConfiguration<OrphanFile>
    {
        public VOrphanFileConfiguration() : this("assets") { }

        public VOrphanFileConfiguration(string schema)
        {
            ToTable(schema + ".VOrphanFile");
            HasKey(x => x.FileIdentifier);
        }
    }
}
