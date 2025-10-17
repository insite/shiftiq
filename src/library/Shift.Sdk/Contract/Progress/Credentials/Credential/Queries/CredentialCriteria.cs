using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICredentialCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? AchievementIdentifier { get; set; }
        Guid? AuthorityIdentifier { get; set; }
        Guid? EmployerGroupIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        string AuthorityLocation { get; set; }
        string AuthorityName { get; set; }
        string AuthorityReference { get; set; }
        string AuthorityType { get; set; }
        string CredentialDescription { get; set; }
        string CredentialGrantedDescription { get; set; }
        string CredentialNecessity { get; set; }
        string CredentialPriority { get; set; }
        string CredentialReminderType { get; set; }
        string CredentialRevokedReason { get; set; }
        string CredentialStatus { get; set; }
        string EmployerGroupStatus { get; set; }
        string ExpirationLifetimeUnit { get; set; }
        string ExpirationType { get; set; }
        string PublisherAddress { get; set; }
        string TransactionHash { get; set; }

        int? ExpirationLifetimeQuantity { get; set; }
        int? PublicationStatus { get; set; }

        decimal? CredentialGrantedScore { get; set; }
        decimal? CredentialHours { get; set; }
        decimal? CredentialRevokedScore { get; set; }

        DateTimeOffset? CredentialAssigned { get; set; }
        DateTimeOffset? CredentialExpirationExpected { get; set; }
        DateTimeOffset? CredentialExpirationReminderDelivered0 { get; set; }
        DateTimeOffset? CredentialExpirationReminderDelivered1 { get; set; }
        DateTimeOffset? CredentialExpirationReminderDelivered2 { get; set; }
        DateTimeOffset? CredentialExpirationReminderDelivered3 { get; set; }
        DateTimeOffset? CredentialExpirationReminderRequested0 { get; set; }
        DateTimeOffset? CredentialExpirationReminderRequested1 { get; set; }
        DateTimeOffset? CredentialExpirationReminderRequested2 { get; set; }
        DateTimeOffset? CredentialExpirationReminderRequested3 { get; set; }
        DateTimeOffset? CredentialExpired { get; set; }
        DateTimeOffset? CredentialGranted { get; set; }
        DateTimeOffset? CredentialRevoked { get; set; }
        DateTimeOffset? ExpirationFixedDate { get; set; }
    }
}