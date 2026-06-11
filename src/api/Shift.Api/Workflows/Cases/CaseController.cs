using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflows API: Cases")]
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
    [HttpHead("api/workflows/cases/{case:guid}")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertCase")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid issue, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var organizationId = _principalProvider.GetOrganizationId(principal);
        var exists = await _caseService.AssertAsync(issue, organizationId, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of cases that match specific criteria
    /// </summary>
    [HttpPost("api/workflows/cases/collect")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [EndpointName("collectCases")]
    public async Task<ActionResult<IEnumerable<CaseModel>>> PostCollectAsync([FromBody] CollectCases query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workflows/cases")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [EndpointName("collectCases_get")]
    [AliasFor("collectCases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<CaseModel>>> GetCollectAsync([FromQuery] CollectCases query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<CaseModel>>> CollectAsync(CollectCases query, CancellationToken cancellation)
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
    [HttpPost("api/workflows/cases/count")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [EndpointName("countCases")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountCases query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workflows/cases/count")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [EndpointName("countCases_get")]
    [AliasFor("countCases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountCases query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountCases query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _caseService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of cases that match specific criteria
    /// </summary>    
    [HttpPost("api/workflows/cases/download")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCases")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCases query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workflows/cases/download")]
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
    [HttpGet("api/workflows/cases/{case:guid}")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [ProducesResponseType(typeof(CaseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveCase")]
    public async Task<ActionResult<CaseModel>> RetrieveAsync([FromRoute] Guid issue, CancellationToken cancellation = default)
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
    [HttpPost("api/workflows/cases/search")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [EndpointName("searchCases")]
    public async Task<ActionResult<IEnumerable<CaseMatch>>> PostSearchAsync([FromBody] SearchCases query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workflows/cases/search")]
    [HybridPermission("workflow/cases", DataAccess.Read)]
    [EndpointName("searchCases_get")]
    [AliasFor("searchCases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<CaseMatch>>> GetSearchAsync([FromQuery] SearchCases query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<CaseMatch>>> SearchAsync(SearchCases query, CancellationToken cancellation)
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