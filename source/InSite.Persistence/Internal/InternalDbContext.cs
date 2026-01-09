using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;
using Shift.Common.Timeline.Snapshots;

using InSite.Application;
using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Cases.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Events.Read;
using InSite.Application.Files.Read;
using InSite.Application.Glossaries.Read;
using InSite.Application.Invoices.Read;
using InSite.Application.Issues.Read;
using InSite.Application.Messages.Read;
using InSite.Application.Organizations.Read;
using InSite.Application.Payments.Read;
using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Resources.Read;
using InSite.Application.Sites.Read;
using InSite.Application.Standards.Read;
using InSite.Application.Surveys.Read;
using InSite.Domain.Integration;
using InSite.Persistence.Integration.BCMail;
using InSite.Persistence.Integration.DirectAccess;
using InSite.Persistence.Integration.Moodle;
using InSite.Persistence.Plugin.CMDS;
using InSite.Persistence.Plugin.NCSHA;

using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence
{
    internal interface ISaveHandler { void Before(ServiceIdentity identity, InternalDbContext context); }

    /// <summary>
    /// This is the database context through which the Persistence layer communicates with the SQL Server database. The
    /// access modifier for this class should be *internal* and not *public*, therefore we need to work on removing
    /// references to this class from the code in all other projects. (This will take some time to complete!)
    /// </summary>
    internal class InternalDbContext : DbContext
    {
        #region Properties (core entities)

        public DbSet<Address> Addresses { get; set; }
        public DbSet<ApiRequest> ApiRequests { get; set; }
        public DbSet<ArchivedFollower> ArchivedFollowers { get; set; }
        public DbSet<ArchivedSubscriber> ArchivedSubscribers { get; set; }
        public DbSet<TLearnerAttemptSummary> TLearnerAttemptSummaries { get; set; }
        public DbSet<CompanyDepartment> CompanyDepartments { get; set; }
        public DbSet<CompetencyValidationSummary> CompetencyValidationSummaries { get; set; }
        public DbSet<ContactExperience> ContactExperiences { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentProfileCompetency> DepartmentProfileCompetencies { get; set; }
        public DbSet<DepartmentProfileUser> DepartmentProfileUsers { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<DuplicateEmail> DuplicateEmails { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<GroupHierarchy> GroupHierarchies { get; set; }
        public DbSet<GroupToolkitPermissionSummary> GroupToolkitPermissionSummaries { get; set; }
        public DbSet<Impersonation> Impersonations { get; set; }
        public DbSet<ImpersonationSummary> ImpersonationSummaries { get; set; }
        public DbSet<LtiLaunch> LtiLaunchs { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Occupation> Occupations { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<QPerson> QPersons { get; set; }
        public DbSet<QPersonSecret> QPersonSecrets { get; set; }
        public DbSet<QPersonAddress> QPersonAddresses { get; set; }
        public DbSet<VDevPerson> VDevPersons { get; set; }
        public DbSet<QAccommodation> Accommodations { get; set; }
        public DbSet<QAchievement> QAchievements { get; set; }
        public DbSet<QAchievementPrerequisite> QAchievementPrerequisites { get; set; }
        public DbSet<QAreaRequirement> QAreaRequirements { get; set; }
        public DbSet<QAttempt> QAttempts { get; set; }
        public DbSet<QAttemptMatch> QAttemptMatches { get; set; }
        public DbSet<QAttemptOption> QAttemptOptions { get; set; }
        public DbSet<QAttemptQuestion> QAttemptQuestions { get; set; }
        public DbSet<QAttemptPin> QAttemptPins { get; set; }
        public DbSet<QAttemptSection> QAttemptSections { get; set; }
        public DbSet<QAttemptSolution> QAttemptSolutions { get; set; }
        public DbSet<VPerformanceReport> VPerformanceReports { get; set; }
        public DbSet<QBank> Banks { get; set; }
        public DbSet<QBankForm> BankForms { get; set; }
        public DbSet<QBankOption> BankOptions { get; set; }
        public DbSet<QBankQuestion> BankQuestions { get; set; }
        public DbSet<QBankQuestionGradeItem> BankQuestionGradeItems { get; set; }
        public DbSet<QBankQuestionAttachment> BankQuestionAttachments { get; set; }
        public DbSet<QBankQuestionSubCompetency> BankQuestionSubCompetencies { get; set; }
        public DbSet<QBankSpecification> BankSpecifications { get; set; }
        public DbSet<QClick> Clickthroughs { get; set; }
        public DbSet<QComment> QComments { get; set; }
        public DbSet<QCompetencyRequirement> QCompetencyRequirements { get; set; }
        public DbSet<QCredential> QCredentials { get; set; }
        public DbSet<QCredentialHistory> QCredentialHistories { get; set; }
        public DbSet<QRecipient> Recipients { get; set; }
        public DbSet<QEnrollment> QEnrollments { get; set; }
        public DbSet<QEnrollmentHistory> QEnrollmentHistories { get; set; }
        public DbSet<QEvent> Events { get; set; }
        public DbSet<QEventAttendee> EventAttendees { get; set; }
        public DbSet<QEventAssessmentForm> EventAssessmentForms { get; set; }
        public DbSet<QEventTimer> EventTimers { get; set; }
        public DbSet<QExperience> QExperiences { get; set; }
        public DbSet<QExperienceCompetency> QExperienceCompetencies { get; set; }
        public DbSet<QFollower> Followers { get; set; }
        public DbSet<QGlossaryTerm> GlossaryTerms { get; set; }
        public DbSet<QGlossaryTermContent> GlossaryTermContents { get; set; }
        public DbSet<QGradebook> QGradebooks { get; set; }
        public DbSet<QGradebookEvent> QGradebookEvents { get; set; }
        public DbSet<QGradeItem> QGradeItems { get; set; }
        public DbSet<QGradeItemCompetency> QGradeItemCompetencies { get; set; }
        public DbSet<QGroup> QGroups { get; set; }
        public DbSet<QGroupAddress> QGroupAddresses { get; set; }
        public DbSet<QGroupConnection> QGroupConnections { get; set; }
        public DbSet<QGroupTag> QGroupTags { get; set; }
        public DbSet<QInvoice> QInvoices { get; set; }
        public DbSet<QInvoiceItem> QInvoiceItems { get; set; }
        public DbSet<QIssue> QIssues { get; set; }
        public DbSet<QIssueAttachment> QIssueAttachments { get; set; }
        public DbSet<QIssueGroup> QIssueGroups { get; set; }
        public DbSet<QIssueFileRequirement> QIssueFileRequirements { get; set; }
        public DbSet<QIssueUser> QIssueUsers { get; set; }
        public DbSet<QJournal> QJournals { get; set; }
        public DbSet<QJournalSetup> QJournalSetups { get; set; }
        public DbSet<QJournalSetupField> QJournalSetupFields { get; set; }
        public DbSet<QJournalSetupUser> QJournalSetupUsers { get; set; }
        public DbSet<QJournalSetupGroup> QJournalSetupGroups { get; set; }
        public DbSet<QLearnerProgramSummary> QLearnerProgramSummaries { get; set; }
        public DbSet<QLink> Links { get; set; }
        public DbSet<QMailout> Mailouts { get; set; }
        public DbSet<QMembership> QMemberships { get; set; }
        public DbSet<QMembershipDeletion> QMembershipDeletions { get; set; }
        public DbSet<QMembershipReason> QMembershipReasons { get; set; }
        public DbSet<QMessage> Messages { get; set; }
        public DbSet<QOrganization> QOrganizations { get; set; }
        public DbSet<QPage> QPages { get; set; }
        public DbSet<QPayment> QPayments { get; set; }
        public DbSet<QPeriod> QPeriods { get; set; }
        public DbSet<QProgress> QProgresses { get; set; }
        public DbSet<QProgressHistory> QProgressHistories { get; set; }
        public DbSet<QRegistration> Registrations { get; set; }
        public DbSet<QRegistrationInstructor> RegistrationInstructors { get; set; }
        public DbSet<QRegistrationTimer> Timers { get; set; }
        public DbSet<QResponseAnswer> QResponseAnswers { get; set; }
        public DbSet<QResponseOption> QResponseOptions { get; set; }
        public DbSet<QResponseSession> QResponseSessions { get; set; }
        public DbSet<VResponse> VResponses { get; set; }
        public DbSet<VResponseFirstComment> VResponseFirstComments { get; set; }
        public DbSet<VResponseFirstSelection> VResponseFirstSelections { get; set; }
        public DbSet<QSeat> Seats { get; set; }
        public DbSet<QSite> QSites { get; set; }
        public DbSet<QStandard> QStandards { get; set; }
        public DbSet<QStandardAchievement> QStandardAchievements { get; set; }
        public DbSet<QStandardCategory> QStandardCategories { get; set; }
        public DbSet<QStandardConnection> QStandardConnections { get; set; }
        public DbSet<QStandardContainment> QStandardContainments { get; set; }
        public DbSet<QStandardGroup> QStandardGroups { get; set; }
        public DbSet<QStandardOrganization> QStandardOrganizations { get; set; }
        public DbSet<QStandardTier> QStandardTiers { get; set; }
        public DbSet<QSubscriberGroup> SubscriberGroups { get; set; }
        public DbSet<QSubscriberUser> SubscriberUsers { get; set; }
        public DbSet<QSurveyCondition> QSurveyConditions { get; set; }
        public DbSet<QSurveyForm> QSurveyForms { get; set; }
        public DbSet<QSurveyOptionItem> QSurveyOptionItems { get; set; }
        public DbSet<QSurveyOptionList> QSurveyOptionLists { get; set; }
        public DbSet<QSurveyQuestion> QSurveyQuestions { get; set; }
        public DbSet<QGradebookCompetencyValidation> QGradebookCompetencyValidations { get; set; }
        public DbSet<QStandardValidation> QStandardValidations { get; set; }
        public DbSet<QStandardValidationLog> QStandardValidationLogs { get; set; }
        public DbSet<ResourceCommentSummary> ResourceCommentSummaries { get; set; }
        public DbSet<RoleSummary> RoleSummaries { get; set; }
        public DbSet<SerializedAggregate> Aggregates { get; set; }
        public DbSet<SerializedChange> Changes { get; set; }
        public DbSet<SerializedCommand> Commands { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<StandardClassification> StandardClassifications { get; set; }
        public DbSet<StandardConnection> StandardConnections { get; set; }
        public DbSet<StandardContainment> StandardContainments { get; set; }
        public DbSet<StandardContainmentSummary> StandardContainmentSummaries { get; set; }
        public DbSet<StandardHierarchy> StandardHierarchies { get; set; }
        public DbSet<StandardOrganization> StandardOrganizations { get; set; }
        public DbSet<StandardValidation> StandardValidations { get; set; }
        public DbSet<StandardValidationChange> StandardValidationChanges { get; set; }
        public DbSet<TCatalog> TCatalogs { get; set; }
        public DbSet<TCourseCategory> TCourseCategories { get; set; }
        public DbSet<TAchievementCategory> TAchievementCategories { get; set; }
        public DbSet<VAchievementCategory> VAchievementCategories { get; set; }
        public DbSet<VAchievementClassification> VAchievementClassifications { get; set; }
        public DbSet<TAchievementDepartment> TAchievementDepartments { get; set; }
        public DbSet<TAchievementOrganization> TAchievementOrganizations { get; set; }
        public DbSet<TAchievementStandard> TAchievementStandards { get; set; }
        public DbSet<TAction> TActions { get; set; }
        public DbSet<TActivityCompetency> TActivityCompetencies { get; set; }
        public DbSet<TCertificateLayout> TCertificateLayouts { get; set; }
        public DbSet<TCollection> TCollections { get; set; }
        public DbSet<TCollectionItem> TCollectionItems { get; set; }
        public DbSet<TCollegeCertificate> TCollegeCertificates { get; set; }
        public DbSet<TContent> TContents { get; set; }
        public DbSet<TDepartmentStandard> TDepartmentStandards { get; set; }
        public DbSet<TDiscount> TDiscounts { get; set; }
        public DbSet<VOrganization> Organizations { get; set; }
        public DbSet<TCourseDistribution> TCourseDistributions { get; set; }
        public DbSet<TGroupPermission> TGroupPermissions { get; set; }
        public DbSet<TGroupSetting> TGroupSetting { get; set; }
        public DbSet<TCaseStatus> TCaseStatuses { get; set; }
        public DbSet<TLtiLink> LtiLinks { get; set; }
        public DbSet<TMeasurement> Measurements { get; set; }
        public DbSet<TPartitionSettingEntity> TPartitionSettings { get; set; }
        public DbSet<TPersonField> TPersonFields { get; set; }
        public DbSet<TPrerequisite> TPrerequisites { get; set; }
        public DbSet<TProduct> TProducts { get; set; }
        public DbSet<TOrder> TOrders { get; set; }
        public DbSet<TOrderItem> TOrderItems { get; set; }
        public DbSet<TProgram> TPrograms { get; set; }
        public DbSet<TProgramCategory> TProgramCategories { get; set; }
        public DbSet<TProgramEnrollment> TProgramEnrollments { get; set; }
        public DbSet<TProgramGroupEnrollment> TProgramGroupEnrollments { get; set; }
        public DbSet<TTax> TTaxes { get; set; }
        public DbSet<TReport> TReports { get; set; }
        public DbSet<QRubric> QRubrics { get; set; }
        public DbSet<QRubricCriterion> QRubricCriteria { get; set; }
        public DbSet<QRubricRating> QRubricRatings { get; set; }
        public DbSet<TMoodleEvent> TMoodleEvents { get; set; }
        public DbSet<TScormEvent> TScormEvents { get; set; }
        public DbSet<TScormRegistration> TScormRegistrations { get; set; }
        public DbSet<TScormRegistrationActivity> TScormRegistrationActivities { get; set; }
        public DbSet<TScormStatement> TScormStatements { get; set; }
        public DbSet<TSender> TSenders { get; set; }
        public DbSet<TSenderOrganization> TSenderOrganizations { get; set; }
        public DbSet<TTask> TTasks { get; set; }
        public DbSet<TTaskEnrollment> TTaskEnrollments { get; set; }
        public DbSet<TUserAuthenticationFactor> TUserAuthenticationFactors { get; set; }
        public DbSet<TUserSession> TUserSessions { get; set; }
        public DbSet<TUserSessionCache> TUserSessionCaches { get; set; }
        public DbSet<TUserSessionCacheSummary> SessionSummaries { get; set; }
        public DbSet<TUserSetting> TUserSettings { get; set; }
        public DbSet<Upgrade> Upgrades { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<UploadRelation> UploadRelations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<UserRegistrationDetail> UserRegistrationDetails { get; set; }
        public DbSet<QUser> QUsers { get; set; }
        public DbSet<QUserConnection> QUserConnections { get; set; }
        public DbSet<VAchievement> VAchievements { get; set; }
        public DbSet<VActivityCompetency> VActivityCompetencies { get; set; }
        public DbSet<VAddress> VAddresses { get; set; }
        public DbSet<VAssessmentPage> VAssessmentPages { get; set; }
        public DbSet<VAttendance> Attendances { get; set; }
        public DbSet<VBank> VBanks { get; set; }
        public DbSet<VClick> VClicks { get; set; }
        public DbSet<VComment> VComments { get; set; }
        public DbSet<VCompetency> VCompetencies { get; set; }
        public DbSet<VCourse> VCourses { get; set; }
        public DbSet<VCatalogCourse> VCatalogCourses { get; set; }
        public DbSet<VCatalogProgram> VCatalogPrograms { get; set; }
        public DbSet<VCredential> VCredentials { get; set; }
        public DbSet<VExamEventSchedule> VExamEventSchedules { get; set; }
        public DbSet<VFollower> XFollowers { get; set; }
        public DbSet<VFramework> Frameworks { get; set; }
        public DbSet<VGradeItemHierarchy> VGradeItemHierarchies { get; set; }
        public DbSet<VGroup> VGroups { get; set; }
        public DbSet<VGroupDetail> Groups { get; set; }
        public DbSet<VGroupEmployer> VGroupEmployers { get; set; }
        public DbSet<VInvoice> VInvoices { get; set; }
        public DbSet<VIssue> VIssues { get; set; }
        public DbSet<VIssueAttachment> VIssueAttachments { get; set; }
        public DbSet<VIssueFileRequirement> VIssueFileRequirements { get; set; }
        public DbSet<VIssueUser> VIssueUsers { get; set; }
        public DbSet<VJournalSetupUser> VJournalSetupUsers { get; set; }
        public DbSet<VLearnerActivity> VLearnerActivities { get; set; }
        public DbSet<VMailout> XMailouts { get; set; }
        public DbSet<VMembership> VMemberships { get; set; }
        public DbSet<VMessage> XMessages { get; set; }
        public DbSet<OrphanFile> VOrphanFiles { get; set; }
        public DbSet<VOrganizationGroupAddress> VOrganizationGroupAddresses { get; set; }
        public DbSet<VOrganizationPersonAddress> VOrganizationPersonAddresses { get; set; }
        public DbSet<VPerson> VPersons { get; set; }
        public DbSet<VProgram> VPrograms { get; set; }
        public DbSet<VProgramEnrollment> VProgramEnrollments { get; set; }
        public DbSet<VReport> VReports { get; set; }
        public DbSet<VStandard> VStandards { get; set; }
        public DbSet<VStatement> VStatements { get; set; }
        public DbSet<VSubscriberGroup> VSubscriberGroups { get; set; }
        public DbSet<VSurveyResponseAnswer> VSurveyResponseAnswers { get; set; }
        public DbSet<VSurveyResponseSummary> SurveyResponseSummaries { get; set; }
        public DbSet<VTopStudent> VTopStudents { get; set; }
        public DbSet<VUpload> VUploads { get; set; }
        public DbSet<VUser> VUsers { get; set; }
        public DbSet<VUserSession> VUserSessions { get; set; }
        public DbSet<VWebPageHierarchy> VWebPageHierarchies { get; set; }
        public DbSet<XRegistrationTimer> XTimers { get; set; }
        public DbSet<XSubscriberPerson> XSubscriberPersons { get; set; }
        public DbSet<XSubscriberUser> XSubscriberUsers { get; set; }
        public DbSet<TFile> TFiles { get; set; }
        public DbSet<TFileActivity> TFileActivities { get; set; }
        public DbSet<TFileClaim> TFileClaims { get; set; }
        public DbSet<TQuiz> TQuizzes { get; set; }
        public DbSet<TQuizAttempt> TQuizAttempts { get; set; }
        public DbSet<QActivityCompetency> QActivityCompetencies { get; set; }
        public DbSet<QActivity> QActivities { get; set; }
        public DbSet<QCourse> QCourses { get; set; }
        public DbSet<QCourseEnrollment> QCourseEnrollments { get; set; }
        public DbSet<QCoursePrerequisite> QCoursePrerequisites { get; set; }
        public DbSet<QModule> QModules { get; set; }
        public DbSet<QUnit> QUnits { get; set; }

        #endregion

        #region Properties (custom entities)

        // CMDS

        public DbSet<ActiveUser> ActiveUsers { get; set; }
        public DbSet<BillableUser> BillableUsers { get; set; }
        public DbSet<BillableUserSummary> BillableUserSummaries { get; set; }
        public DbSet<VCmdsCompetencyOrganization> VCmdsCompetencyOrganizations { get; set; }
        public DbSet<VCmdsProfileOrganization> VCmdsProfileOrganizations { get; set; }
        public DbSet<VCmdsAchievementOrganization> VCmdsAchievementOrganizations { get; set; }
        public DbSet<CompetencyCategory> CompetencyCategories { get; set; }
        public DbSet<Competency> Competencies { get; set; }
        public DbSet<CreditDetail> CreditDetails { get; set; }
        public DbSet<CmdsDepartmentProfileCompetency> CmdsDepartmentProfileCompetencies { get; set; }
        public DbSet<DepartmentProfile> DepartmentProfiles { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<PermissionList> PermissionLists { get; set; }
        public DbSet<ProfileCertification> ProfileCertifications { get; set; }
        public DbSet<ProfileCompetency> ProfileCompetencies { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<VCmdsAchievementCategory> VCmdsAchievementCategories { get; set; }
        public DbSet<VCmdsAchievementCompetency> VCmdsAchievementCompetencies { get; set; }
        public DbSet<VCmdsAchievementDepartment> VCmdsAchievementDepartments { get; set; }
        public DbSet<VCmdsAchievement> VCmdsAchievements { get; set; }
        public DbSet<UserCompetency> UserCompetencies { get; set; }
        public DbSet<UserCompetencyExpiration> UserCompetencyExpirations { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<VCmdsCredentialAndExperience> VCmdsCredentialAndExperiences { get; set; }
        public DbSet<VCmdsCredential> VCmdsCredentials { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        // CMDS Temp

        public DbSet<QInvoiceFee> QInvoiceFees { get; set; }
        public DbSet<TUserStatus> TUserStatuses { get; set; }
        public DbSet<ZUserStatus> ZUserStatuses { get; set; }

        // SkilledTradesBC

        public DbSet<ExamDistributionRequest> ExamDistributionRequests { get; set; }
        public DbSet<Individual> Individuals { get; set; }

        // NCSHA

        public DbSet<AbProgram> AbPrograms { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Filter> Filters { get; set; }
        public DbSet<HcProgram> HcPrograms { get; set; }
        public DbSet<HiProgram> HiPrograms { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<MfProgram> MfPrograms { get; set; }
        public DbSet<MrProgram> MrPrograms { get; set; }
        public DbSet<NcshaProgram> Programs { get; set; }
        public DbSet<PaProgram> PaPrograms { get; set; }
        public DbSet<ProgramFolder> ProgramFolders { get; set; }
        public DbSet<ProgramFolderComment> ProgramFolderComments { get; set; }

        #endregion

        #region Methods (entity configuration)

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);
            AddConfigurations(builder);
        }

        public static void AddConfigurations(DbModelBuilder builder)
        {
            // Functions

            builder.Conventions.Add(new CoreFunctionsConvention());
            builder.Conventions.Add(new JsonFunctionsConvention());

            // Tables and Views

            builder.Configurations.Add(new AddressConfiguration());
            builder.Configurations.Add(new AggregateConfiguration());
            builder.Configurations.Add(new ApiRequestConfiguration());
            builder.Configurations.Add(new ArchivedFollowerConfiguration());
            builder.Configurations.Add(new ArchivedSubscriberConfiguration());
            builder.Configurations.Add(new TLearnerAttemptSummaryConfiguration());
            builder.Configurations.Add(new ChangeConfiguration());
            builder.Configurations.Add(new CommandConfiguration());
            builder.Configurations.Add(new CompanyDepartmentConfiguration());
            builder.Configurations.Add(new CompetencyValidationSummaryConfiguration());
            builder.Configurations.Add(new ContactExperienceConfiguration());
            builder.Configurations.Add(new DepartmentConfiguration());
            builder.Configurations.Add(new DepartmentProfileCompetencyConfiguration());
            builder.Configurations.Add(new DepartmentProfileUserConfiguration());
            builder.Configurations.Add(new DivisionConfiguration());
            builder.Configurations.Add(new DuplicateEmailConfiguration());
            builder.Configurations.Add(new EmployeeConfiguration());
            builder.Configurations.Add(new EmployerConfiguration());
            builder.Configurations.Add(new GroupHierarchyConfiguration());
            builder.Configurations.Add(new GroupToolkitPermissionSummaryConfiguration());
            builder.Configurations.Add(new ImpersonationConfiguration());
            builder.Configurations.Add(new ImpersonationSummaryConfiguration());
            builder.Configurations.Add(new LtiLaunchConfiguration());
            builder.Configurations.Add(new MembershipConfiguration());
            builder.Configurations.Add(new OccupationConfiguration());
            builder.Configurations.Add(new PersonConfiguration());
            builder.Configurations.Add(new QPersonConfiguration());
            builder.Configurations.Add(new QPersonSecretConfiguration());
            builder.Configurations.Add(new QPersonAddressConfiguration());
            builder.Configurations.Add(new VDevPersonConfiguration());
            builder.Configurations.Add(new QAccommodationConfiguration());
            builder.Configurations.Add(new QAchievementConfiguration());
            builder.Configurations.Add(new QAchievementPrerequisiteConfiguration());
            builder.Configurations.Add(new QAreaRequirementConfiguration());
            builder.Configurations.Add(new QAttemptConfiguration());
            builder.Configurations.Add(new QAttemptMatchConfiguration());
            builder.Configurations.Add(new QAttemptOptionConfiguration());
            builder.Configurations.Add(new QAttemptQuestionConfiguration());
            builder.Configurations.Add(new QAttemptSectionConfiguration());
            builder.Configurations.Add(new QAttemptPinConfiguration());
            builder.Configurations.Add(new QAttemptSolutionConfiguration());
            builder.Configurations.Add(new QBankConfiguration());
            builder.Configurations.Add(new QBankFormConfiguration());
            builder.Configurations.Add(new QBankOptionConfiguration());
            builder.Configurations.Add(new QBankQuestionAttachmentConfiguration());
            builder.Configurations.Add(new QBankQuestionConfiguration());
            builder.Configurations.Add(new QBankQuestionGradeItemConfiguration());
            builder.Configurations.Add(new QBankQuestionSubCompetencyConfiguration());
            builder.Configurations.Add(new QBankSpecificationConfiguration());
            builder.Configurations.Add(new QCarbonCopyConfiguration());
            builder.Configurations.Add(new QClickConfiguration());
            builder.Configurations.Add(new QCommentConfiguration());
            builder.Configurations.Add(new QCompetencyRequirementConfiguration());
            builder.Configurations.Add(new QCredentialConfiguration());
            builder.Configurations.Add(new QCredentialHistoryConfiguration());
            builder.Configurations.Add(new QRecipientConfiguration());
            builder.Configurations.Add(new QEnrollmentConfiguration());
            builder.Configurations.Add(new QEnrollmentHistoryConfiguration());
            builder.Configurations.Add(new QEventAttendeeConfiguration());
            builder.Configurations.Add(new QEventConfiguration());
            builder.Configurations.Add(new QEventTimerConfiguration());
            builder.Configurations.Add(new QEventAssessmentFormConfiguration());
            builder.Configurations.Add(new QExperienceCompetencyConfiguration());
            builder.Configurations.Add(new QExperienceConfiguration());
            builder.Configurations.Add(new QFollowerConfiguration());
            builder.Configurations.Add(new QGlossaryTermConfiguration());
            builder.Configurations.Add(new QGlossaryTermContentConfiguration());
            builder.Configurations.Add(new QGradebookConfiguration());
            builder.Configurations.Add(new QGradebookEventConfiguration());
            builder.Configurations.Add(new QGradeItemCompetencyConfiguration());
            builder.Configurations.Add(new QGradeItemConfiguration());
            builder.Configurations.Add(new QGroupAddressConfiguration());
            builder.Configurations.Add(new QGroupConfiguration());
            builder.Configurations.Add(new QGroupConnectionConfiguration());
            builder.Configurations.Add(new QGroupTagConfiguration());
            builder.Configurations.Add(new QInvoiceConfiguration());
            builder.Configurations.Add(new QInvoiceItemConfiguration());
            builder.Configurations.Add(new QIssueAttachmentConfiguration());
            builder.Configurations.Add(new QIssueConfiguration());
            builder.Configurations.Add(new QIssueGroupConfiguration());
            builder.Configurations.Add(new QIssueFileRequirementConfiguration());
            builder.Configurations.Add(new QIssueUserConfiguration());
            builder.Configurations.Add(new QJournalConfiguration());
            builder.Configurations.Add(new QJournalSetupConfiguration());
            builder.Configurations.Add(new QJournalSetupFieldConfiguration());
            builder.Configurations.Add(new QJournalSetupGroupConfiguration());
            builder.Configurations.Add(new QJournalSetupUserConfiguration());
            builder.Configurations.Add(new QLearnerProgramSummaryConfiguration());
            builder.Configurations.Add(new QLinkConfiguration());
            builder.Configurations.Add(new QMailoutConfiguration());
            builder.Configurations.Add(new QMembershipConfiguration());
            builder.Configurations.Add(new QMembershipDeletionConfiguration());
            builder.Configurations.Add(new QMembershipReasonConfiguration());
            builder.Configurations.Add(new QMessageConfiguration());
            builder.Configurations.Add(new QPageConfiguration());
            builder.Configurations.Add(new QPaymentConfiguration());
            builder.Configurations.Add(new QPeriodConfiguration());
            builder.Configurations.Add(new QProgressConfiguration());
            builder.Configurations.Add(new QProgressHistoryConfiguration());
            builder.Configurations.Add(new QRegistrationConfiguration());
            builder.Configurations.Add(new QRegistrationInstructorConfiguration());
            builder.Configurations.Add(new QRegistrationTimerConfiguration());
            builder.Configurations.Add(new QResponseAnswerConfiguration());
            builder.Configurations.Add(new QResponseOptionConfiguration());
            builder.Configurations.Add(new QResponseSessionConfiguration());
            builder.Entity<QResponseSession>().Ignore(r => r.FirstCommentText);
            builder.Entity<QResponseSession>().Ignore(r => r.FirstSelectionText);
            builder.Entity<QResponseSession>().Ignore(r => r.GroupName);
            builder.Entity<QResponseSession>().Ignore(r => r.PeriodName);
            builder.Entity<QResponseSession>().Ignore(r => r.RespondentEmail);
            builder.Entity<QResponseSession>().Ignore(r => r.RespondentName);
            builder.Entity<QResponseSession>().Ignore(r => r.RespondentNameFirst);
            builder.Entity<QResponseSession>().Ignore(r => r.RespondentNameLast);
            builder.Entity<QResponseSession>().Ignore(r => r.SurveyIsConfidential);
            builder.Entity<QResponseSession>().Ignore(r => r.SurveyName);
            builder.Entity<QResponseSession>().Ignore(r => r.SurveyNumber);
            builder.Configurations.Add(new VResponseConfiguration());
            builder.Configurations.Add(new VResponseFirstCommentConfiguration());
            builder.Configurations.Add(new VResponseFirstSelectionConfiguration());
            builder.Configurations.Add(new QSeatConfiguration());
            builder.Configurations.Add(new QSiteConfigruation());
            builder.Configurations.Add(new QStandardConfiguration());
            builder.Configurations.Add(new QStandardAchievementConfiguration());
            builder.Configurations.Add(new QStandardCategoryConfiguration());
            builder.Configurations.Add(new QStandardConnectionConfiguration());
            builder.Configurations.Add(new QStandardContainmentConfiguration());
            builder.Configurations.Add(new QStandardGroupConfiguration());
            builder.Configurations.Add(new QStandardOrganizationConfiguration());
            builder.Configurations.Add(new QStandardTierConfiguration());
            builder.Configurations.Add(new QSubscriberGroupConfiguration());
            builder.Configurations.Add(new QSubscriberUserConfiguration());
            builder.Configurations.Add(new QSurveyConditionConfiguration());
            builder.Configurations.Add(new QSurveyFormConfiguration());
            builder.Configurations.Add(new QSurveyOptionItemConfiguration());
            builder.Configurations.Add(new QSurveyOptionListConfiguration());
            builder.Configurations.Add(new QSurveyQuestionConfiguration());
            builder.Configurations.Add(new QOrganizationConfiguration());
            builder.Configurations.Add(new QGradebookCompetencyValidationConfiguration());
            builder.Configurations.Add(new QStandardValidationConfiguration());
            builder.Configurations.Add(new QStandardValidationLogConfiguration());
            builder.Configurations.Add(new ResourceCommentSummaryConfiguration());
            builder.Configurations.Add(new RoleSummaryConfiguration());
            builder.Configurations.Add(new SnapshotConfiguration());
            builder.Configurations.Add(new StandardClassificationConfiguration());
            builder.Configurations.Add(new StandardConfiguration());
            builder.Configurations.Add(new StandardConnectionConfiguration());
            builder.Configurations.Add(new StandardContainmentConfiguration());
            builder.Configurations.Add(new StandardContainmentSummaryConfiguration());
            builder.Configurations.Add(new StandardHierarchyConfiguration());
            builder.Configurations.Add(new StandardOrganizationConfiguration());
            builder.Configurations.Add(new StandardValidationChangeConfiguration());
            builder.Configurations.Add(new StandardValidationConfiguration());
            builder.Configurations.Add(new TCatalogConfiguration());
            builder.Configurations.Add(new TCourseDistributionConfiguration());
            builder.Configurations.Add(new TCourseCategoryConfiguration());
            builder.Configurations.Add(new TAchievementCategoryConfiguration());
            builder.Configurations.Add(new VStatementConfiguration());
            builder.Configurations.Add(new VAchievementCategoryConfiguration());
            builder.Configurations.Add(new VAchievementClassificationConfiguration());
            builder.Configurations.Add(new TAchievementDepartmentConfiguration());
            builder.Configurations.Add(new TAchievementStandardConfiguration());
            builder.Configurations.Add(new TAchievementOrganizationConfiguration());
            builder.Configurations.Add(new TActionConfiguration());
            builder.Configurations.Add(new TActivityCompetencyConfiguration());
            builder.Configurations.Add(new TCertificateLayoutConfiguration());
            builder.Configurations.Add(new TCollectionConfiguration());
            builder.Configurations.Add(new TCollectionItemConfiguration());
            builder.Configurations.Add(new TCollegeCertificateConfiguration());
            builder.Configurations.Add(new TContentConfiguration());
            builder.Configurations.Add(new TDepartmentConfiguration());
            builder.Configurations.Add(new TDepartmentStandardConfiguration());
            builder.Configurations.Add(new TDiscountConfiguration());
            builder.Configurations.Add(new TFileConfiguration());
            builder.Configurations.Add(new TFileActivityConfiguration());
            builder.Configurations.Add(new TFileClaimConfiguration());
            builder.Configurations.Add(new TGroupPermissionConfiguration());
            builder.Configurations.Add(new VEventGroupPermissionConfiguration());
            builder.Configurations.Add(new TGroupSettingConfiguration());
            builder.Configurations.Add(new TCaseStatusConfiguration());
            builder.Configurations.Add(new TLtiLinkConfiguration());
            builder.Configurations.Add(new TMeasurementConfiguration());
            builder.Configurations.Add(new TPartitionSettingConfiguration());
            builder.Configurations.Add(new TPersonFieldConfiguration());
            builder.Configurations.Add(new TPrerequisiteConfiguration());
            builder.Configurations.Add(new TProductConfiguration());
            builder.Configurations.Add(new TOrderConfiguration());
            builder.Configurations.Add(new TOrderItemConfiguration());
            builder.Configurations.Add(new TProgramConfiguration());
            builder.Configurations.Add(new TProgramCategoryConfiguration());
            builder.Configurations.Add(new TProgramEnrollmentConfiguration());
            builder.Configurations.Add(new TProgramGroupEnrollmentConfiguration());
            builder.Configurations.Add(new TTaxConfiguration());
            builder.Configurations.Add(new TQuizConfiguration());
            builder.Configurations.Add(new TQuizAttemptConfiguration());
            builder.Configurations.Add(new TReportConfiguration());
            builder.Configurations.Add(new QRubricConfiguration());
            builder.Configurations.Add(new QRubricCriterionConfiguration());
            builder.Configurations.Add(new QRubricRatingConfiguration());
            builder.Configurations.Add(new TMoodleEventConfiguration());
            builder.Configurations.Add(new TScormEventConfiguration());
            builder.Configurations.Add(new TScormRegistrationConfiguration());
            builder.Configurations.Add(new TScormRegistrationActivityConfiguration());
            builder.Configurations.Add(new TScormStatementConfiguration());
            builder.Configurations.Add(new TSenderConfiguration());
            builder.Configurations.Add(new TSenderOrganizationConfiguration());
            builder.Configurations.Add(new TTaskConfiguration());
            builder.Configurations.Add(new TTaskEnrollmentConfiguration());
            builder.Configurations.Add(new TUserAuthenticationFactorConfiguration());
            builder.Configurations.Add(new TUserSessionCacheConfiguration());
            builder.Configurations.Add(new TUserSessionCacheSummaryConfiguration());
            builder.Configurations.Add(new TUserSessionConfiguration());
            builder.Configurations.Add(new TUserSettingConfiguration());
            builder.Configurations.Add(new UpgradeConfiguration());
            builder.Configurations.Add(new UploadConfiguration());
            builder.Configurations.Add(new UploadRelationConfiguration());
            builder.Configurations.Add(new UserConfiguration());
            builder.Configurations.Add(new UserConnectionConfiguration());
            builder.Configurations.Add(new QUserConfiguration());
            builder.Configurations.Add(new QUserConnectionConfiguration());
            builder.Configurations.Add(new UserRegistrationDetailConfiguration());
            builder.Configurations.Add(new VAchievementConfiguration());
            builder.Configurations.Add(new VActivityCompetencyConfiguration());
            builder.Configurations.Add(new VAddressConfiguration());
            builder.Configurations.Add(new VAssessmentPageConfiguration());
            builder.Configurations.Add(new VAttendanceConfiguration());
            builder.Configurations.Add(new VBankConfiguration());
            builder.Configurations.Add(new VClickConfiguration());
            builder.Configurations.Add(new VCommentConfiguration());
            builder.Configurations.Add(new VCompetencyConfiguration());
            builder.Configurations.Add(new VCatalogCourseConfiguration());
            builder.Configurations.Add(new VCatalogProgramConfiguration());
            builder.Configurations.Add(new VCredentialConfiguration());
            builder.Configurations.Add(new VExamEventScheduleConfiguration());
            builder.Configurations.Add(new VFrameworkConfiguration());
            builder.Configurations.Add(new VGradeItemHierarchyConfiguration());
            builder.Configurations.Add(new VGroup2Configuration());
            builder.Configurations.Add(new VGroupConfiguration());
            builder.Configurations.Add(new VGroupEmployerConfiguration());
            builder.Configurations.Add(new VInvoiceConfiguration());
            builder.Configurations.Add(new VIssueAttachmentConfiguration());
            builder.Configurations.Add(new VIssueConfiguration());
            builder.Configurations.Add(new VIssueFileRequirementConfiguration());
            builder.Configurations.Add(new VIssueUserConfiguration());
            builder.Configurations.Add(new VJournalSetupUserConfiguration());
            builder.Configurations.Add(new VLearnerActivityConfiguration());
            builder.Configurations.Add(new VMembershipConfiguration());
            builder.Configurations.Add(new VOrphanFileConfiguration());
            builder.Configurations.Add(new VOrganizationConfiguration());
            builder.Configurations.Add(new VPerformanceReportConfiguration());
            builder.Configurations.Add(new VPersonConfiguration());
            builder.Configurations.Add(new VProgramConfiguration());
            builder.Configurations.Add(new VProgramEnrollmentConfiguration());
            builder.Configurations.Add(new VReportConfiguration());
            builder.Configurations.Add(new VStandardConfiguration());
            builder.Configurations.Add(new VSubscriberGroupConfiguration());
            builder.Configurations.Add(new VSurveyResponseAnswerConfiguration());
            builder.Configurations.Add(new VSurveyResponseSummaryConfiguration());
            builder.Configurations.Add(new VOrganizationGroupAddressConfiguration());
            builder.Configurations.Add(new VOrganizationPersonAddressConfiguration());
            builder.Configurations.Add(new VTopStudentConfiguration());
            builder.Configurations.Add(new VUploadConfiguration());
            builder.Configurations.Add(new VUserConfiguration());
            builder.Configurations.Add(new VUserSessionConfiguration());
            builder.Configurations.Add(new VWebPageHierarchyConfiguration());
            builder.Configurations.Add(new XFollowerConfiguration());
            builder.Configurations.Add(new XMailoutConfiguration());
            builder.Configurations.Add(new XMessageConfiguration());
            builder.Configurations.Add(new XRegistrationTimerConfiguration());
            builder.Configurations.Add(new XSubscriberPersonConfiguration());
            builder.Configurations.Add(new XSubscriberUserConfiguration());
            builder.Configurations.Add(new QActivityCompetencyConfiguration());
            builder.Configurations.Add(new QActivityConfiguration());
            builder.Configurations.Add(new QCourseConfiguration());
            builder.Configurations.Add(new QCourseEnrollmentConfiguration());
            builder.Configurations.Add(new QCoursePrerequisiteConfiguration());
            builder.Configurations.Add(new QModuleConfiguration());
            builder.Configurations.Add(new QUnitConfiguration());
            builder.Configurations.Add(new VCourseConfiguration());

            #region Custom

            // CMDS

            builder.Configurations.Add(new ActiveUserConfiguration());
            builder.Configurations.Add(new BillableUserConfiguration());
            builder.Configurations.Add(new BillableUserSummaryConfiguration());
            builder.Configurations.Add(new CmdsDepartmentProfileCompetencyConfiguration());
            builder.Configurations.Add(new CompetencyCategoryConfiguration());
            builder.Configurations.Add(new CompetencyConfiguration());
            builder.Configurations.Add(new CreditDetailConfiguration());
            builder.Configurations.Add(new DepartmentProfileConfiguration());
            builder.Configurations.Add(new EmploymentConfiguration());
            builder.Configurations.Add(new PermissionListConfiguration());
            builder.Configurations.Add(new ProfileCertificationConfiguration());
            builder.Configurations.Add(new ProfileCompetencyConfiguration());
            builder.Configurations.Add(new ProfileConfiguration());
            builder.Configurations.Add(new UserCompetencyConfiguration());
            builder.Configurations.Add(new UserCompetencyExpirationConfiguration());
            builder.Configurations.Add(new UserProfileConfiguration());
            builder.Configurations.Add(new UserRoleConfiguration());
            builder.Configurations.Add(new VCmdsAchievementConfiguration());
            builder.Configurations.Add(new VCmdsAchievementCategoryConfiguration());
            builder.Configurations.Add(new VCmdsAchievementCompetencyConfiguration());
            builder.Configurations.Add(new VCmdsAchievementDepartmentConfiguration());
            builder.Configurations.Add(new VCmdsAchievementOrganizationConfiguration());
            builder.Configurations.Add(new VCmdsCompetencyOrganizationConfiguration());
            builder.Configurations.Add(new VCmdsCredentialConfiguration());
            builder.Configurations.Add(new VCmdsCredentialAndExperienceConfiguration());
            builder.Configurations.Add(new VCmdsProfileOrganizationConfiguration());

            // CMDS Temp

            builder.Configurations.Add(new QInvoiceFeeConfiguration());
            builder.Configurations.Add(new TUserStatusConfiguration());
            builder.Configurations.Add(new ZUserStatusConfiguration());

            // SkilledTradesBC

            builder.Configurations.Add(new ExamDistributionRequestConfiguration());
            builder.Configurations.Add(new IndividualConfiguration());

            // NCSHA

            builder.Configurations.Add(new AbProgramMapping());
            builder.Configurations.Add(new CounterMapping());
            builder.Configurations.Add(new FieldMapping());
            builder.Configurations.Add(new FilterMapping());
            builder.Configurations.Add(new HcProgramMapping());
            builder.Configurations.Add(new HiProgramMapping());
            builder.Configurations.Add(new HistoryMapping());
            builder.Configurations.Add(new MfProgramMapping());
            builder.Configurations.Add(new MrProgramMapping());
            builder.Configurations.Add(new NcshaProgramMapping());
            builder.Configurations.Add(new PaProgramMapping());
            builder.Configurations.Add(new ProgramFolderCommentMapping());
            builder.Configurations.Add(new ProgramFolderMapping());

            #endregion
        }

        #endregion

        #region Methods (construction)

        static InternalDbContext()
        {
            Database.SetInitializer<InternalDbContext>(null);
        }

        public InternalDbContext()
            : base(DbSettings.ConnectionString)
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public InternalDbContext(bool proxy, bool lazy = true)
            : base(DbSettings.ConnectionString)
        {
            Configuration.ProxyCreationEnabled = proxy;
            Configuration.LazyLoadingEnabled = lazy;
        }

        public InternalDbContext(string databaseName) : base(databaseName) { }

        #endregion

        #region Methods (queries)

        public static int? GetMaxLength<T>(Expression<Func<T, string>> column)
        {
            int? result = null;
            using (var context = new InternalDbContext())
            {
                var entType = typeof(T);
                var columnName = ((MemberExpression)column.Body).Member.Name;

                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var test = objectContext.MetadataWorkspace.GetItems(DataSpace.CSpace);

                if (test == null)
                    return null;

                var q = test
                    .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    .SelectMany(meta => ((EntityType)meta).Properties
                        .Where(p => p.Name == columnName && p.TypeUsage.EdmType.Name == "String"));

                var queryResult = q.Where(p =>
                {
                    var match = p.DeclaringType.Name == entType.Name;
                    if (!match)
                        match = entType.Name == p.DeclaringType.Name;

                    return match;
                })
                    .Select(sel => sel.TypeUsage.Facets["MaxLength"].Value)
                    .ToList();

                if (queryResult.Any())
                    result = Convert.ToInt32(queryResult.First());

                return result;
            }
        }

        public DbRawSqlQuery<T> SqlQuery<T>(string query)
        {
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<T>(query);
            }
        }

        #endregion

        #region Methods (saving changes)

        private readonly DbEntityCustodian _custodian = new DbEntityCustodian();

        public bool EnablePrepareToSaveChanges { get; set; } = true;

        private readonly SaveHandler _saveHandler = new SaveHandler();

        private static Func<ServiceIdentity> _identification;

        public static void SetIdentification(Func<ServiceIdentity> identification)
        {
            _identification = identification;
        }

        public event EventHandler ChangesSaved;

        private int Save()
        {
            if (DbSettings.IsReadOnly)
                return 0;

            if (_identification != null)
                _saveHandler.Before(_identification(), this);

            var triesCount = 3;

            while (true)
            {
                try
                {
                    return base.SaveChanges();
                }
                catch (DbUpdateConcurrencyException cex)
                {
                    foreach (var entry in cex.Entries)
                    {
                        if (entry.State == EntityState.Deleted)
                            entry.State = EntityState.Detached;
                        else
                            throw;
                    }

                    triesCount--;

                    if (triesCount == 0)
                        throw;
                }
            }
        }

        public override int SaveChanges()
        {
            if (DbSettings.IsReadOnly)
                return 0;

            try
            {
                if (EnablePrepareToSaveChanges)
                    _custodian.PrepareToSaveChanges(this);

                var result = Save();

                OnChangesSaved();

                return result;
            }
            catch (DbEntityValidationException e)
            {
                throw new DbEntityException(e);
            }
        }

        private void OnChangesSaved() => ChangesSaved?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}