using Microsoft.AspNetCore.Mvc;

using Shift.Service.Content.Searches;
using Shift.Service.Content.Searches.Dtos;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class SearchController(IPrincipalProvider identityService, SearchEntryService searchService) : ShiftControllerBase
{
    [HttpGet("api/assets/searches")]
    [HybridPermission("assets/searches", DataAccess.Read)]
    public async Task<ActionResult<SearchEntryViewDto[]>> SearchAsync(string searchText, int page, CancellationToken cancellation)
    {
        if (page < 1)
            return Array.Empty<SearchEntryViewDto>();

        var principal = identityService.GetPrincipal();
        var result = await searchService.SearchAsync(principal.OrganizationId, principal.UserId, principal.IsOperator, searchText, page - 1, cancellation);

        var pagination = new QueryPagination(page, searchService.PageSize, -1);

        Response.AddPagination(pagination);

        return result;
    }

    [HttpPost("api/assets/searches")]
    [HybridPermission("assets/searches/admin", DataAccess.Create)]
    public async Task RefreshAsync(CancellationToken cancellation)
    {
        var principal = identityService.GetPrincipal();
        bool isCmds = StringHelper.EqualsAny(principal.Partition.Slug, ["e03", "cmds"]);

        await searchService.RefreshAsync(isCmds, cancellation);
    }
}
