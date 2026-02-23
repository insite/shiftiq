using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseController : ShiftControllerBase
{
    private readonly CaseService _caseService;
    private readonly IPrincipalProvider _principalProvider;

    public CaseController(CaseService caseService, IPrincipalProvider principalProvider)
    {
        _caseService = caseService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific case
    /// </summary>
    [HttpHead("api/workflow/cases/{case:guid}")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCase")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid issue, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var organizationId = _principalProvider.GetOrganizationId(principal);
        var exists = await _caseService.AssertAsync(issue, organizationId, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of cases that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases/collect")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCases")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCases query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCases_get")]
    [AliasFor("collectCases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectCases query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectCases query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _caseService.CollectAsync(query, cancellation);

        var count = await _caseService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the cases that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases/count")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCases")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCases query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases/count")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCases_get")]
    [AliasFor("countCases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountCases query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountCases query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _caseService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of cases that match specific criteria
    /// </summary>    
    [HttpPost("api/workflow/cases/download")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCases")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCases query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases/download")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCases_get")]
    [AliasFor("downloadCases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectCases query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectCases query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Workflow", "Cases", query.Filter.Format, User);

        var models = await _caseService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _caseService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific case
    /// </summary>
    [HttpGet("api/workflow/cases/{case:guid}")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<CaseModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCase")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid issue, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var model = await _caseService.RetrieveAsync(issue, cancellation);
        if (model == null)
            return NotFound();
        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();
        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of cases that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases/search")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCases")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCases query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases/search")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCases_get")]
    [AliasFor("searchCases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchCases query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchCases query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _caseService.SearchAsync(query, cancellation);

        var count = await _caseService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}