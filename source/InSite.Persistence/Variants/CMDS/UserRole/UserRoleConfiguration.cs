using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserRoleConfiguration : EntityTypeConfiguration<UserRole>
    {
        public UserRoleConfiguration() : this("custom_cmds") { }

        public UserRoleConfiguration(string schema)
        {
            ToTable(schema + ".UserRole");
            HasKey(x => new { x.GroupIdentifier, x.UserIdentifier });
        
            Property(x => x.GroupAbbreviation).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.UserEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
