using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Submissions")]
public class SubmissionController : ShiftControllerBase
{
    private readonly SubmissionService _submissionService;
    private readonly IPrincipalProvider _principalProvider;

    public SubmissionController(SubmissionService submissionService, IPrincipalProvider principalProvider)
    {
        _submissionService = submissionService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific submission
    /// </summary>
    [HttpHead("api/workflow/submissions/{submission:guid}")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertSubmission")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid responseSession, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _submissionService.AssertAsync(responseSession, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of submissions that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/submissions/collect")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<SubmissionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workflow/submissions")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<SubmissionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissions_get")]
    [AliasFor("collectSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectSubmissions query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _submissionService.CollectAsync(query, cancellation);

        var count = await _submissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the submissions that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/submissions/count")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountSubmissions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workflow/submissions/count")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissions_get")]
    [AliasFor("countSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountSubmissions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountSubmissions query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _submissionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of submissions that match specific criteria
    /// </summary>    
    [HttpPost("api/workflow/submissions/download")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workflow/submissions/download")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissions_get")]
    [AliasFor("downloadSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectSubmissions query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Workflow", "Submissions", query.Filter.Format, User);

        var models = await _submissionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _submissionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific submission
    /// </summary>
    [HttpGet("api/workflow/submissions/{submission:guid}")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<SubmissionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveSubmission")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid responseSession, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _submissionService.RetrieveAsync(responseSession, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of submissions that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/submissions/search")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<SubmissionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchSubmissions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workflow/submissions/search")]
    [HybridPermission("workflow/submissions", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<SubmissionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissions_get")]
    [AliasFor("searchSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchSubmissions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchSubmissions query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _submissionService.SearchAsync(query, cancellation);

        var count = await _submissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}