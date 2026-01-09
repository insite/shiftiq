using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseUserController : ShiftControllerBase
{
    private readonly CaseUserService _caseUserService;

    public CaseUserController(CaseUserService caseUserService)
    {
        _caseUserService = caseUserService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific case user
    /// </summary>
    [HttpHead("workflow/cases-users/{relationship:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseUser")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid join, CancellationToken cancellation = default)
    {
        var exists = await _caseUserService.AssertAsync(join, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of case users that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-users/collect")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Collect)]
    [ProducesResponseType<IEnumerable<CaseUserModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseUsers")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCaseUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-users")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Collect)]
    [ProducesResponseType<IEnumerable<CaseUserModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseUsers_get")]
    [AliasFor("collectCaseUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectCaseUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectCaseUsers query, CancellationToken cancellation)
    {
        var models = await _caseUserService.CollectAsync(query, cancellation);

        var count = await _caseUserService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the case users that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-users/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseUsers")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCaseUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-users/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseUsers_get")]
    [AliasFor("countCaseUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountCaseUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountCaseUsers query, CancellationToken cancellation)
    {
        var count = await _caseUserService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of case users that match specific criteria
    /// </summary>    
    [HttpPost("workflow/cases-users/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseUsers")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCaseUsers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-users/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseUsers_get")]
    [AliasFor("downloadCaseUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectCaseUsers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectCaseUsers query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "CaseUsers", query.Filter.Format, User);

        var models = await _caseUserService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _caseUserService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific case user
    /// </summary>
    [HttpGet("workflow/cases-users/{relationship:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Retrieve)]
    [ProducesResponseType<CaseUserModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseUser")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid join, CancellationToken cancellation = default)
    {
        var model = await _caseUserService.RetrieveAsync(join, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of case users that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-users/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Search)]
    [ProducesResponseType<IEnumerable<CaseUserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseUsers")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCaseUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-users/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseUser.Search)]
    [ProducesResponseType<IEnumerable<CaseUserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseUsers_get")]
    [AliasFor("searchCaseUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchCaseUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchCaseUsers query, CancellationToken cancellation)
    {
        var matches = await _caseUserService.SearchAsync(query, cancellation);

        var count = await _caseUserService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}