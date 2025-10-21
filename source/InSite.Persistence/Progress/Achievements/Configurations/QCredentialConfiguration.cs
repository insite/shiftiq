using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QCredentialConfiguration : EntityTypeConfiguration<QCredential>
    {
        public QCredentialConfiguration() : this("achievements") { }

        public QCredentialConfiguration(string schema)
        {
            ToTable(schema + ".QCredential");
            HasKey(x => new { x.CredentialIdentifier });

            Property(x => x.AchievementIdentifier).IsRequired();
            Property(x => x.AuthorityLocation).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AuthorityName).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AuthorityReference).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.CredentialAssigned).IsOptional();
            Property(x => x.CredentialDescription).IsOptional().IsUnicode(false);
            Property(x => x.CredentialExpirationExpected).IsOptional();
            Property(x => x.CredentialExpirationReminderDelivered0).IsOptional();
            Property(x => x.CredentialExpirationReminderDelivered1).IsOptional();
            Property(x => x.CredentialExpirationReminderDelivered2).IsOptional();
            Property(x => x.CredentialExpirationReminderDelivered3).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested0).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested1).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested2).IsOptional();
            Property(x => x.CredentialExpirationReminderRequested3).IsOptional();
            Property(x => x.CredentialExpired).IsOptional();
            Property(x => x.CredentialIdentifier).IsRequired();
            Property(x => x.CredentialGranted).IsOptional();
            Property(x => x.CredentialHours).IsOptional();
            Property(x => x.CredentialNecessity).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CredentialPriority).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CredentialReminderType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.CredentialRevoked).IsOptional();
            Property(x => x.CredentialStatus).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.ExpirationFixedDate).IsOptional();
            Property(x => x.ExpirationLifetimeQuantity).IsOptional();
            Property(x => x.ExpirationLifetimeUnit).IsOptional().IsUnicode(false).HasMaxLength(6);
            Property(x => x.ExpirationType).IsOptional().IsUnicode(false).HasMaxLength(8);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.EmployerGroupStatus).IsOptional().IsUnicode(false).HasMaxLength(100);

            HasRequired(x => x.Achievement).WithMany(x => x.Credentials).HasForeignKey(x => x.AchievementIdentifier);
            HasOptional(x => x.EmployerGroup).WithMany(x => x.Credentials).HasForeignKey(x => x.EmployerGroupIdentifier);
        }
    }
}
