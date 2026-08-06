using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Contract;
using Shift.Service.Assessment;
using Shift.Service.Booking;
using Shift.Service.Cases;
using Shift.Service.Competency;
using Shift.Service.Content;
using Shift.Service.Directory;
using Shift.Service.Evaluation;
using Shift.Service.Messaging;
using Shift.Service.Learning;
using Shift.Service.Metadata;
using Shift.Service.Progress;
using Shift.Service.Security;
using Shift.Service.Setup;
using Shift.Service.Timeline;
using Shift.Service.Utility;
using Shift.Service.Workflow;
using Shift.Service.Workspace;
using Shift.Service.Reports;

namespace Shift.Service;

public class TableDbContext : DbContext
{
    public TableDbContext(DbContextOptions options) : base(options) { }

    #region Storage Tables

    // Domain: Assessment
    internal DbSet<AssessmentEntity> Assessment { get; set; }
    internal DbSet<AttemptEntity> Attempt { get; set; }
    internal DbSet<BankEntity> Bank { get; set; }
    internal DbSet<BankQuestionEntity> BankQuestion { get; set; }
    internal DbSet<BankSpecificationEntity> BankSpecification { get; set; }

    // Domain: Booking
    internal DbSet<EventEntity> QEvent { get; set; }
    internal DbSet<EventUserEntity> QEventUser { get; set; }
    internal DbSet<RegistrationEntity> QRegistration { get; set; }

    // Domain: Competency
    internal DbSet<StandardEntity> QStandard { get; set; }

    // Domain: Contact
    internal DbSet<AddressEntity> QPersonAddress { get; set; }
    internal DbSet<GroupEntity> QGroup { get; set; }
    internal DbSet<MembershipEntity> QMembership { get; set; }
    internal DbSet<MembershipDeletionEntity> QMembershipDeletion { get; set; }
    internal DbSet<PersonEntity> QPerson { get; set; }
    internal DbSet<QPersonSecretEntity> QPersonSecret { get; set; }

    // Domain: Content
    internal DbSet<FileActivityEntity> TFileActivity { get; set; }
    internal DbSet<FileClaimEntity> TFileClaim { get; set; }
    internal DbSet<FileEntity> TFile { get; set; }
    internal DbSet<TInputEntity> TInput { get; set; }
    internal DbSet<UploadEntity> Upload { get; set; }

    // Feature: Messaging
    internal DbSet<MailoutEntity> Mailout { get; set; }
    internal DbSet<RecipientEntity> Recipient { get; set; }

    // Domain: Directory
    internal DbSet<PendingPersonEntity> PendingPerson { get; set; }

    // Domain: Learning
    internal DbSet<CourseEntity> Course { get; set; }
    internal DbSet<CourseEnrollmentEntity> CourseEnrollment { get; set; }
    internal DbSet<CourseMatch> CourseMatch { get; set; }

    // Domain: Progress
    internal DbSet<AchievementEntity> QAchievement { get; set; }
    internal DbSet<CredentialEntity> QCredential { get; set; }
    internal DbSet<GradebookEntity> QGradebook { get; set; }
    internal DbSet<QGradebookEnrollmentEntity> QGradebookEnrollment { get; set; }
    internal DbSet<PeriodEntity> QPeriod { get; set; }

    // Domain: Reports
    internal DbSet<ToolkitVisitEntity> TToolkitVisit { get; set; }
    internal DbSet<ToolkitUsageEntity> TToolkitUsage { get; set; }

    // Domain: Workflow
    internal DbSet<CaseDocumentEntity> QCaseDocument { get; set; }
    internal DbSet<CaseDocumentRequestEntity> QCaseDocumentRequest { get; set; }
    internal DbSet<CaseEntity> QCase { get; set; }
    internal DbSet<CaseGroupEntity> QCaseGroup { get; set; }
    internal DbSet<CaseUserEntity> QCaseUser { get; set; }
    internal DbSet<TCaseStatusEntity> TCaseStatus { get; set; }
    internal DbSet<FormEntity> Form { get; set; }
    internal DbSet<SubmissionEntity> Submission { get; set; }

    // Domain: Workspace
    internal DbSet<PageEntity> QPage { get; set; }
    internal DbSet<SiteEntity> QSite { get; set; }

    // Utility: Metadata
    internal DbSet<TActionEntity> TAction { get; set; }

    // Utility: Security
    internal DbSet<OrganizationEntity> Organization { get; set; }
    internal DbSet<OrganizationPermissionEntity> OrganizationPermission { get; set; }
    internal DbSet<UserConnectionEntity> QUserConnection { get; set; }
    internal DbSet<UserEntity> QUser { get; set; }
    internal DbSet<TPermissionEntity> TPermission { get; set; }
    internal DbSet<UserSessionEntity> TUserSession { get; set; }
    internal DbSet<TUserFieldEntity> TUserSetting { get; set; }

