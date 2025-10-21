using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserProfileConfiguration : EntityTypeConfiguration<UserProfile>
    {
        public UserProfileConfiguration() : this("custom_cmds") { }

        public UserProfileConfiguration(string schema)
        {
            ToTable(schema + ".UserProfile");
            HasKey(x => new { x.ProfileStandardIdentifier, x.UserIdentifier });
        
            Property(x => x.CurrentStatus).IsOptional().IsUnicode(false).HasMaxLength(22);
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.IsComplianceRequired).IsRequired();
            Property(x => x.IsPrimary).IsRequired();
            Property(x => x.ProfileStandardIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
