using System;

namespace Shift.Contract
{
    public class CreateCredential
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid? AuthorityIdentifier { get; set; }
        public Guid CredentialIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string AuthorityLocation { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityReference { get; set; }
        public string AuthorityType { get; set; }
        public string CredentialDescription { get; set; }
        public string CredentialGrantedDescription { get; set; }
        public string CredentialNecessity { get; set; }
        public string CredentialPriority { get; set; }
        public string CredentialReminderType { get; set; }
        public string CredentialRevokedReason { get; set; }
        public string CredentialStatus { get; set; }
        public string EmployerGroupStatus { get; set; }
        public string ExpirationLifetimeUnit { get; set; }
        public string ExpirationType { get; set; }
        public string PublisherAddress { get; set; }
        public string TransactionHash { get; set; }

        public int? ExpirationLifetimeQuantity { get; set; }
        public int? PublicationStatus { get; set; }

        public decimal? CredentialGrantedScore { get; set; }
        public decimal? CredentialHours { get; set; }
        public decimal? CredentialRevokedScore { get; set; }

        public DateTimeOffset? CredentialAssigned { get; set; }
        public DateTimeOffset? CredentialExpirationExpected { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered0 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered1 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered2 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered3 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested0 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested1 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested2 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested3 { get; set; }
        public DateTimeOffset? CredentialExpired { get; set; }
        public DateTimeOffset? CredentialGranted { get; set; }
        public DateTimeOffset? CredentialRevoked { get; set; }
        public DateTimeOffset? ExpirationFixedDate { get; set; }
    }
}