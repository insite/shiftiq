using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSessionEntity>
{
    public void Configure(EntityTypeBuilder<UserSessionEntity> builder)
    {
        builder.ToTable("TUserSession", "identity");
        builder.HasKey(x => new { x.SessionIdentifier });

        builder.Property(x => x.AuthenticationErrorType).HasColumnName("AuthenticationErrorType").IsUnicode(false).HasMaxLength(256);
        builder.Property(x => x.AuthenticationErrorMessage).HasColumnName("AuthenticationErrorMessage").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.SessionCode).HasColumnName("SessionCode").IsRequired().IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.SessionIsAuthenticated).HasColumnName("SessionIsAuthenticated").IsRequired();
        builder.Property(x => x.SessionStarted).HasColumnName("SessionStarted").IsRequired();
        builder.Property(x => x.SessionStopped).HasColumnName("SessionStopped");
        builder.Property(x => x.SessionMinutes).HasColumnName("SessionMinutes");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.UserAgent).HasColumnName("UserAgent").IsUnicode(false).HasMaxLength(512);
        builder.Property(x => x.UserBrowser).HasColumnName("UserBrowser").IsRequired().IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.UserBrowserVersion).HasColumnName("UserBrowserVersion").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.UserEmail).HasColumnName("UserEmail").IsRequired().IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.UserHostAddress).HasColumnName("UserHostAddress").IsRequired().IsUnicode(false).HasMaxLength(16);
        builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
        builder.Property(x => x.UserLanguage).HasColumnName("UserLanguage").IsRequired().IsUnicode(false).HasMaxLength(5);
        builder.Property(x => x.SessionIdentifier).HasColumnName("SessionIdentifier").IsRequired();
        builder.Property(x => x.AuthenticationSource).HasColumnName("AuthenticationSource").IsRequired().IsUnicode(false).HasMaxLength(19);
    }
}