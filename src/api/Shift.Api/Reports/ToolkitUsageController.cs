using Microsoft.AspNetCore.Mvc;

using Shift.Sdk.Service.Security.Cookies;
using Shift.Service.Reports;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Toolkit Usage API")]
public class ToolkitUsageController(
    IPrincipalProvider principalProvider,
    ToolkitUsageService toolkitUsageService
) : ShiftControllerBase
{
    public class Input
    {
        public required string ActionUrl { get; init; }
        public DateTimeOffset? Visited { get; init; }
    }

    [HttpPost("api/reports/toolkit-usage/visit")]
    public async Task<ActionResult> VisitAsync(Input input)
    {
        var principal = principalProvider.GetPrincipal();
        
        if (principal.IsAuthenticated)
        {
            var visited = principal.IsOperator ? input.Visited : null;
            var tokenId = visited == null ? principal.CookieId : UniqueIdentifier.Create();

            await toolkitUsageService.SaveVisitAsync(principal.OrganizationId, principal.UserId, tokenId, input.ActionUrl, visited);
        }

        return Ok(new {});
    }    

    [HttpPost("api/reports/toolkit-usage/calculate")]
    [HybridPermission("reports/toolkit-usage", DataAccess.Create)]
    public async Task<ActionResult> CalculateAsync()
    {
        var count = await toolkitUsageService.CalculateAsync();
        return Ok(new { Count = count });
    }    
}