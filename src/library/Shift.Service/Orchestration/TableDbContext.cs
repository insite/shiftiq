using Microsoft.EntityFrameworkCore;

using Shift.Service.Achievement;
using Shift.Service.Booking;
using Shift.Service.Cases;
using Shift.Service.Competency;
using Shift.Service.Content;
using Shift.Service.Directory;
using Shift.Service.Gradebook;
using Shift.Service.Metadata;
using Shift.Service.Progress;
using Shift.Service.Security;
using Shift.Service.Site;

namespace Shift.Service;

public class TableDbContext : DbContext
{
    public TableDbContext(DbContextOptions options) : base(options) { }

    #region Storage Tables

    // Feature: Booking
    internal DbSet<QEventEntity> QEvent { get; set; }
    internal DbSet<EventUserEntity> QEventUser { get; set; }

    // Feature: Competency
    internal DbSet<StandardEntity> QStandard { get; set; }

    // Feature: Contact
    internal DbSet<QGroupEntity> QGroup { get; set; }
    internal DbSet<QMembershipEntity> QMembership { get; set; }
    internal DbSet<PersonEntity> QPerson { get; set; }
    internal DbSet<QPersonSecretEntity> QPersonSecret { get; set; }

    // Feature: Content
    internal DbSet<FileActivityEntity> TFileActivity { get; set; }
    internal DbSet<FileClaimEntity> TFileClaim { get; set; }
    internal DbSet<FileEntity> TFile { get; set; }
    internal DbSet<TInputEntity> TInput { get; set; }

    // Feature: Progress
    internal DbSet<QAchievementEntity> QAchievement { get; set; }
    internal DbSet<QCredentialEntity> QCredential { get; set; }
    internal DbSet<QGradebookEntity> QGradebook { get; set; }
    internal DbSet<QGradebookEnrollmentEntity> QGradebookEnrollment { get; set; }
    internal DbSet<PeriodEntity> QPeriod { get; set; }

    // Feature: Shell
    internal DbSet<QPageEntity> QPage { get; set; }
    internal DbSet<QSiteEntity> QSite { get; set; }

    // Feature: Workflow
    internal DbSet<TCaseStatusEntity> TCaseStatus { get; set; }

    // Utility: Metadata
    internal DbSet<TActionEntity> TAction { get; set; }

    // Utility: Security
    internal DbSet<QOrganizationEntity> QOrganization { get; set; }
    internal DbSet<QUserConnectionEntity> QUserConnection { get; set; }
    internal DbSet<QUserEntity> QUser { get; set; }
    internal DbSet<TPartitionFieldEntity> TPartitionSetting { get; set; }
    internal DbSet<TPermissionEntity> TPermission { get; set; }
    internal DbSet<TUserSessionEntity> TUserSession { get; set; }
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
        // Feature: Booking
        builder.ApplyConfiguration(new QEventConfiguration());
        builder.ApplyConfiguration(new EventUserConfiguration());

        // Feature: Competency
        builder.ApplyConfiguration(new StandardConfiguration());

        // Feature: Contact
        builder.ApplyConfiguration(new QGroupConfiguration());
        builder.ApplyConfiguration(new QMembershipConfiguration());
        builder.ApplyConfiguration(new PersonConfiguration());
        builder.ApplyConfiguration(new QPersonSecretConfiguration());

        // Feature: Content
        builder.ApplyConfiguration(new FileActivityConfiguration());
        builder.ApplyConfiguration(new FileClaimConfiguration());
        builder.ApplyConfiguration(new FileConfiguration());
        builder.ApplyConfiguration(new TInputConfiguration());

        // Feature: Progress
        builder.ApplyConfiguration(new QAchievementConfiguration());
        builder.ApplyConfiguration(new QCredentialConfiguration());
        builder.ApplyConfiguration(new QGradebookConfiguration());
        builder.ApplyConfiguration(new QGradebookEnrollmentConfiguration());
        builder.ApplyConfiguration(new PeriodConfiguration());

