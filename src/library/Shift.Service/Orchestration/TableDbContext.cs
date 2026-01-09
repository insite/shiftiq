using Microsoft.EntityFrameworkCore;

using Shift.Service.Assessment;
using Shift.Service.Booking;
using Shift.Service.Cases;
using Shift.Service.Competency;
using Shift.Service.Content;
using Shift.Service.Directory;
using Shift.Service.Metadata;
using Shift.Service.Progress;
using Shift.Service.Security;
using Shift.Service.Workflow;
using Shift.Service.Workspace;

namespace Shift.Service;

public class TableDbContext : DbContext
{
    public TableDbContext(DbContextOptions options) : base(options) { }

    #region Storage Tables

    // Feature: Assessment
    internal DbSet<AttemptEntity> Attempt { get; set; }
    internal DbSet<AttemptMatchEntity> AttemptMatch { get; set; }
    internal DbSet<AttemptOptionEntity> AttemptOption { get; set; }
    internal DbSet<AttemptPinEntity> AttemptPin { get; set; }
    internal DbSet<AttemptQuestionEntity> AttemptQuestion { get; set; }
    internal DbSet<AttemptSectionEntity> AttemptSection { get; set; }
    internal DbSet<AttemptSolutionEntity> AttemptSolution { get; set; }
    internal DbSet<BankEntity> Bank { get; set; }
    internal DbSet<BankFormEntity> BankForm { get; set; }
    internal DbSet<BankFormQuestionGradeitemEntity> BankFormQuestionGradeitem { get; set; }
    internal DbSet<BankOptionEntity> BankOption { get; set; }
    internal DbSet<BankQuestionAttachmentEntity> BankQuestionAttachment { get; set; }
    internal DbSet<BankQuestionEntity> BankQuestion { get; set; }
    internal DbSet<BankSpecificationEntity> BankSpecification { get; set; }
    internal DbSet<QuizAttemptEntity> QuizAttempt { get; set; }
    internal DbSet<QuizEntity> Quiz { get; set; }

    // Feature: Booking
    internal DbSet<EventEntity> QEvent { get; set; }
    internal DbSet<EventUserEntity> QEventUser { get; set; }
    internal DbSet<RegistrationAccommodationEntity> QRegistrationAccommodation { get; set; }
    internal DbSet<RegistrationEntity> QRegistration { get; set; }
    internal DbSet<RegistrationInstructorEntity> QRegistrationInstructor { get; set; }
    internal DbSet<RegistrationTimerEntity> QRegistrationTimer { get; set; }

    // Feature: Competency
    internal DbSet<StandardEntity> QStandard { get; set; }

    // Feature: Contact
    internal DbSet<GroupEntity> QGroup { get; set; }
    internal DbSet<MembershipEntity> QMembership { get; set; }
    internal DbSet<PersonEntity> QPerson { get; set; }
    internal DbSet<QPersonSecretEntity> QPersonSecret { get; set; }

    // Feature: Content
    internal DbSet<FileActivityEntity> TFileActivity { get; set; }
    internal DbSet<FileClaimEntity> TFileClaim { get; set; }
    internal DbSet<FileEntity> TFile { get; set; }
    internal DbSet<TInputEntity> TInput { get; set; }

    // Feature: Progress
    internal DbSet<AchievementEntity> QAchievement { get; set; }
    internal DbSet<QCredentialEntity> QCredential { get; set; }
    internal DbSet<GradebookEntity> QGradebook { get; set; }
    internal DbSet<QGradebookEnrollmentEntity> QGradebookEnrollment { get; set; }
    internal DbSet<PeriodEntity> QPeriod { get; set; }

    // Feature: Shell
    internal DbSet<PageEntity> QPage { get; set; }
    internal DbSet<SiteEntity> QSite { get; set; }

    // Feature: Workflow
    internal DbSet<CaseDocumentEntity> QCaseDocument { get; set; }
    internal DbSet<CaseDocumentRequestEntity> QCaseDocumentRequest { get; set; }
    internal DbSet<CaseEntity> QCase { get; set; }
    internal DbSet<CaseGroupEntity> QCaseGroup { get; set; }
    internal DbSet<CaseUserEntity> QCaseUser { get; set; }
    internal DbSet<TCaseStatusEntity> TCaseStatus { get; set; }
    internal DbSet<FormConditionEntity> FormCondition { get; set; }
    internal DbSet<FormEntity> Form { get; set; }
    internal DbSet<FormOptionItemEntity> FormOptionItem { get; set; }
    internal DbSet<FormOptionListEntity> FormOptionList { get; set; }
    internal DbSet<FormQuestionEntity> FormQuestion { get; set; }
    internal DbSet<SubmissionAnswerEntity> SubmissionAnswer { get; set; }
    internal DbSet<SubmissionEntity> Submission { get; set; }
    internal DbSet<SubmissionOptionEntity> SubmissionOption { get; set; }

    // Utility: Metadata
    internal DbSet<TActionEntity> TAction { get; set; }

