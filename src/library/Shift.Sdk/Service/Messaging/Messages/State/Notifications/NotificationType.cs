namespace InSite.Domain.Messages
{
    public enum NotificationType
    {
        AchievementCredentialsExpiredToday,
        AchievementCredentialsExpiringInOneMonth,
        AchievementCredentialsExpiringInThreeMonths,
        AchievementCredentialsExpiringInTwoMonths,
        AddedToWaitingList,
        AssessmentAttemptCompleted,
        AssessmentAttemptStarted,
        AuthenticationAlarmTriggered,
        CmdsBlogSubscriptionRequested,
        CmdsCollegeCertificationRequested,
        CmdsCompetenciesExpired,
        CmdsCompetencyChanged,
        CmdsCompetencyValidationRequested,
        CmdsResourceChanged,
        CmdsTrainingRegistrationSubmitted,

        ClassReminderLearner,
        ClassReminderInstructor,

        CourseCompleted,
        CourseStalled,

        CredentialCreated,

        EmployerGroupCreated,

        EmployerOpportunityApplicationSubmitted,

        EventVenueChanged,
        HelpRequested,
        
        JobsCandidateContactRequested,
        JobsCandidateAppliedForOpportunity,

        InvoicePaid,
        ClassScheduled,
        IssueOwnerChanged,

        ITA001,
        ITA003,
        ITA004,
        ITA005,
        ITA006,
        ITA007,
        ITA008,
        ITA009,
        ITA013,
        ITA014,
        ITA016,
        ITA017,
        ITA020,
        ITA021,
        ITA022,
        ITA023,
        ITA024,
        ITA025,
        ITA026,
        ITA027,

        LogbookModified,
        LogbookJoined,
        MembershipEnded,
        MembershipStarted,
        PasswordChanged,
        PasswordResetRequested,
        PersonCodeNotEntered,
        PersonCommentFlagged,

        ProgramCompleted,
        ProgramStalled,

        ProgressCompleted,
        RegistrantContactInformationChanged,
        RegistrationComplete,
        RegistrationInvitation,
        RegistrationInvitationExpired,
        
        ResponseCompleted,
        ResponseConfirmed,
        ResponseStarted,

        ApplicationAccessGranted,
        ApplicationAccessRequested,
        UnhandledExceptionIntercepted,
        UserAccountArchived,
        UserAccountCreated,
        UserAccountWelcomed,
        UserEmailSubscriptionModified,
        UserEmailVerificationRequested,
        UserOTPActivationCode,
        UserRegistrationSubmitted,
        UnsubscribeSuccess,
        BankNotification,
        WelcomeLearner,
        ManagementWelcomeEmail,
        LearningWelcomeEmail
    }
}