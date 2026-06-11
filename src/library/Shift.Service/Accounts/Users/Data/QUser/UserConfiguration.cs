using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("QUser", "identities");
        builder.HasKey(x => new { x.UserIdentifier });

        builder.Property(x => x.DefaultPassword).HasColumnName("DefaultPassword").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.DefaultPasswordExpired).HasColumnName("DefaultPasswordExpired");
        builder.Property(x => x.Email).HasColumnName("Email").IsRequired().IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.EmailVerified).HasColumnName("EmailVerified").IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.Honorific).HasColumnName("Honorific").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.ImageUrl).HasColumnName("ImageUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.Initials).HasColumnName("Initials").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.AccessGrantedToCmds).HasColumnName("AccessGrantedToCmds").IsRequired();
        builder.Property(x => x.LastName).HasColumnName("LastName").IsRequired().IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.MiddleName).HasColumnName("MiddleName").IsUnicode(false).HasMaxLength(38);
        builder.Property(x => x.FullName).HasColumnName("FullName").IsRequired().IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.SoundexFirstName).HasColumnName("SoundexFirstName").IsUnicode(false).HasMaxLength(4);
        builder.Property(x => x.SoundexLastName).HasColumnName("SoundexLastName").IsUnicode(false).HasMaxLength(4);
        builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
        builder.Property(x => x.TimeZone).HasColumnName("TimeZone").IsRequired().IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.UserLicenseAccepted).HasColumnName("UserLicenseAccepted");
        builder.Property(x => x.UserPasswordChanged).HasColumnName("UserPasswordChanged");
        builder.Property(x => x.UserPasswordExpired).HasColumnName("UserPasswordExpired").IsRequired();
        builder.Property(x => x.UserPasswordHash).HasColumnName("UserPasswordHash").IsRequired().IsUnicode(false).HasMaxLength(70);
        builder.Property(x => x.UtcArchived).HasColumnName("UtcArchived");
        builder.Property(x => x.UtcUnarchived).HasColumnName("UtcUnarchived");
        builder.Property(x => x.MultiFactorAuthentication).HasColumnName("MultiFactorAuthentication").IsRequired();
        builder.Property(x => x.MultiFactorAuthenticationCode).HasColumnName("MultiFactorAuthenticationCode").IsUnicode(false).HasMaxLength(6);
        builder.Property(x => x.EmailAlternate).HasColumnName("EmailAlternate").IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.PhoneMobile).HasColumnName("PhoneMobile").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.AccountCloaked).HasColumnName("AccountCloaked");
        builder.Property(x => x.PrimaryLoginMethod).HasColumnName("PrimaryLoginMethod");
        builder.Property(x => x.SecondaryLoginMethod).HasColumnName("SecondaryLoginMethod");
        builder.Property(x => x.OAuthProviderUserId).HasColumnName("OAuthProviderUserId").IsUnicode(false);
        builder.Property(x => x.LoginOrganizationCode).HasColumnName("LoginOrganizationCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.OldUserPasswordHash).HasColumnName("OldUserPasswordHash").IsUnicode(false).HasMaxLength(70);
        builder.Property(x => x.UserPasswordChangeRequested).HasColumnName("UserPasswordChangeRequested");
    }
}