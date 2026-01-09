using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TUserSettingConfiguration : EntityTypeConfiguration<TUserSetting>
    {
        public TUserSettingConfiguration() : this("accounts") { }

        public TUserSettingConfiguration(string schema)
        {
            ToTable(schema + ".TUserSetting");

            HasKey(x => x.SettingIdentifier);

            Property(x => x.Name)
                .HasMaxLength(128)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.ValueType)
                .HasMaxLength(256)
                .IsRequired()
                .IsUnicode(false);

            Property(x => x.ValueJson)
                .IsRequired()
                .IsUnicode(false);
        }
    }
}
