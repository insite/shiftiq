using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ProfileConfiguration : EntityTypeConfiguration<Profile>
    {
        public ProfileConfiguration() : this("custom_cmds") { }

        public ProfileConfiguration(string schema)
        {
            ToTable(schema + ".Profile");
            HasKey(x => new { x.ProfileStandardIdentifier });
        
            Property(x => x.ProfileNumber).IsUnicode(false).HasMaxLength(256);
            Property(x => x.Visibility).IsRequired().IsUnicode(false).HasMaxLength(22);
        }
    }
}
