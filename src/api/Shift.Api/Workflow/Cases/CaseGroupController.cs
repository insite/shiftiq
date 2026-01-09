using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseGroupController : ShiftControllerBase
{
    private readonly CaseGroupService _caseGroupService;

    public CaseGroupController(CaseGroupService caseGroupService)
    {
        _caseGroupService = caseGroupService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific case group
    /// </summary>
    [HttpHead("workflow/cases-groups/{relationship:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseGroup")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid join, CancellationToken cancellation = default)
    {
        var exists = await _caseGroupService.AssertAsync(join, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of case groups that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-groups/collect")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Collect)]
    [ProducesResponseType<IEnumerable<CaseGroupModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCaseGroups")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCaseGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-groups")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Collect)]
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
        var models = await _caseGroupService.CollectAsync(query, cancellation);

        var count = await _caseGroupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the case groups that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-groups/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseGroups")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCaseGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-groups/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Count)]
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
        var count = await _caseGroupService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of case groups that match specific criteria
    /// </summary>    
    [HttpPost("workflow/cases-groups/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseGroups")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCaseGroups query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-groups/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Download)]
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
    [HttpGet("workflow/cases-groups/{relationship:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Retrieve)]
    [ProducesResponseType<CaseGroupModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseGroup")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid join, CancellationToken cancellation = default)
    {
        var model = await _caseGroupService.RetrieveAsync(join, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of case groups that match specific criteria
    /// </summary>
    [HttpPost("workflow/cases-groups/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Search)]
    [ProducesResponseType<IEnumerable<CaseGroupMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCaseGroups")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCaseGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/cases-groups/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseGroup.Search)]
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
        var matches = await _caseGroupService.SearchAsync(query, cancellation);

        var count = await _caseGroupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}