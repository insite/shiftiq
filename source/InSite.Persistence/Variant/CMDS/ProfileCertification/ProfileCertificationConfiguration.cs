using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ProfileCertificationConfiguration : EntityTypeConfiguration<ProfileCertification>
    {
        public ProfileCertificationConfiguration() : this("custom_cmds") { }

        public ProfileCertificationConfiguration(string schema)
        {
            ToTable(schema + ".ProfileCertification");
            HasKey(x => new { x.ProfileStandardIdentifier, x.UserIdentifier });
        
            Property(x => x.AuthorityName).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.DateGranted).IsOptional();
            Property(x => x.DateRequested).IsOptional();
            Property(x => x.DateSubmitted).IsOptional();
            Property(x => x.ProfileStandardIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
