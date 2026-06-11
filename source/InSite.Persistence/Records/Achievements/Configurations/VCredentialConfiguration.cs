using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class VCredentialConfiguration : EntityTypeConfiguration<VCredential>
    {
        public VCredentialConfiguration() : this("achievements") { }

        public VCredentialConfiguration(string schema)
        {
            ToTable(schema + ".VCredential");
            HasKey(x => new { x.AchievementIdentifier, x.UserIdentifier });

            Property(x => x.AchievementDescription).IsOptional().IsUnicode(false);
            Property(x => x.AchievementExpirationFixedDate).IsOptional();
            Property(x => x.AchievementExpirationLifetimeQuantity).IsOptional();
            Property(x => x.AchievementExpirationLifetimeUnit).IsOptional().IsUnicode(false).HasMaxLength(6);
            Property(x => x.AchievementExpirationType).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.AchievementIsEnabled).IsRequired();
            Property(x => x.AchievementLabel).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AchievementTitle).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.AuthorityLocation).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AuthorityName).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AuthorityReference).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.CredentialAssigned).IsOptional();
            Property(x => x.CredentialDescription).IsOptional().IsUnicode(false);
            Property(x => x.CredentialNecessity).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CredentialPriority).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CredentialExpirationExpected).IsOptional();
            Property(x => x.CredentialExpirationFixedDate).IsOptional();
            Property(x => x.CredentialExpirationLifetimeQuantity).IsOptional();
            Property(x => x.CredentialExpirationLifetimeUnit).IsOptional().IsUnicode(false).HasMaxLength(6);
            Property(x => x.CredentialExpirationReminderDelivered0).IsOptional();
            Property(x => x.CredentialExpirationReminderDelivered1).IsOptional();
            Property(x => x.CredentialExpirationReminderDelivered2).IsOptional();
            Property(x => x.CredentialExpirationReminderDelivered3).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested0).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested1).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested2).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested3).IsOptional();
            Property(x => x.CredentialExpirationType).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.CredentialExpired).IsOptional();
            Property(x => x.CredentialGranted).IsOptional();
            Property(x => x.CredentialHours).IsOptional();
            Property(x => x.CredentialRevoked).IsOptional();
            Property(x => x.CredentialStatus).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.EmployerGroupIdentifier).IsOptional();
            Property(x => x.EmployerGroupName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.EmployerGroupStatus).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserFullName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserRegion).IsOptional().IsUnicode(false).HasMaxLength(20);
        }
    }
}
