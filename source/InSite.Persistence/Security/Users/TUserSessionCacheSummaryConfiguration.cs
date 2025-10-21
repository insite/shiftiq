using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TUserSessionCacheSummaryConfiguration : EntityTypeConfiguration<TUserSessionCacheSummary>
    {
        public TUserSessionCacheSummaryConfiguration() : this("accounts") { }

        public TUserSessionCacheSummaryConfiguration(string schema)
        {
            ToTable(schema + ".TUserSessionCacheSummary");
            HasKey(x => new { x.UserIdentifier, x.SessionStarted });
        
            Property(x => x.CompanyName).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.CompanyTitle).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.SessionStarted).IsRequired();
            Property(x => x.OrganizationCode).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}