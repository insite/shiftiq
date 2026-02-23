using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseDocumentRequestController : ShiftControllerBase
{
    private readonly CaseDocumentRequestService _caseDocumentRequestService;
    private readonly IPrincipalProvider _principalProvider;

    public CaseDocumentRequestController(CaseDocumentRequestService caseDocumentRequestService, IPrincipalProvider principalProvider)
    {
        _caseDocumentRequestService = caseDocumentRequestService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific case document request
    /// </summary>
    [HttpHead("api/workflow/cases-documents-requests/{relationship:guid}")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseDocumentRequest")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid issue, [FromRoute] string requestedFileCategory, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var organizationId = _principalProvider.GetOrganizationId(principal);
        var exists = await _caseDocumentRequestService.AssertAsync(issue, requestedFileCategory, organizationId, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of case document requests that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases-documents-requests/collect")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseDocumentRequestModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseDocumentRequests")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-documents-requests")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseDocumentRequestModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseDocumentRequests_get")]
    [AliasFor("collectCaseDocumentRequests")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectCaseDocumentRequests query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _caseDocumentRequestService.CollectAsync(query, cancellation);

        var count = await _caseDocumentRequestService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the case document requests that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases-documents-requests/count")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseDocumentRequests")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-documents-requests/count")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseDocumentRequests_get")]
    [AliasFor("countCaseDocumentRequests")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountCaseDocumentRequests query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _caseDocumentRequestService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of case document requests that match specific criteria
    /// </summary>    
    [HttpPost("api/workflow/cases-documents-requests/download")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseDocumentRequests")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-documents-requests/download")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseDocumentRequests_get")]
    [AliasFor("downloadCaseDocumentRequests")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectCaseDocumentRequests query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Workflow", "CaseDocumentRequests", query.Filter.Format, User);

        var models = await _caseDocumentRequestService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _caseDocumentRequestService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific case document request
    /// </summary>
    [HttpGet("api/workflow/cases-documents-requests/{relationship:guid}")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<CaseDocumentRequestModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseDocumentRequest")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid issue, [FromRoute] string requestedFileCategory, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var model = await _caseDocumentRequestService.RetrieveAsync(issue, requestedFileCategory, cancellation);
        if (model == null)
            return NotFound();
        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();
        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of case document requests that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/cases-documents-requests/search")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseDocumentRequestMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseDocumentRequests")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workflow/cases-documents-requests/search")]
    [HybridPermission("workflow/cases-documents-requests", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CaseDocumentRequestMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseDocumentRequests_get")]
    [AliasFor("searchCaseDocumentRequests")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchCaseDocumentRequests query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _caseDocumentRequestService.SearchAsync(query, cancellation);

        var count = await _caseDocumentRequestService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}