using System;
using System.Linq;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Application.Messages.Write
{
    public class AlertMailer : IAlertMailer
    {
        private readonly EnvironmentName _environment;
        private readonly IEmailOutbox _outbox;
        private readonly Action<string> _error;

        public AlertMailer(EnvironmentName environment, IEmailOutbox outbox, Action<string> error)
        {
            _environment = environment;
            _outbox = outbox;
            _error = error;
        }

        public void Send(EmailDraft draft)
        {
            SendAlert(draft);
        }

        public Guid[] Send(Notification notification)
        {
            return Send(notification, null);
        }

        public Guid[] Send(Notification notification, Guid? to)
        {
            var organization = notification.GetOriginOrganization();
            var user = notification.GetOriginUser();
            var type = notification.Type;

            return SendAlert(organization, user, to, notification, type);
        }

        public void Send(Guid organizationId, Guid userId, AlertCompetencyValidationRequested alert, Guid[] to)
            => SendAlert(organizationId, userId, null, alert, NotificationType.CmdsCompetencyValidationRequested, to);

        public void Send(Guid organizationId, Guid userId, AlertEventVenueChanged alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.EventVenueChanged);

        public void Send(Guid organizationId, Guid userId, AlertEmployerGroupCreated alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.EmployerGroupCreated);

        public void Send(Guid organizationId, Guid userId, AlertHelpRequested alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.HelpRequested);

        public void Send(Guid organizationId, Guid userId, Guid? recipientId, AlertAddedToWaitingList alert)
            => SendAlert(organizationId, userId, recipientId, alert, NotificationType.AddedToWaitingList);

        public void Send(Guid organizationId, Guid userId, Guid? recipientId, AlertApplicationAccessGranted alert)
            => SendAlert(organizationId, userId, recipientId, alert, NotificationType.ApplicationAccessGranted);

        public void Send(Guid organizationId, Guid userId, AlertApplicationAccessRequested alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.ApplicationAccessRequested);

        public void Send(Guid organizationId, Guid userId, AlertUserEmailVerificationRequested alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.UserEmailVerificationRequested);

        public void Send(Guid organizationId, Guid userId, AlertOTPActivationCode alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.UserOTPActivationCode);

        public Guid[] Send(Guid organizationId, Guid userId, AlertPasswordResetRequested alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.PasswordResetRequested);

        public void Send(Guid organizationId, Guid userId, AlertPasswordChanged alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.PasswordChanged);

        public void Send(Guid organizationId, Guid userId, AlertProgressCompleted alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.ProgressCompleted);

        public void Send(Guid organizationId, AlertUnhandledExceptionIntercepted alert)
            => SendAlert(organizationId, UserIdentifiers.Root, null, alert, NotificationType.UnhandledExceptionIntercepted);

        public void Send(Guid organizationId, Guid userId, AlertUserRegistrationSubmitted alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.UserRegistrationSubmitted);

        public void Send(Guid organizationId, Guid userId, AlertUserAccountArchived alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.UserAccountArchived);

        public void Send(Guid organizationId, Guid userId, AlertUserAccountCreated alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.UserAccountCreated);

        public void Send(Guid organizationId, Guid userId, AlertPersonCodeNotEntered alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.PersonCodeNotEntered);

        public void Send(Guid organizationId, Guid userId, AlertUserAccountWelcomed alert, Guid[] cc = null, Guid[] bcc = null)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.UserAccountWelcomed, null, cc, bcc);

        public void Send(Guid organizationId, Guid userId, AlertManagementWelcomeEmail alert, Guid[] cc = null, Guid[] bcc = null)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.ManagementWelcomeEmail, null, cc, bcc);

        public void Send(Guid organizationId, Guid userId, AlertRegistrantContactInformationChanged alert)
            => SendAlert(organizationId, userId, null, alert, NotificationType.RegistrantContactInformationChanged);

        public void Send(Guid organizationId, Guid userId, AlertRegistrationComplete alert, Guid[] cc, Guid[] bcc, string[] attachments)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.RegistrationComplete, null, cc, bcc, attachments);

        public void Send(Guid organizationId, Guid userId, AlertRegistrationInvitation alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.RegistrationInvitation);

        public void Send(Guid organizationId, Guid userId, AlertRegistrationInvitationExpired alert, Guid? to)
            => SendAlert(organizationId, userId, to, alert, NotificationType.RegistrationInvitationExpired);

        public void Send(Guid organizationId, Guid userId, AlertInvoicePaid alert, string[] attachments = null)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.InvoicePaid, null, null, null, attachments);

        public void Send(Guid organizationId, Guid userId, AlertClassScheduled alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.ClassScheduled);

        public void Send(Guid organizationId, Guid userId, AlertJobsCandidateContactRequested alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.JobsCandidateContactRequested);

        public void Send(Guid organizationId, Guid userId, AlertJobsCandidateAppliedForOpportunity alert, string[] attachments)
            => SendAlert(organizationId, userId, null, alert, NotificationType.JobsCandidateAppliedForOpportunity, new Guid[] { alert.EmployerIdentifier }, null, null, attachments);

        public void Send(Guid organization, Guid author, Notification_IssueOwnerChanged alert)
            => SendAlert(organization, author, null, alert, NotificationType.IssueOwnerChanged);

        public void Send(Guid organization, Guid author, Notification_PersonCommentFlagged alert)
            => SendAlert(organization, author, null, alert, NotificationType.PersonCommentFlagged);

        public void Send(Guid organizationId, Guid userId, AlertUnsubscribeSuccess alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.UnsubscribeSuccess);

        public void Send(Guid organizationId, Guid userId, AlertWelcomeLearner alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.WelcomeLearner);

        public void Send(Guid organizationId, Guid userId, AlertLearningWelcomeEmail alert)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.LearningWelcomeEmail);

        public void Send(Guid organizationId, Guid userId, AlertManagementWelcomeEmail alert, string[] attachments = null)
            => SendAlert(organizationId, userId, userId, alert, NotificationType.ManagementWelcomeEmail, null, null, null, attachments);

        private Guid[] SendAlert(Guid organizationId, Guid userId, Guid? recipientId, Notification alert,
            NotificationType type, Guid[] to = null, Guid[] cc = null, Guid[] bcc = null, string[] attachments = null)
        {
            if (alert.Slug.IsEmpty())
                alert.Type = type;

            try
            {
                var emails = _outbox.Compose(_environment, type, organizationId, userId, recipientId, alert.MessageIdentifier, alert.BuildVariableList(), to, cc, bcc);

                foreach (var email in emails)
                {
                    if (!ValidateEmail(email))
                        continue;

                    if (Calendar.IsEmpty(email.MailoutScheduled))
                        email.MailoutScheduled = DateTimeOffset.Now;

                    email.ContentAttachments.AddRange(attachments.EmptyIfNull());

                    _outbox.Send(email, $"Notification: {type}", false, type.ToString());
                }

                return emails.Select(x => x.MailoutIdentifier).ToArray();
            }
            catch (MessageNotFoundException)
            {
                return new Guid[0];
            }
        }

        private void SendAlert(EmailDraft email)
        {
            if (ValidateEmail(email))
                _outbox.Send(email, "Alert");
        }

        private bool ValidateEmail(EmailDraft email)
        {
            if (email.SenderType == "Mailgun")
                return email.RecipientListTo.IsNotEmpty();

            _error?.Invoke($"The sender for this email is {email.SenderType} but instead must be Mailgun. (Message {email.MessageIdentifier} Mailout {email.MailoutIdentifier})");

            return false;
        }
    }
}