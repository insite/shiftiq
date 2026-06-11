using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class PermissionListConfiguration : EntityTypeConfiguration<PermissionList>
    {
        public PermissionListConfiguration() : this("custom_cmds") { }

        public PermissionListConfiguration(string schema)
        {
            ToTable(schema + ".PermissionList");
            HasKey(x => new { x.GroupIdentifier });
        
            Property(x => x.GroupAbbreviation).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
        }
    }
}
