using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Progress;

public class QCredentialConfiguration : IEntityTypeConfiguration<QCredentialEntity>
{
    public void Configure(EntityTypeBuilder<QCredentialEntity> builder)
    {
        builder.ToTable("QCredential", "achievements");
        builder.HasKey(x => new { x.CredentialIdentifier });

        builder.Property(x => x.AchievementIdentifier).HasColumnName("AchievementIdentifier").IsRequired();
        builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
        builder.Property(x => x.CredentialGranted).HasColumnName("CredentialGranted");
        builder.Property(x => x.CredentialRevoked).HasColumnName("CredentialRevoked");
        builder.Property(x => x.CredentialExpired).HasColumnName("CredentialExpired");
        builder.Property(x => x.ExpirationType).HasColumnName("ExpirationType").IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.ExpirationFixedDate).HasColumnName("ExpirationFixedDate");
        builder.Property(x => x.ExpirationLifetimeQuantity).HasColumnName("ExpirationLifetimeQuantity");
        builder.Property(x => x.ExpirationLifetimeUnit).HasColumnName("ExpirationLifetimeUnit").IsUnicode(false).HasMaxLength(6);
        builder.Property(x => x.CredentialAssigned).HasColumnName("CredentialAssigned");
        builder.Property(x => x.CredentialStatus).HasColumnName("CredentialStatus").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.CredentialReminderType).HasColumnName("CredentialReminderType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.AuthorityName).HasColumnName("AuthorityName").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.AuthorityLocation).HasColumnName("AuthorityLocation").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.AuthorityReference).HasColumnName("AuthorityReference").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.CredentialDescription).HasColumnName("CredentialDescription").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.CredentialHours).HasColumnName("CredentialHours").HasPrecision(5, 2);
        builder.Property(x => x.CredentialExpirationExpected).HasColumnName("CredentialExpirationExpected");
        builder.Property(x => x.CredentialExpirationReminderRequested0).HasColumnName("CredentialExpirationReminderRequested0");
        builder.Property(x => x.CredentialExpirationReminderRequested1).HasColumnName("CredentialExpirationReminderRequested1");
        builder.Property(x => x.CredentialExpirationReminderRequested2).HasColumnName("CredentialExpirationReminderRequested2");
        builder.Property(x => x.CredentialExpirationReminderRequested3).HasColumnName("CredentialExpirationReminderRequested3");
        builder.Property(x => x.CredentialExpirationReminderDelivered0).HasColumnName("CredentialExpirationReminderDelivered0");
        builder.Property(x => x.CredentialExpirationReminderDelivered1).HasColumnName("CredentialExpirationReminderDelivered1");
        builder.Property(x => x.CredentialExpirationReminderDelivered2).HasColumnName("CredentialExpirationReminderDelivered2");
        builder.Property(x => x.CredentialExpirationReminderDelivered3).HasColumnName("CredentialExpirationReminderDelivered3");
        builder.Property(x => x.CredentialIdentifier).HasColumnName("CredentialIdentifier").IsRequired();
        builder.Property(x => x.CredentialNecessity).HasColumnName("CredentialNecessity").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CredentialPriority).HasColumnName("CredentialPriority").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.AuthorityIdentifier).HasColumnName("AuthorityIdentifier");
        builder.Property(x => x.AuthorityType).HasColumnName("AuthorityType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.CredentialRevokedReason).HasColumnName("CredentialRevokedReason").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.CredentialGrantedDescription).HasColumnName("CredentialGrantedDescription").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.CredentialGrantedScore).HasColumnName("CredentialGrantedScore").HasPrecision(5, 4);
        builder.Property(x => x.CredentialRevokedScore).HasColumnName("CredentialRevokedScore").HasPrecision(5, 4);
        builder.Property(x => x.TransactionHash).HasColumnName("TransactionHash").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.PublisherAddress).HasColumnName("PublisherAddress").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.PublicationStatus).HasColumnName("PublicationStatus");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");
        builder.Property(x => x.EmployerGroupIdentifier).HasColumnName("EmployerGroupIdentifier");
        builder.Property(x => x.EmployerGroupStatus).HasColumnName("EmployerGroupStatus").IsUnicode(false).HasMaxLength(100);

    }
}