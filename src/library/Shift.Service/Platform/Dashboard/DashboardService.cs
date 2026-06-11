using System.Collections.Concurrent;

using Shift.Contract;
using Shift.Sdk.Service.Platform;
using Shift.Sdk.Service.Platform.DashboardNotifications;
using Shift.Service.Assessment;
using Shift.Service.Directory;
using Shift.Service.Evaluation;
using Shift.Service.Learning;
using Shift.Service.Security;

namespace Shift.Service.Platform.Dashboard;

public class DashboardService(
    BankReader bankReader,
    AssessmentReader formReader,
    PersonReader personReader,
    CourseReader courseReader,
    CourseEnrollmentReader courseEnrollmentReader,
    IDashboardNotificationManager notificationManager,
    OrganizationService organizationService,
    OrganizationAdapter organizationAdapter
) : IDashboardService
{
    private class CashedDashboardCounts
    {
        public required DashboardCounts Counts { get; init; }
        public DateTimeOffset CreatedOn { get; init; }
    }

    private const int CacheLifeInMinutes = 15;

    private static readonly ConcurrentDictionary<Guid, CashedDashboardCounts> _countsPerOrganization = new ();

    public async Task<DashboardData> CreateAsync(Guid organizationId)
    {
        var activeNotifications = notificationManager.CollectActiveNotifications();

        var organization = await organizationService.RetrieveAsync(organizationId);
        var organizationData = organization != null ? organizationAdapter.ToData(organization) : null;

        var hideMyDashboard = organizationData != null && organizationData.Toolkits.Accounts.HideAdminMyDashboard;

        var counts = hideMyDashboard
            ? new DashboardCounts()
            : await GetCountsAsync(organizationId);

        return new DashboardData
        {
            ActiveNotifications = activeNotifications,
            HideMyDashboard = hideMyDashboard,
            Counts = counts
        };
    }

    private async Task<DashboardCounts> GetCountsAsync(Guid organizationId)
    {
        if (_countsPerOrganization.TryGetValue(organizationId, out var cachedCounts)
            && cachedCounts.CreatedOn.AddMinutes(CacheLifeInMinutes) >= DateTimeOffset.Now
            )
        {
            return cachedCounts.Counts;
        }

        var counts = new DashboardCounts
        {
            BankCount = await CountBanksAsync(organizationId),
            ActiveBankCount = await CountActiveBanksAsync(organizationId),
            FormCount = await CountFormsAsync(organizationId),
            PublishedFormCount = await CountPublishedFormsAsync(organizationId),
            PersonCount = await CountPeopleAsync(organizationId),
            ActivePersonCount = await CountActivePeopleAsync(organizationId),
            ApprovedPersonCount = await CountApprovedPeopleAsync(organizationId),
            CourseCount = await CountCoursesAsync(organizationId),
            PublishedCourseCount = await CountPublishedCoursesAsync(organizationId),
            StartedEnrollmentCount = await courseEnrollmentReader.CountStartedEnrollmentsAsync(organizationId),
            CompletedEnrollmentCount = await courseEnrollmentReader.CountCompletedEnrollmentsAsync(organizationId),
        };

        cachedCounts = new CashedDashboardCounts
        {
            Counts = counts,
            CreatedOn = DateTimeOffset.Now
        };

        _countsPerOrganization.AddOrUpdate(organizationId, cachedCounts, (_, _) => cachedCounts);

        return counts;
    }

    private async Task<int> CountBanksAsync(Guid organizationId)
    {
        var criteria = new CountBanks
        {
            OrganizationId = organizationId,
        };
        criteria.DisablePaging();

        return await bankReader.CountAsync(criteria);        
    }

    private async Task<int> CountActiveBanksAsync(Guid organizationId)
    {
        var criteria = new CountBanks
        {
            OrganizationId = organizationId,
            IsActive = true,
        };
        criteria.DisablePaging();

        return await bankReader.CountAsync(criteria);        
    }

    private async Task<int> CountFormsAsync(Guid organizationId)
    {
        var criteria = new CountAssessments
        {
            OrganizationId = organizationId,
        };
        criteria.DisablePaging();

        return await formReader.CountAsync(criteria);
    }

    private async Task<int> CountPublishedFormsAsync(Guid organizationId)
    {
        var criteria = new CountAssessments
        {
            OrganizationId = organizationId,
            FormPublicationStatus = "Published"
        };
        criteria.DisablePaging();

        return await formReader.CountAsync(criteria);
    }

    private async Task<int> CountPeopleAsync(Guid organizationId)
    {
        var criteria = new CountPeople
        {
            OrganizationId = organizationId
        };
        criteria.DisablePaging();

        return await personReader.CountAsync(criteria);
    }

    private async Task<int> CountActivePeopleAsync(Guid organizationId)
    {
        var criteria = new CountPeople
        {
            OrganizationId = organizationId,
            LastAuthenticatedSince = DateTimeOffset.UtcNow.AddMonths(-3)
        };
        criteria.DisablePaging();

        return await personReader.CountAsync(criteria);
    }

    private async Task<int> CountApprovedPeopleAsync(Guid organizationId)
    {
        var criteria = new CountPeople
        {
            OrganizationId = organizationId,
            IsApproved = true
        };
        criteria.DisablePaging();

        return await personReader.CountAsync(criteria);        
    }

    private async Task<int> CountCoursesAsync(Guid organizationId)
    {
        var criteria = new CountCourses
        {
            OrganizationId = organizationId,
        };
        criteria.DisablePaging();

        return await courseReader.CountAsync(criteria);
    }

    private async Task<int> CountPublishedCoursesAsync(Guid organizationId)
    {
        var criteria = new CountCourses
        {
            OrganizationId = organizationId,
            IsPublished = true,
        };
        criteria.DisablePaging();

        return await courseReader.CountAsync(criteria);
    }
}