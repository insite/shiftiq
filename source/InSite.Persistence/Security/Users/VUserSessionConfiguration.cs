using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VUserSessionConfiguration : EntityTypeConfiguration<VUserSession>
    {
        public VUserSessionConfiguration() : this("accounts") { }

        public VUserSessionConfiguration(string schema)
        {
            ToTable(schema + ".VUserSession");
            HasKey(x => new { x.SessionStarted, x.UserIdentifier });

            Property(x => x.AuthenticationErrorMessage).IsOptional().IsUnicode(false);
            Property(x => x.AuthenticationErrorType).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.SessionCode).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.SessionIsAuthenticated).IsRequired();
            Property(x => x.SessionMinutes).IsOptional();
            Property(x => x.SessionStarted).IsRequired();
            Property(x => x.SessionStopped).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserAgent).IsOptional().IsUnicode(false).HasMaxLength(512);
            Property(x => x.UserBrowser).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserBrowserVersion).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserFirstName).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.UserHostAddress).IsRequired().IsUnicode(false).HasMaxLength(16);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserLanguage).IsRequired().IsUnicode(false).HasMaxLength(5);
            Property(x => x.UserLastName).IsRequired().IsUnicode(false).HasMaxLength(40);
        }
    }
}