    // Utility: Setup
    internal DbSet<CollectionEntity> TCollection { get; set; }
    internal DbSet<CollectionItemEntity> TCollectionItem { get; set; }
    internal DbSet<RouteEndpoint> RouteEndpoint { get; set; }

    // Domain: Timeline
    internal DbSet<ChangeEntity> Change { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ApplyConfigurations(builder);
        ConfigureProperties(builder);
    }

    private void ApplyConfigurations(ModelBuilder builder)
    {
        // Domain: Booking
        // NOTE: EventUserConfiguration is applied before EventConfiguration because it is a junction entity
        // and must be configured first to avoid cryptic EF Core runtime exceptions.
        builder.ApplyConfiguration(new EventUserConfiguration());
        builder.ApplyConfiguration(new EventConfiguration());
        builder.ApplyConfiguration(new RegistrationConfiguration());

        // Domain: Competency
        builder.ApplyConfiguration(new StandardConfiguration());

        // Domain: Content
        builder.ApplyConfiguration(new FileActivityConfiguration());
        builder.ApplyConfiguration(new FileClaimConfiguration());
        builder.ApplyConfiguration(new FileConfiguration());
        builder.ApplyConfiguration(new TInputConfiguration());
        builder.ApplyConfiguration(new UploadConfiguration());

        // Feature: Messaging
        builder.ApplyConfiguration(new MailoutConfiguration());
        builder.ApplyConfiguration(new RecipientConfiguration());

        // Domain: Directory
        builder.ApplyConfiguration(new AddressConfiguration());
        builder.ApplyConfiguration(new GroupConfiguration());
        builder.ApplyConfiguration(new MembershipConfiguration());
        builder.ApplyConfiguration(new MembershipDeletionConfiguration());
        builder.ApplyConfiguration(new PendingPersonConfiguration());
        builder.ApplyConfiguration(new PersonConfiguration());
        builder.ApplyConfiguration(new QPersonSecretConfiguration());

        // Domain: Evaluation
        builder.ApplyConfiguration(new AssessmentConfiguration());
        builder.ApplyConfiguration(new AttemptConfiguration());
        builder.ApplyConfiguration(new BankConfiguration());
        builder.ApplyConfiguration(new BankQuestionConfiguration());
        builder.ApplyConfiguration(new BankSpecificationConfiguration());

        // Domain: Learning
        builder.ApplyConfiguration(new CourseConfiguration());
        builder.ApplyConfiguration(new CourseEnrollmentConfiguration());
        builder.ApplyConfiguration(new CourseMatchConfiguration());

        // Domain: Progress
        builder.ApplyConfiguration(new AchievementConfiguration());
        builder.ApplyConfiguration(new CredentialConfiguration());
        builder.ApplyConfiguration(new GradebookConfiguration());
        builder.ApplyConfiguration(new QGradebookEnrollmentConfiguration());
        builder.ApplyConfiguration(new PeriodConfiguration());

        // Domain: Reports
        builder.ApplyConfiguration(new ToolkitVisitConfiguration());
        builder.ApplyConfiguration(new ToolkitUsageConfiguration());

        // Domain: Workflow
        builder.ApplyConfiguration(new CaseConfiguration());
        builder.ApplyConfiguration(new CaseDocumentConfiguration());
        builder.ApplyConfiguration(new CaseDocumentRequestConfiguration());
        builder.ApplyConfiguration(new CaseGroupConfiguration());
        builder.ApplyConfiguration(new CaseUserConfiguration());
        builder.ApplyConfiguration(new TCaseStatusConfiguration());
        builder.ApplyConfiguration(new FormConfiguration());
        builder.ApplyConfiguration(new SubmissionConfiguration());

        // Domain: Workspace
        builder.ApplyConfiguration(new PageConfiguration());
        builder.ApplyConfiguration(new SiteConfiguration());

        // Utility: Metadata
        builder.ApplyConfiguration(new TActionConfiguration());

        // Utility: Security
        builder.ApplyConfiguration(new QOrganizationConfiguration());
        builder.ApplyConfiguration(new OrganizationPermissionConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new UserConnectionConfiguration());
        builder.ApplyConfiguration(new TPermissionConfiguration());
        builder.ApplyConfiguration(new UserSessionConfiguration());
        builder.ApplyConfiguration(new TUserFieldConfiguration());

        // Utility: Setup
        builder.ApplyConfiguration(new CollectionConfiguration());
        builder.ApplyConfiguration(new CollectionItemConfiguration());
        builder.ApplyConfiguration(new RouteEndpointConfiguration());

        // Utility: Timeline
        builder.ApplyConfiguration(new ChangeConfiguration());
    }

    private void ConfigureProperties(ModelBuilder builder)
    {
        var decimalProperties = builder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => (Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

        foreach (var property in decimalProperties)
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }
    }
}