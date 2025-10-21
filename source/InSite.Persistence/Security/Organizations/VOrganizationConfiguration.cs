using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VOrganizationConfiguration : EntityTypeConfiguration<VOrganization>
    {
        public VOrganizationConfiguration()
        {
            ToTable("VOrganization", "accounts");
            HasKey(x => new { x.OrganizationIdentifier });

            Property(x => x.AccountStatus).IsRequired().IsUnicode(false).HasMaxLength(6);
            Property(x => x.CompanyDomain).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CompanyName).IsUnicode(false).HasMaxLength(50);
            Property(x => x.CompanySize).IsUnicode(false).HasMaxLength(10);
            Property(x => x.CompanySummary).IsUnicode(false);
            Property(x => x.CompanyTitle).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CompanyWebSiteUrl).IsUnicode(false).HasMaxLength(100);
            Property(x => x.CompetencyAutoExpirationMode).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.OrganizationCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.OrganizationData).IsRequired().IsUnicode(false);
            Property(x => x.OrganizationLogoUrl).IsUnicode(false).HasMaxLength(100);
            Property(x => x.StandardContentLabels).IsUnicode(false).HasMaxLength(200);
            Property(x => x.TimeZone).IsUnicode(false).HasMaxLength(32);
        }
    }
}
