using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseDocumentRequestController : ShiftControllerBase
{
    private readonly CaseDocumentRequestService _caseDocumentRequestService;

    public CaseDocumentRequestController(CaseDocumentRequestService caseDocumentRequestService)
    {
        _caseDocumentRequestService = caseDocumentRequestService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific case document request
    /// </summary>
    [HttpHead("workflow/cases-documents-requests/{relationship:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseDocumentRequest")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid issue, [FromRoute] string requestedFileCategory, CancellationToken cancellation = default)
    {
        var exists = await _caseDocumentRequestService.AssertAsync(issue, requestedFileCategory, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of case document requests that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-documents-requests/collect")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Collect)]
    [ProducesResponseType<IEnumerable<CaseDocumentRequestModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseDocumentRequests")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents-requests")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Collect)]
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
        var models = await _caseDocumentRequestService.CollectAsync(query, cancellation);

        var count = await _caseDocumentRequestService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the case document requests that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-documents-requests/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseDocumentRequests")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents-requests/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Count)]
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
        var count = await _caseDocumentRequestService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of case document requests that match specific criteria
    /// </summary>    
    [HttpPost("workflow/cases-documents-requests/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseDocumentRequests")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents-requests/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Download)]
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
    [HttpGet("workflow/cases-documents-requests/{relationship:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Retrieve)]
    [ProducesResponseType<CaseDocumentRequestModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseDocumentRequest")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid issue, [FromRoute] string requestedFileCategory, CancellationToken cancellation = default)
    {
        var model = await _caseDocumentRequestService.RetrieveAsync(issue, requestedFileCategory, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of case document requests that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-documents-requests/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Search)]
    [ProducesResponseType<IEnumerable<CaseDocumentRequestMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseDocumentRequests")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCaseDocumentRequests query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents-requests/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocumentRequest.Search)]
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
        var matches = await _caseDocumentRequestService.SearchAsync(query, cancellation);

        var count = await _caseDocumentRequestService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}