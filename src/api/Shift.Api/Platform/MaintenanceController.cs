using Microsoft.AspNetCore.Mvc;

namespace Shift.Api.Platform;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class MaintenanceController : ShiftControllerBase
{
    public class LockoutResult
    {
        public required bool IsClosed { get; init; }
        public string? Description { get; init; }
    }

    [HttpGet("api/platform/maintenance/lockout")]
    [HybridPermission("platform/maintenance-lockout", DataAccess.Read)]
    [EndpointName("retrieveMaintenanceLockout")]
    public ActionResult<LockoutResult> RetrieveMaintenanceLockout(AppSettings appSettings)
    {
        var lockoutList = appSettings.Platform.Maintenance.Lockouts;
        if (lockoutList == null || lockoutList.Length == 0)
            return new LockoutResult { IsClosed = true };

        var enterprise = appSettings.Partition.Slug;

        var environment = appSettings.Environment.Name.ToString();

        var lockouts = new Lockouts(lockoutList, DateTimeOffset.Now, enterprise, environment);

        return new LockoutResult
        {
            IsClosed = lockouts.State == LockoutState.Closed,
            Description = lockouts.State != LockoutState.Closed
                ? Markdown.ToHtml(lockouts.Description)
                : null
        };
    }
}