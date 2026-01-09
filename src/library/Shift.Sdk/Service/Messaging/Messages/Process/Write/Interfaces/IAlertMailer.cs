using System;

using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Application.Messages.Write
{
    public interface IAlertMailer
    {
        void Send(EmailDraft email);

        // New approach

        Guid[] Send(Notification notification);
        Guid[] Send(Notification notification, Guid? to);

        // Old approach

        void Send(Guid organizationId, Guid userId, AlertCompetencyValidationRequested alert, Guid[] to);
        void Send(Guid organizationId, Guid userId, AlertEmployerGroupCreated alert);
        void Send(Guid organizationId, Guid userId, AlertEventVenueChanged alert);
        void Send(Guid organizationId, Guid userId, AlertHelpRequested alert);
        void Send(Guid organizationId, Guid userId, AlertInvoicePaid alert, string[] attachments = null);
        void Send(Guid organizationId, Guid userId, AlertClassScheduled alert);
        void Send(Guid organizationId, Guid userId, AlertPasswordChanged alert);
        Guid[] Send(Guid organizationId, Guid userId, AlertPasswordResetRequested alert);
        void Send(Guid organizationId, Guid userId, AlertProgressCompleted alert);
        void Send(Guid organizationId, Guid userId, AlertRegistrantContactInformationChanged alert);
        void Send(Guid organizationId, Guid userId, AlertRegistrationComplete alert, Guid[] cc, Guid[] bcc, string[] attachments);
        void Send(Guid organizationId, Guid userId, AlertRegistrationInvitation alert);
        void Send(Guid organizationId, Guid userId, AlertRegistrationInvitationExpired alert, Guid? to);
        void Send(Guid organizationId, Guid userId, AlertApplicationAccessRequested alert);
        void Send(Guid organizationId, AlertUnhandledExceptionIntercepted alert);
        void Send(Guid organizationId, Guid userId, AlertUserAccountArchived alert);
        void Send(Guid organizationId, Guid userId, AlertUserAccountCreated alert);
        void Send(Guid organizationId, Guid userId, AlertPersonCodeNotEntered alert);
        void Send(Guid organizationId, Guid userId, AlertJobsCandidateContactRequested alert);
        void Send(Guid organizationId, Guid userId, AlertJobsCandidateAppliedForOpportunity alert, string[] attachments);
        void Send(Guid organizationId, Guid userId, AlertUserAccountWelcomed alert, Guid[] cc = null, Guid[] bcc = null);
        void Send(Guid organizationId, Guid userId, AlertManagementWelcomeEmail alert, Guid[] cc = null, Guid[] bcc = null);
        void Send(Guid organizationId, Guid userId, AlertUserEmailVerificationRequested alert);
        void Send(Guid organizationId, Guid userId, AlertUserRegistrationSubmitted alert);
        void Send(Guid organizationId, Guid userId, AlertOTPActivationCode alert);
        void Send(Guid organizationId, Guid userId, Guid? recipientId, AlertAddedToWaitingList alert);
        void Send(Guid organizationId, Guid userId, Guid? recipientId, AlertApplicationAccessGranted alert);
        void Send(Guid organization, Guid author, Notification_IssueOwnerChanged alert);
        void Send(Guid organization, Guid author, Notification_PersonCommentFlagged alert);
        void Send(Guid organizationId, Guid userId, AlertUnsubscribeSuccess alert);
        void Send(Guid organizationId, Guid userId, AlertWelcomeLearner alert);
        void Send(Guid organizationId, Guid userId, AlertManagementWelcomeEmail alert, string[] attachments = null);
        void Send(Guid organizationId, Guid userId, AlertLearningWelcomeEmail alert);
    }
}