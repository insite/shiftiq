using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration() : this("identities") { }

        public UserConfiguration(string schema)
        {
            ToTable(schema + ".User");
            HasKey(x => new { x.UserIdentifier });

            Property(x => x.DefaultPassword).IsUnicode(false).HasMaxLength(100);
            Property(x => x.Email).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.EmailAlternate).IsUnicode(false).HasMaxLength(254);
            Property(x => x.EmailVerified).IsUnicode(false).HasMaxLength(254);
            Property(x => x.FirstName).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.FullName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.Honorific).IsUnicode(false).HasMaxLength(32);
            Property(x => x.ImageUrl).IsUnicode(false).HasMaxLength(254);
            Property(x => x.Initials).IsUnicode(false).HasMaxLength(32);
            Property(x => x.LastName).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.MiddleName).IsUnicode(false).HasMaxLength(32);
            Property(x => x.MultiFactorAuthenticationCode).IsUnicode(false).HasMaxLength(6);
            Property(x => x.PhoneMobile).IsUnicode(false).HasMaxLength(32);
            Property(x => x.SoundexFirstName).IsUnicode(false).HasMaxLength(4);
            Property(x => x.SoundexLastName).IsUnicode(false).HasMaxLength(4);
            Property(x => x.TimeZone).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.UserPasswordHash).IsRequired().IsUnicode(false).HasMaxLength(70);
            Property(x => x.OldUserPasswordHash).IsUnicode(false).HasMaxLength(70);
        }
    }
}
