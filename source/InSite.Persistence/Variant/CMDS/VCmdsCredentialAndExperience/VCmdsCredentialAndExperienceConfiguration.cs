using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsCredentialAndExperienceConfiguration : EntityTypeConfiguration<VCmdsCredentialAndExperience>
    {
        public VCmdsCredentialAndExperienceConfiguration() : this("custom_cmds") { }

        public VCmdsCredentialAndExperienceConfiguration(string schema)
        {
            ToTable(schema + ".VCmdsCredentialAndExperience");
            HasKey(x => new { x.ExperienceIdentifier, x.AchievementIdentifier, x.UserIdentifier });

            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.AchievementLabel).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AchievementOrganizationIdentifier).IsOptional();
            Property(x => x.AchievementTitle).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.AuthorityLocation).IsOptional().IsUnicode(false).HasMaxLength(388);
            Property(x => x.AuthorityName).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.AuthorityReference).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.CredentialDescription).IsOptional().IsUnicode(false);
            Property(x => x.CredentialExpired).IsOptional();
            Property(x => x.CredentialGranted).IsOptional();
            Property(x => x.CredentialIsMandatory).IsRequired();
            Property(x => x.CredentialIsPlanned).IsOptional();
            Property(x => x.CredentialIsTimeSensitive).IsOptional();
            Property(x => x.ExperienceIdentifier).IsRequired();
            Property(x => x.UserFullName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
