using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TUserAuthenticationFactorConfiguration
        : EntityTypeConfiguration<TUserAuthenticationFactor>
    {
        public TUserAuthenticationFactorConfiguration() :this("accounts") {}
        public TUserAuthenticationFactorConfiguration(string schema)
        {
            ToTable(schema + $".{nameof(TUserAuthenticationFactor)}");

            HasKey(m => new { m.UserAuthenticationFactorIdentifier });

            HasRequired(m => m.User)
                .WithOptional(u => u.UserAuthenticationFactorInfo)
                .WillCascadeOnDelete(false);

            Property(m => m.OtpRecoveryPhrases).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(m => m.OtpTokenSecret).IsRequired();
            Property(m => m.OtpTokenRefreshed).IsRequired();
            Property(m => m.OtpMode).IsRequired();
            Property(m => m.ShortLivedCookieToken).IsOptional();
            Property(m => m.ShortCookieTokenStartTime).IsOptional();
        }
    }
}