    // Utility: Security
    internal DbSet<OrganizationEntity> Organization { get; set; }
    internal DbSet<UserConnectionEntity> QUserConnection { get; set; }
    internal DbSet<UserEntity> QUser { get; set; }
    internal DbSet<TPartitionFieldEntity> TPartitionSetting { get; set; }
    internal DbSet<TPermissionEntity> TPermission { get; set; }
    internal DbSet<UserSessionEntity> TUserSession { get; set; }
    internal DbSet<TUserFieldEntity> TUserSetting { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ApplyConfigurations(builder);
        ConfigureJunctions(builder);
        ConfigureEntities(builder);
        ConfigureProperties(builder);
    }

    private void ApplyConfigurations(ModelBuilder builder)
    {
        // Feature: Assessment
        builder.ApplyConfiguration(new AttemptConfiguration());
        builder.ApplyConfiguration(new AttemptMatchConfiguration());
        builder.ApplyConfiguration(new AttemptOptionConfiguration());
        builder.ApplyConfiguration(new AttemptPinConfiguration());
        builder.ApplyConfiguration(new AttemptQuestionConfiguration());
        builder.ApplyConfiguration(new AttemptSectionConfiguration());
        builder.ApplyConfiguration(new AttemptSolutionConfiguration());
        builder.ApplyConfiguration(new BankConfiguration());
        builder.ApplyConfiguration(new BankFormConfiguration());
        builder.ApplyConfiguration(new BankFormQuestionGradeitemConfiguration());
        builder.ApplyConfiguration(new BankOptionConfiguration());
        builder.ApplyConfiguration(new BankQuestionAttachmentConfiguration());
        builder.ApplyConfiguration(new BankQuestionConfiguration());
        builder.ApplyConfiguration(new BankSpecificationConfiguration());
        builder.ApplyConfiguration(new QuizAttemptConfiguration());
        builder.ApplyConfiguration(new QuizConfiguration());

        // Feature: Booking
        builder.ApplyConfiguration(new EventConfiguration());
        builder.ApplyConfiguration(new EventUserConfiguration());
        builder.ApplyConfiguration(new RegistrationAccommodationConfiguration());
        builder.ApplyConfiguration(new RegistrationConfiguration());
        builder.ApplyConfiguration(new RegistrationInstructorConfiguration());
        builder.ApplyConfiguration(new RegistrationTimerConfiguration());

        // Feature: Competency
        builder.ApplyConfiguration(new StandardConfiguration());

        // Feature: Contact
        builder.ApplyConfiguration(new GroupConfiguration());
        builder.ApplyConfiguration(new MembershipConfiguration());
        builder.ApplyConfiguration(new PersonConfiguration());
        builder.ApplyConfiguration(new QPersonSecretConfiguration());

        // Feature: Content
        builder.ApplyConfiguration(new FileActivityConfiguration());
        builder.ApplyConfiguration(new FileClaimConfiguration());
        builder.ApplyConfiguration(new FileConfiguration());
        builder.ApplyConfiguration(new TInputConfiguration());

        // Feature: Progress
        builder.ApplyConfiguration(new AchievementConfiguration());
        builder.ApplyConfiguration(new QCredentialConfiguration());
        builder.ApplyConfiguration(new GradebookConfiguration());
        builder.ApplyConfiguration(new QGradebookEnrollmentConfiguration());
        builder.ApplyConfiguration(new PeriodConfiguration());

        // Feature: Shell
        builder.ApplyConfiguration(new PageConfiguration());
        builder.ApplyConfiguration(new SiteConfiguration());

        // Feature: Workflow
        builder.ApplyConfiguration(new CaseConfiguration());
        builder.ApplyConfiguration(new CaseDocumentConfiguration());
        builder.ApplyConfiguration(new CaseDocumentRequestConfiguration());
        builder.ApplyConfiguration(new CaseGroupConfiguration());
        builder.ApplyConfiguration(new CaseUserConfiguration());
        builder.ApplyConfiguration(new TCaseStatusConfiguration());
        builder.ApplyConfiguration(new FormConditionConfiguration());
        builder.ApplyConfiguration(new FormConfiguration());
        builder.ApplyConfiguration(new FormOptionItemConfiguration());
        builder.ApplyConfiguration(new FormOptionListConfiguration());
        builder.ApplyConfiguration(new FormQuestionConfiguration());
        builder.ApplyConfiguration(new SubmissionAnswerConfiguration());
        builder.ApplyConfiguration(new SubmissionConfiguration());
        builder.ApplyConfiguration(new SubmissionOptionConfiguration());

        // Utility: Metadata
        builder.ApplyConfiguration(new TActionConfiguration());

        // Utility: Security
        builder.ApplyConfiguration(new QOrganizationConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new UserConnectionConfiguration());
        builder.ApplyConfiguration(new TPartitionFieldConfiguration());
        builder.ApplyConfiguration(new TPermissionConfiguration());
        builder.ApplyConfiguration(new UserSessionConfiguration());
        builder.ApplyConfiguration(new TUserFieldConfiguration());
    }

    /// <remarks>
    /// Always configure junction entities before you configure individual entities. Otherwise, you can expect very 
    /// cryptic unhandled run-time exceptions from EF Core.
    /// </remarks>
    private void ConfigureJunctions(ModelBuilder builder)
    {
        builder.Entity<EventUserEntity>()
            .HasKey(junction => new { junction.EventIdentifier, junction.UserIdentifier });
    }

