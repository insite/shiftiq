using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseGroupController : ShiftControllerBase
{
    private readonly CaseGroupService _caseGroupService;
    private readonly IPrincipalProvider _principalProvider;

    public CaseGroupController(CaseGroupService caseGroupService, IPrincipalProvider principalProvider)
    {
        _caseGroupService = caseGroupService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific case group
    /// </summary>
    [HttpHead("api/workflow/cases-groups/{relationship:guid}")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseGroup")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid join, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var organizationId = _principalProvider.GetOrganizationId(principal);
        var exists = await _caseGroupService.AssertAsync(join, organizationId, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of case groups that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases-groups/collect")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseGroupModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseGroups")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCaseGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-groups")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseGroupModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseGroups_get")]
    [AliasFor("collectCaseGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectCaseGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectCaseGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _caseGroupService.CollectAsync(query, cancellation);

        var count = await _caseGroupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the case groups that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases-groups/count")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseGroups")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCaseGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-groups/count")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseGroups_get")]
    [AliasFor("countCaseGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountCaseGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountCaseGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _caseGroupService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of case groups that match specific criteria
    /// </summary>    
    [HttpPost("api/workflow/cases-groups/download")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseGroups")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCaseGroups query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-groups/download")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseGroups_get")]
    [AliasFor("downloadCaseGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectCaseGroups query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectCaseGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Workflow", "CaseGroups", query.Filter.Format, User);

        var models = await _caseGroupService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _caseGroupService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific case group
    /// </summary>
    [HttpGet("api/workflow/cases-groups/{relationship:guid}")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<CaseGroupModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseGroup")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid join, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var model = await _caseGroupService.RetrieveAsync(join, cancellation);
        if (model == null)
            return NotFound();
        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();
        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of case groups that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases-groups/search")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseGroupMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseGroups")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCaseGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-groups/search")]
    [HybridPermission("workflow/cases-groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseGroupMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseGroups_get")]
    [AliasFor("searchCaseGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchCaseGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchCaseGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _caseGroupService.SearchAsync(query, cancellation);

        var count = await _caseGroupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}