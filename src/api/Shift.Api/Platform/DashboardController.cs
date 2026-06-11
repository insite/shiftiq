using Microsoft.AspNetCore.Mvc;

using Shift.Sdk.Service.Platform;
using Shift.Sdk.Service.Platform.DashboardNotifications;

namespace Shift.Api.Platform;

[ApiController]
[ApiExplorerSettings(GroupName = "Platform API: Dashboard")]
public class DashboardController(IDashboardNotificationManager notificationManager) : ShiftControllerBase
{
    public class SearchResult
    {
        public required DashboardNotification[] Notifications { get; init; }
        public required Dictionary<Guid, string> Users { get; init; }
        public required Dictionary<Guid, bool> VisibleOnDashboard { get; init; }
    }

    [HttpGet("api/platform/dashboard")]
    [HybridPermission("platform/dashboard", DataAccess.Read)]
    [EndpointName("retrieveDashboard")]
    public async Task<ActionResult<DashboardData>> RetrieveDashboardAsync(IPrincipalProvider principalProvider, IDashboardService service)
    {
        var dashboard = await service.CreateAsync(principalProvider.OrganizationId);

        return Ok(dashboard);
    }

    [HttpPost("api/platform/dashboard/notifications/search")]
    [HybridPermission("platform/dashboard-notifications", DataAccess.Read)]
    [EndpointName("searchNotifications")]
    public async Task<ActionResult<SearchResult>> SearchNotificationsAsync(UserReader userReader, DashboardNotificationCriteria  criteria)
    {
        if (criteria == null)
            criteria = new DashboardNotificationCriteria();

        if (criteria.Filter == null)
            criteria.Filter = new QueryFilter();
        else
            criteria.Filter.PageSize = new QueryFilter().PageSize;

        var result = notificationManager.SearchNotifications(criteria);

        Dictionary<Guid, string> users;

        if (result.Notifications.Length > 0)
        {
            var userCriteria = new SearchUsers
            {
                UserIds = result.Notifications
                    .Select(x => x.CreatedBy)
                    .Union(result.Notifications.Select(x => x.ModifiedBy))
                    .Distinct().ToArray()
            };
            userCriteria.DisablePaging();

            users = (await userReader.SearchAsync(userCriteria)).ToDictionary(x => x.UserId, x => x.FullName);
        }
        else
            users = new Dictionary<Guid, string>();

        Response.AddPagination(criteria.Filter, result.TotalCount);

        return Ok(new SearchResult
        {
            Notifications = result.Notifications,
            Users = users,
            VisibleOnDashboard = result.VisibleOnDashboard
        });
    }

    [HttpGet("api/platform/dashboard/notifications/{notificationId}")]
    [HybridPermission("platform/dashboard-notifications", DataAccess.Read)]
    [ProducesResponseType<DashboardNotification>(StatusCodes.Status200OK)]
    [EndpointName("retrieveNotification")]
    public IActionResult RetrieveNotification(Guid notificationId)
    {
        var notification = notificationManager.RetrieveNotification(notificationId);
        return Ok(notification);
    }

    [HttpPost("api/platform/dashboard/notifications")]
    [HybridPermission("platform/dashboard-notifications", DataAccess.Update)]
    [ProducesResponseType<DashboardNotification>(StatusCodes.Status200OK)]
    [EndpointName("modifyNotification")]
    public IActionResult ModifyNotification(IPrincipalProvider principalProvider, DashboardNotification notification)
    {
        var principal = principalProvider.GetPrincipal();

        notificationManager.ModifyNotification(notification, principal.UserId);

        return Ok(notification);
    }

    [HttpDelete("api/platform/dashboard/notifications/{notificationId}")]
    [HybridPermission("platform/dashboard-notifications", DataAccess.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointName("deleteNotification")]
    public IActionResult DeleteNotification(Guid notificationId)
    {
        notificationManager.DeleteNotification(notificationId);

        return Ok(new {});
    }
}