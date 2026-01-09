using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TUserSessionConfiguration : EntityTypeConfiguration<TUserSession>
    {
        public TUserSessionConfiguration() : this("security") { }

        public TUserSessionConfiguration(string schema)
        {
            ToTable(schema + ".TUserSession");
            HasKey(x => x.SessionIdentifier);

            Property(x => x.AuthenticationErrorMessage).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.AuthenticationErrorType).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.AuthenticationSource).IsOptional().IsUnicode(false).HasMaxLength(19);
            Property(x => x.SessionCode).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.SessionIsAuthenticated).IsOptional();
            Property(x => x.SessionMinutes).IsOptional();
            Property(x => x.SessionStarted).IsRequired();
            Property(x => x.SessionStopped).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserAgent).IsOptional().IsUnicode(false).HasMaxLength(512);
            Property(x => x.UserBrowser).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserBrowserVersion).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserHostAddress).IsRequired().IsUnicode(false).HasMaxLength(16);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserLanguage).IsRequired().IsUnicode(false).HasMaxLength(5);
        }
    }
}