    private void ConfigureEntities(ModelBuilder builder)
    {
        builder.Entity<AchievementEntity>()
            .HasMany(e => e.Credentials)
            .WithOne(e => e.Achievement)
            .HasForeignKey(e => e.AchievementIdentifier)
            .HasPrincipalKey(e => e.AchievementIdentifier);

        builder.Entity<AchievementEntity>()
            .HasMany(e => e.Gradebooks)
            .WithOne(e => e.Achievement)
            .HasForeignKey(e => e.AchievementIdentifier)
            .HasPrincipalKey(e => e.AchievementIdentifier);

        builder.Entity<AttemptEntity>()
            .HasMany(e => e.Pins)
            .WithOne(e => e.Attempt)
            .HasForeignKey(e => e.AttemptIdentifier)
            .HasPrincipalKey(e => e.AttemptIdentifier);

        builder.Entity<AttemptEntity>()
            .HasMany(e => e.Sections)
            .WithOne(e => e.Attempt)
            .HasForeignKey(e => e.AttemptIdentifier)
            .HasPrincipalKey(e => e.AttemptIdentifier);

        builder.Entity<AttemptEntity>()
            .HasMany(e => e.Solutions)
            .WithOne(e => e.Attempt)
            .HasForeignKey(e => e.AttemptIdentifier)
            .HasPrincipalKey(e => e.AttemptIdentifier);

        builder.Entity<EventEntity>()
            .HasMany(e => e.Gradebooks)
            .WithOne(e => e.Event)
            .HasForeignKey(e => e.EventIdentifier)
            .HasPrincipalKey(e => e.EventIdentifier);

        builder.Entity<EventEntity>()
            .HasMany(e => e.Users)
            .WithOne(e => e.Event)
            .HasForeignKey(e => e.EventIdentifier)
            .HasPrincipalKey(e => e.EventIdentifier);

        builder.Entity<GradebookEntity>()
            .HasMany(e => e.Enrollments)
            .WithOne(e => e.Gradebook)
            .HasForeignKey(e => e.GradebookIdentifier)
            .HasPrincipalKey(e => e.GradebookIdentifier);

        builder.Entity<GroupEntity>()
            .HasMany(e => e.Memberships)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupIdentifier)
            .HasPrincipalKey(e => e.GroupIdentifier);

        builder.Entity<GroupEntity>()
            .HasMany(e => e.Permissions)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupIdentifier)
            .HasPrincipalKey(e => e.GroupIdentifier);

        builder.Entity<PageEntity>()
            .HasMany(e => e.Children)
            .WithOne(e => e.Parent)
            .HasForeignKey(e => e.ParentPageIdentifier)
            .HasPrincipalKey(e => e.PageIdentifier);

        builder.Entity<OrganizationEntity>()
           .HasMany(e => e.Permissions)
           .WithOne(e => e.Organization)
           .HasForeignKey(e => e.OrganizationIdentifier)
           .HasPrincipalKey(e => e.OrganizationIdentifier);

        builder.Entity<OrganizationEntity>()
           .HasMany(e => e.People)
           .WithOne(e => e.Organization)
           .HasForeignKey(e => e.OrganizationIdentifier)
           .HasPrincipalKey(e => e.OrganizationIdentifier);

        builder.Entity<OrganizationEntity>()
           .HasMany(e => e.Files)
           .WithOne(e => e.Organization)
           .HasForeignKey(e => e.OrganizationIdentifier)
           .HasPrincipalKey(e => e.OrganizationIdentifier);

        builder.Entity<SiteEntity>()
            .HasMany(e => e.Pages)
            .WithOne(e => e.Site)
            .HasForeignKey(e => e.SiteIdentifier)
            .HasPrincipalKey(e => e.SiteIdentifier);

        builder.Entity<UserEntity>()
            .HasMany(e => e.Events)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserIdentifier)
            .HasPrincipalKey(e => e.UserIdentifier);

        builder.Entity<UserEntity>()
            .HasMany(e => e.Memberships)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserIdentifier)
            .HasPrincipalKey(e => e.UserIdentifier);

        builder.Entity<UserEntity>()
            .HasMany(e => e.People)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserIdentifier)
            .HasPrincipalKey(e => e.UserIdentifier);

        builder.Entity<UserEntity>()
           .HasMany(e => e.Files)
           .WithOne(e => e.User)
           .HasForeignKey(e => e.UserIdentifier)
           .HasPrincipalKey(e => e.UserIdentifier);

        builder.Entity<FileEntity>()
            .HasMany(e => e.Activities)
            .WithOne(e => e.File)
            .HasForeignKey(e => e.FileIdentifier)
            .HasPrincipalKey(e => e.FileIdentifier);

        builder.Entity<FileEntity>()
            .HasMany(e => e.Claims)
            .WithOne(e => e.File)
            .HasForeignKey(e => e.FileIdentifier)
            .HasPrincipalKey(e => e.FileIdentifier);
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