        // Feature: Shell
        builder.ApplyConfiguration(new QPageConfiguration());
        builder.ApplyConfiguration(new QSiteConfiguration());

        // Feature: Workflow
        builder.ApplyConfiguration(new TCaseStatusConfiguration());

        // Utility: Metadata
        builder.ApplyConfiguration(new TActionConfiguration());

        // Utility: Security
        builder.ApplyConfiguration(new QOrganizationConfiguration());
        builder.ApplyConfiguration(new QUserConfiguration());
        builder.ApplyConfiguration(new QUserConnectionConfiguration());
        builder.ApplyConfiguration(new TPartitionFieldConfiguration());
        builder.ApplyConfiguration(new TPermissionConfiguration());
        builder.ApplyConfiguration(new TUserSessionConfiguration());
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
        builder.Entity<QAchievementEntity>()
            .HasMany(e => e.Credentials)
            .WithOne(e => e.Achievement)
            .HasForeignKey(e => e.AchievementIdentifier)
            .HasPrincipalKey(e => e.AchievementIdentifier);

        builder.Entity<QAchievementEntity>()
            .HasMany(e => e.Gradebooks)
            .WithOne(e => e.Achievement)
            .HasForeignKey(e => e.AchievementIdentifier)
            .HasPrincipalKey(e => e.AchievementIdentifier);

        builder.Entity<QEventEntity>()
            .HasMany(e => e.Gradebooks)
            .WithOne(e => e.Event)
            .HasForeignKey(e => e.EventIdentifier)
            .HasPrincipalKey(e => e.EventIdentifier);

        builder.Entity<QEventEntity>()
            .HasMany(e => e.Users)
            .WithOne(e => e.Event)
            .HasForeignKey(e => e.EventIdentifier)
            .HasPrincipalKey(e => e.EventIdentifier);

        builder.Entity<QGradebookEntity>()
            .HasMany(e => e.Enrollments)
            .WithOne(e => e.Gradebook)
            .HasForeignKey(e => e.GradebookIdentifier)
            .HasPrincipalKey(e => e.GradebookIdentifier);

        builder.Entity<QGroupEntity>()
            .HasMany(e => e.Memberships)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupIdentifier)
            .HasPrincipalKey(e => e.GroupIdentifier);

        builder.Entity<QGroupEntity>()
            .HasMany(e => e.Permissions)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupIdentifier)
            .HasPrincipalKey(e => e.GroupIdentifier);

        builder.Entity<QPageEntity>()
            .HasMany(e => e.Children)
            .WithOne(e => e.Parent)
            .HasForeignKey(e => e.ParentPageIdentifier)
            .HasPrincipalKey(e => e.PageIdentifier);

        builder.Entity<QOrganizationEntity>()
           .HasMany(e => e.Permissions)
           .WithOne(e => e.Organization)
           .HasForeignKey(e => e.OrganizationIdentifier)
           .HasPrincipalKey(e => e.OrganizationIdentifier);

        builder.Entity<QOrganizationEntity>()
           .HasMany(e => e.Files)
           .WithOne(e => e.Organization)
           .HasForeignKey(e => e.OrganizationIdentifier)
           .HasPrincipalKey(e => e.OrganizationIdentifier);

        builder.Entity<QSiteEntity>()
            .HasMany(e => e.Pages)
            .WithOne(e => e.Site)
            .HasForeignKey(e => e.SiteIdentifier)
            .HasPrincipalKey(e => e.SiteIdentifier);

        builder.Entity<QUserEntity>()
            .HasMany(e => e.Events)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserIdentifier)
            .HasPrincipalKey(e => e.UserIdentifier);

        builder.Entity<QUserEntity>()
            .HasMany(e => e.Memberships)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserIdentifier)
            .HasPrincipalKey(e => e.UserIdentifier);

        builder.Entity<QUserEntity>()
            .HasMany(e => e.People)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserIdentifier)
            .HasPrincipalKey(e => e.UserIdentifier);

        builder.Entity<QUserEntity>()
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