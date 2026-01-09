using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public class AssessmentAttemptStartedNotification : Notification
    {
        public AssessmentAttemptStartedNotification()
        {
            Type = NotificationType.AssessmentAttemptStarted;
        }

        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }
        public string AssessmentFormName { get; set; }
    }

    public class AssessmentAttemptCompletedNotification : Notification
    {
        public AssessmentAttemptCompletedNotification()
        {
            Type = NotificationType.AssessmentAttemptCompleted;
        }

        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }
        public string AssessmentFormName { get; set; }
        public string AssessmentAttemptScore { get; set; }
    }

    public class AlertPersonCodeNotEntered : Notification
    {
        public StringDictionary PersonCodeNotEnteredProperties { get; set; }

        public override StringDictionary BuildVariableList()
        {
            if (PersonCodeNotEnteredProperties != null)
                return PersonCodeNotEnteredProperties;
            return new StringDictionary();
        }
    }

    public class AuthenticationAlarmTriggeredNotification : Notification
    {
        public AuthenticationAlarmTriggeredNotification()
        {
            Type = NotificationType.AuthenticationAlarmTriggered;
        }

        public int FailedLoginCount { get; set; }
        public string Organization { get; set; }
        public string SignInUrl { get; set; }
        public string UserEmail { get; set; }
        public string UserHostAddress { get; set; }
    }

    public class AlertCompetencyValidationRequested : Notification
    {
        public Guid UserIdentifier { get; set; }

        public string AssignmentCount { get; set; }
        public string AssignmentList { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public DateTimeOffset Notified { get; set; }
    }

    public class AlertEmployerGroupCreated : Notification
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
    }

    public class AlertEventVenueChanged : Notification
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }

        public Guid EventIdentifier { get; set; }
        public string EventDate { get; set; }
        public string EventName { get; set; }
        public string EventNumber { get; set; }
    }

    public class AlertHelpRequested : Notification
    {
        public string BrowserAddress { get; set; }
        public string BrowserName { get; set; }
        public string Organization { get; set; }
        public string RequestDescription { get; set; }
        public string RequestSource { get; set; }
        public string RequestSummary { get; set; }
        public string RequestUrl { get; set; }
        public string UserEmail { get; set; }
        public string UserEmployer { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string BrowserUrl { get; set; }
        public string UserCompany { get; set; }
    }

    public class AlertProgressCompleted : Notification
    {
        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }
        public string GradeItemName { get; set; }
        public string GradeItemScore { get; set; }
    }

    public class AlertApplicationAccessGranted : Notification
    {
        public string ApproverEmail { get; set; }
        public string ApproverName { get; set; }
        public string Organization { get; set; }
        public string UserAccess { get; set; }
        public string UserFirstName { get; set; }
    }

    public class AlertApplicationAccessRequested : Notification
    {
        public string AppUrl { get; set; }
        public string Organization { get; set; }
        public string SourceUrl { get; set; }
        public string UserEmail { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserName { get; set; }

        public string ApproveAccessUrl
            => $"{AppUrl}/ui/admin/contacts/people/approve-access?user={UserIdentifier}";
    }

    public class AlertUserEmailVerificationRequested : Notification
    {
        public string AppUrl { get; set; }
        public string Organization { get; set; }
        public string UserEmail { get; set; }
        public Guid UserIdentifier { get; set; }
        public string VerifyEmailUrl => $"{AppUrl}/ui/lobby/verify-email?thumbprint={StringHelper.EncodeBase64Url(UserIdentifier.ToString())}";
    }

    public class AlertPasswordResetRequested : Notification
    {
        public Guid TenantIdentifier { get; set; }
        public Guid Recipient { get; set; }
        public string ResetUrl { get; set; }
    }

    public class AlertPasswordChanged : Notification
    {
        public Guid Recipient { get; set; }
        public Guid Tenant { get; set; }

        public string BrowserAddress { get; set; }
        public string UserEmail { get; set; }
        public string UserTimeZone { get; set; }
        public string EventTime
            => TimeZones.Format((DateTimeOffset)DateTime.Now, UserTimeZone);
    }

    public class AlertUserRegistrationSubmitted : Notification
    {
        public string ApprovalUrl { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string Organization { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string RegistrationUrl { get; set; }
        public Guid Thumbprint { get; set; }
    }

    public class AlertUserAccountArchived : Notification
    {
        public Guid TenantIdentifier { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }

    public class AlertUserAccountCreated : Notification
    {
        public Guid TenantIdentifier { get; set; }
        public string Tenant { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string CompanyTitle { get; set; }
    }

    public class AlertRegistrantContactInformationChanged : Notification
    {
        public string ContactLoginName { get; set; }
        public string ContactEmail { get; set; }
        public string EventName { get; set; }
        public string ContactChangedFields { get; set; }
    }

    public class AlertInvoicePaid : Notification
    {
        public StringDictionary InvoicePaidProperties { get; set; }

        public override StringDictionary BuildVariableList()
        {
            if (InvoicePaidProperties != null)
                return InvoicePaidProperties;
            return new StringDictionary();
        }
    }

    public class AlertClassScheduled : Notification
    {
        public StringDictionary ClassScheduledProperties { get; set; }

        public override StringDictionary BuildVariableList()
        {
            if (ClassScheduledProperties != null)
                return ClassScheduledProperties;
            return new StringDictionary();
        }
    }

    public class AlertRegistrationComplete : Notification
    {
        public StringDictionary RegistrationCompleteProperties { get; set; }

        public override StringDictionary BuildVariableList()
        {
            if (RegistrationCompleteProperties != null)
                return RegistrationCompleteProperties;
            return new StringDictionary();
        }
    }

    public class AlertRegistrationInvitation : Notification
    {
        public string CandidateFullName { get; set; }
        public string ClassTitle { get; set; }
        public string RegistrationEndTime { get; set; }
        public string ClassStartTime { get; set; }
        public string ClassRegistrationLink { get; set; }
        public string ClassAchievement { get; set; }

        public override StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary
            {
                { "CandidateFullName", CandidateFullName },
                { "ClassTitle", ClassTitle },
                { "RegistrationEndTime", RegistrationEndTime },
                { "ClassStartTime", ClassStartTime },
                { "ClassRegistrationLink", ClassRegistrationLink },
                { "ClassAchievement", ClassAchievement }
            };

            return dict;
        }
    }

    public class AlertRegistrationInvitationExpired : Notification
    {
        public string CandidateFirstName { get; set; }
        public string CandidateLastName { get; set; }
        public string CandidateEmail { get; set; }
        public string ClassTitle { get; set; }
        public string ClassRegistrationLink { get; set; }
    }

    public class AlertUnhandledExceptionIntercepted : Notification
    {
        public string ExceptionMessage { get; set; }
    }

    public class AlertAddedToWaitingList : Notification
    {
        public AlertAddedToWaitingList()
        {
            Type = NotificationType.AddedToWaitingList;
        }

        public string CandidateFullName { get; set; }
        public string CandidateEmail { get; set; }
        public string ClassTitle { get; set; }
        public string ClassStartTime { get; set; }
        public string ClassAchievement { get; set; }
        public string Employer { get; set; }
        public string Organization { get; set; }
        public string WaitlistedBy { get; set; }
    }


    public class AlertUserAccountWelcomed : Notification
    {
        public string UserFirstName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserPasswordHash { get; set; }
        public string Url { get; set; }

        public string TenantCode { get; set; }
        public Guid TenantIdentifier { get; set; }
        public string TenantName { get; set; }

        public DateTimeOffset? UserAccessGranted { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Dictionary<string, string> WelcomeMsgDictionary { get; set; }

        public override StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary
            {
                { "AppUrl", Url },
                { "FirstName", UserFirstName },
                { "Email", UserEmail },
                { "CompanyTitle", TenantName },
                { "SignInUrl", Url + "/ui/lobby/signin" }
            };

            foreach (var property in WelcomeMsgDictionary)
            {
                dict.Add(property.Key, property.Value);
            }

            return dict;
        }
    }

    public class AlertManagementWelcomeEmail : Notification
    {
        public string UserFirstName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserPasswordHash { get; set; }
        public string Url { get; set; }
        public StringDictionary InvoicePaidProperties { get; set; }
        public Dictionary<string, string> WelcomeMsgDictionary { get; set; }

        public override StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary
            {
                { "AppUrl", Url },
                { "FirstName", UserFirstName },
                { "Email", UserEmail },
                { "SignInUrl", $"{Url}/ui/lobby/signin" }
            };

            if (InvoicePaidProperties != null)
            {
                foreach (string key in InvoicePaidProperties.Keys)
                    dict[key] = InvoicePaidProperties[key];
            }

            if (WelcomeMsgDictionary != null)
            {
                foreach (var kv in WelcomeMsgDictionary)
                    dict[kv.Key] = kv.Value;
            }

            return dict;
        }
    }

    public class AlertJournalUpdated : Notification
    {
        public string JournalSetupName { get; set; }
        public string JournalSetupTitle { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        public string ChangeAction { get; set; }

        public override StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary
            {
                { "JournalSetupName", JournalSetupName },
                { "JournalSetupTitle", JournalSetupTitle },
                { "UserEmail", UserEmail },
                { "UserFullName", UserFullName },
                { "ChangeAction", ChangeAction }
            };

            return dict;
        }
    }

    public class AlertJournalCommentedByValidator : Notification
    {
        public string JournalSetupName { get; set; }
        public string JournalSetupTitle { get; set; }
        public string ValidatorFullName { get; set; }

        public override StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary
            {
                { "JournalSetupName", JournalSetupName },
                { "JournalSetupTitle", JournalSetupTitle },
                { "ValidatorFullName", ValidatorFullName }
            };

            return dict;
        }
    }

    public class AlertOTPActivationCode : Notification
    {
        public string ConfirmationCode { get; set; }
        public string Organization { get; set; }
    }

    public class AlertJobsCandidateContactRequested : Notification
    {
        public string CandidateFirstName { get; set; }
        public string CandidateLastName { get; set; }
        public string EmployerName { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyName { get; set; }
        public string Message { get; set; }
    }

    public class AlertJobsCandidateAppliedForOpportunity : Notification
    {
        public Guid OpportunityIdentifier { get; set; }
        public Guid CandidateIdentifier { get; set; }
        public string CandidateFirstName { get; set; }
        public string CandidateLastName { get; set; }
        public string CandidatePhoneNumber { get; set; }
        public string CandidateEmailAddress { get; set; }
        public string CandidateUrl { get; set; }
        public string JobPosition { get; set; }
        public Guid EmployerIdentifier { get; set; }
        public StringCollection Attachments { get; set; }
    }

    public class Notification_PersonCommentFlagged : Notification
    {
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string AuthorEmail { get; set; }

        public string TopicFirstName { get; set; }
        public string TopicLastName { get; set; }
        public string TopicEmail { get; set; }

        public string CommentText { get; set; }

        public override StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary();
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var text = (property.GetValue(this) ?? string.Empty).ToString();
                dict.Add(property.Name, text);
            }
            return dict;
        }
    }

    public class Notification_IssueOwnerChanged : Notification
    {
        public string IssueNumber { get; set; }
        public string IssueType { get; set; }
        public string IssueStatus { get; set; }
        public string IssueSummary { get; set; }

        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerEmail { get; set; }

        public override StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary();
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var text = (property.GetValue(this) ?? string.Empty).ToString();
                dict.Add(property.Name, text);
            }
            return dict;
        }
    }

    public interface ILogbookNotification
    {
        Guid? OriginOrganization { get; set; }
        Guid? OriginUser { get; set; }

        string OrganizationCode { get; set; }
        string OrganizationName { get; set; }
        string AppUrl { get; set; }

        string LogbookTitle { get; set; }
        string LogbookUrl { get; set; }

        string ActorName { get; set; }

        Guid? JournalSetupIdentifier { get; set; }

        Guid? MessageToLearnerWhenLogbookModified { get; set; }
        Guid? MessageToLearnerWhenLogbookStarted { get; set; }
        Guid? MessageToValidatorWhenLogbookModified { get; set; }
    }

    public class LogbookJoinedNotification : Notification, ILogbookNotification
    {
        public LogbookJoinedNotification()
        {
            Type = NotificationType.LogbookJoined;
        }

        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string AppUrl { get; set; }

        public string LogbookTitle { get; set; }
        public string LogbookUrl { get; set; }

        public string ActorName { get; set; }

        public Guid? JournalSetupIdentifier { get; set; }
        public Guid? LearnerIdentifier { get; set; }

        public Guid? MessageToLearnerWhenLogbookStarted { get; set; }

        #region Unused

        Guid? ILogbookNotification.MessageToLearnerWhenLogbookModified { get => null; set { } }
        Guid? ILogbookNotification.MessageToValidatorWhenLogbookModified { get => null; set { } }

        #endregion
    }

    public class LogbookChangedNotification : Notification, ILogbookNotification
    {
        public LogbookChangedNotification()
        {
            Type = NotificationType.LogbookModified;
        }

        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string AppUrl { get; set; }

        public string LogbookTitle { get; set; }
        public string LogbookUrl { get; set; }

        public string ModificationType { get; set; }

        public string ActorName { get; set; }

        public Guid? ExperienceIdentifier { get; set; }
        public Guid? JournalSetupIdentifier { get; set; }
        public Guid? LearnerIdentifier { get; set; }
        public Guid[] ValidatorIdentifiers { get; set; }

        public Guid? MessageToLearnerWhenLogbookModified { get; set; }
        public Guid? MessageToValidatorWhenLogbookModified { get; set; }

        #region Unused

        Guid? ILogbookNotification.MessageToLearnerWhenLogbookStarted { get => null; set { } }

        #endregion
    }

    public class MembershipStartedNotification : Notification
    {
        public MembershipStartedNotification()
        {
            Type = NotificationType.MembershipStarted;
        }

        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string AppUrl { get; set; }
        public string EditPersonUrl { get; set; }

        public string Reason { get; set; }
    }

    public class MembershipEndedNotification : Notification
    {
        public MembershipEndedNotification()
        {
            Type = NotificationType.MembershipEnded;
        }

        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string AppUrl { get; set; }
        public string EditPersonUrl { get; set; }

        public string Reason { get; set; }
    }

    public class ClassReminderLearnerNotification : Notification
    {
        public ClassReminderLearnerNotification()
        {
            Type = NotificationType.ClassReminderLearner;
        }

        public string EventTitle { get; set; }
        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public string VenueName { get; set; }
        public string VenueStreet1 { get; set; }
        public string VenueCity { get; set; }
        public string VenueProvince { get; set; }
        public string VenuePostalCode { get; set; }
        public string ClassURL { get; set; }
    }

    public class ClassReminderInstructorNotification : Notification
    {
        public ClassReminderInstructorNotification()
        {
            Type = NotificationType.ClassReminderInstructor;
        }

        public string EventTitle { get; set; }
        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public string VenueName { get; set; }
        public string VenueStreet1 { get; set; }
        public string VenueCity { get; set; }
        public string VenueProvince { get; set; }
        public string VenuePostalCode { get; set; }
        public string GradebookURL { get; set; }
    }

    public class CourseCompletedNotification : Notification
    {
        public CourseCompletedNotification()
        {
            Type = NotificationType.CourseCompleted;
        }

        public string AppUrl { get; set; }

        public string CourseName { get; set; }
        public string CourseStarted { get; set; }

        public Guid LearnerIdentifier { get; set; }
        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }
    }

    public class CourseStalledNotification : Notification
    {
        public CourseStalledNotification()
        {
            Type = NotificationType.CourseStalled;
        }

        public string AppUrl { get; set; }

        public string CourseName { get; set; }
        public string CourseStarted { get; set; }

        public Guid LearnerIdentifier { get; set; }
        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }
    }

    public class CredentialCreatedNotification : Notification
    {
        public CredentialCreatedNotification()
        {
            Type = NotificationType.CredentialCreated;
        }

        public string AchievementType { get; set; }
        public string AchievementName { get; set; }

        public Guid LearnerIdentifier { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }
    }

    public class BankNotification : Notification
    {
        public BankNotification()
        {
            Type = NotificationType.BankNotification;
        }

        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }
        public string LearnerPersonCode { get; set; }
        public string AssessmentFormName { get; set; }
        public string AssessmentAttemptScore { get; set; }
    }

    public class ProgramCompletedNotification : Notification
    {
        public ProgramCompletedNotification()
        {
            Type = NotificationType.ProgramCompleted;
        }

        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }
        public string ProgramName { get; set; }
        public string ProgramStarted { get; set; }
    }

    public class ProgramStalledNotification : Notification
    {
        public ProgramStalledNotification()
        {
            Type = NotificationType.ProgramStalled;
        }

        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }
        public string ProgramName { get; set; }
        public string ProgramStarted { get; set; }
    }

    public abstract class ResponseNotification : Notification
    {
        public string AppUrl { get; set; }
        public string UtcNow { get; set; }
        public string CurrentYear { get; set; }
        public string OrganizationName { get; set; }
        public string SurveyFormName { get; set; }
        public string Tenant { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        public string UserIdentifier { get; set; }
    }

    public class ResponseCompletedNotification : ResponseNotification
    {
        public ResponseCompletedNotification()
        {
            Type = NotificationType.ResponseCompleted;
        }
    }

    public class ResponseConfirmedNotification : ResponseNotification
    {
        public ResponseConfirmedNotification()
        {
            Type = NotificationType.ResponseConfirmed;
        }
    }

    public class ResponseStartedNotification : ResponseNotification
    {
        public ResponseStartedNotification()
        {
            Type = NotificationType.ResponseStarted;
        }
    }

    public class UserEmailSubscriptionModifiedNotification : Notification
    {
        public UserEmailSubscriptionModifiedNotification()
        {
            Type = NotificationType.UserEmailSubscriptionModified;
        }

        public string RecipientName { get; set; }
        public string RecipientMemberships { get; set; }
    }

    public class AlertUnsubscribeSuccess : Notification
    {
        public string UserEmail { get; set; }
        public string Organization { get; set; }
        public string ResubscribeUrl { get; set; }
    }

    public class AlertWelcomeLearner : Notification
    {
        public string AppUrl { get; set; }
        public string FirstName { get; set; }

        public string LoginUrl => AppUrl + "/ui/lobby/signin";
        public string DashboardUrl => AppUrl + "/ui/portal/learning/dashboard/home";
    }

    public class AlertLearningWelcomeEmail : Notification
    {
        public string AppUrl { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string LoginUrl => AppUrl + "/ui/lobby/signin";
        public string DashboardUrl => AppUrl + "/ui/portal/learning/dashboard/home";
    }
}