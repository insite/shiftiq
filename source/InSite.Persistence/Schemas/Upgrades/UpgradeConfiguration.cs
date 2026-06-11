using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class UpgradeConfiguration : EntityTypeConfiguration<Upgrade>
    {
        public UpgradeConfiguration() : this("utilities") { }

        public UpgradeConfiguration(string schema)
        {
            ToTable(schema + ".Upgrade");
            HasKey(x => x.UpgradeIdentifier);

            Property(x => x.ScriptName).IsRequired().IsUnicode(false).HasMaxLength(128);
            Property(x => x.UtcUpgraded).IsRequired();
        }
    }
}