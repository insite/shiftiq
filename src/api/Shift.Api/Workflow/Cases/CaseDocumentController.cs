using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseDocumentController : ShiftControllerBase
{
    private readonly CaseDocumentService _caseDocumentService;

    public CaseDocumentController(CaseDocumentService caseDocumentService)
    {
        _caseDocumentService = caseDocumentService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific case document
    /// </summary>
    [HttpHead("workflow/cases-documents/{document:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseDocument")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attachment, CancellationToken cancellation = default)
    {
        var exists = await _caseDocumentService.AssertAsync(attachment, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of case documents that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-documents/collect")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Collect)]
    [ProducesResponseType<IEnumerable<CaseDocumentModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseDocuments")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCaseDocuments query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Collect)]
    [ProducesResponseType<IEnumerable<CaseDocumentModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseDocuments_get")]
    [AliasFor("collectCaseDocuments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectCaseDocuments query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectCaseDocuments query, CancellationToken cancellation)
    {
        var models = await _caseDocumentService.CollectAsync(query, cancellation);

        var count = await _caseDocumentService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the case documents that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-documents/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseDocuments")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCaseDocuments query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseDocuments_get")]
    [AliasFor("countCaseDocuments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountCaseDocuments query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountCaseDocuments query, CancellationToken cancellation)
    {
        var count = await _caseDocumentService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of case documents that match specific criteria
    /// </summary>    
    [HttpPost("workflow/cases-documents/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseDocuments")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCaseDocuments query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseDocuments_get")]
    [AliasFor("downloadCaseDocuments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectCaseDocuments query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectCaseDocuments query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "CaseDocuments", query.Filter.Format, User);

        var models = await _caseDocumentService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _caseDocumentService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific case document
    /// </summary>
    [HttpGet("workflow/cases-documents/{document:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Retrieve)]
    [ProducesResponseType<CaseDocumentModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseDocument")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attachment, CancellationToken cancellation = default)
    {
        var model = await _caseDocumentService.RetrieveAsync(attachment, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of case documents that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-documents/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Search)]
    [ProducesResponseType<IEnumerable<CaseDocumentMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseDocuments")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCaseDocuments query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-documents/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseDocument.Search)]
    [ProducesResponseType<IEnumerable<CaseDocumentMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseDocuments_get")]
    [AliasFor("searchCaseDocuments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchCaseDocuments query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchCaseDocuments query, CancellationToken cancellation)
    {
        var matches = await _caseDocumentService.SearchAsync(query, cancellation);

        var count = await _caseDocumentService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}