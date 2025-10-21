using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TGroupSettingConfiguration : EntityTypeConfiguration<TGroupSetting>
    {
        public TGroupSettingConfiguration() : this("contacts") { }

        public TGroupSettingConfiguration(string schema)
        {
            ToTable(schema + ".TGroupSetting");
            HasKey(x => new { x.SettingIdentifier, x.GroupIdentifier });

            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.SettingValue).IsRequired();
            Property(x => x.SettingName).IsRequired().HasMaxLength(100);
            Property(x => x.SettingIdentifier).IsRequired();
        }
    }
}
