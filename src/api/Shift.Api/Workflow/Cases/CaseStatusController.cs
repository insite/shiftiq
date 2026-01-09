using System.Text;

using Microsoft.AspNetCore.Mvc;

using Shift.Service.Cases;

namespace Shift.Api.Workflow;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseStatusController : ControllerBase
{
    private readonly ILogger<CaseStatusController> _logger;
    private readonly ReleaseSettings _releaseSettings;
    private readonly DatabaseSettings _databaseSettings;
    private readonly TCaseStatusService _caseStatusService;

    public CaseStatusController(
        ILogger<CaseStatusController> logger,
        ReleaseSettings releaseSettings,
        DatabaseSettings databaseSettings,
        TCaseStatusService caseStatusService)
    {
        _logger = logger;
        _releaseSettings = releaseSettings;
        _databaseSettings = databaseSettings;
        _caseStatusService = caseStatusService;
    }

    #region Retrieve

    [HttpHead("workflow/case-statuses/{statusId:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseStatus")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid statusId,
        CancellationToken cancellation = default)
    {
        var exists = await _caseStatusService.AssertAsync(statusId, cancellation);
        return exists ? Ok() : NotFound();
    }

    [HttpGet("workflow/case-statuses/{statusId:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Retrieve)]
    [ProducesResponseType<CaseStatusModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseStatus")]
    [ActionName("RetrieveAsync")]
    public async Task<ActionResult<CaseStatusModel>> RetrieveAsync(
        [FromRoute] Guid statusId,
        CancellationToken cancellation = default)
    {
        var model = await _caseStatusService.RetrieveAsync(statusId, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    #endregion

    #region Count

    [HttpGet("workflow/case-statuses/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseStatuses")]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountCaseStatuses query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpPost("workflow/case-statuses/count")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Count)]
    [ProducesResponseType(typeof(CountResult), StatusCodes.Status200OK)]
    [EndpointName("countCaseStatuses_post")]
    [AliasFor("countCaseStatuses")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountCaseStatuses query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(
        CountCaseStatuses query,
        CancellationToken cancellation)
    {
        var count = await _caseStatusService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    #endregion

    #region Collect

    [HttpGet("workflow/case-statuses")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Collect)]
    [ProducesResponseType(typeof(IEnumerable<CaseStatusModel>), StatusCodes.Status200OK)]
    [EndpointName("collectCaseStatuses")]
    public async Task<ActionResult<IEnumerable<CaseStatusModel>>> GetCollectAsync(
        [FromQuery] CollectCaseStatuses query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpPost("workflow/case-statuses/collect")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Collect)]
    [ProducesResponseType(typeof(IEnumerable<CaseStatusModel>), StatusCodes.Status200OK)]
    [EndpointName("collectCaseStatuses_post")]
    [AliasFor("collectCaseStatuses")]
    public async Task<ActionResult<IEnumerable<CaseStatusModel>>> PostCollectAsync(
        [FromBody] CollectCaseStatuses query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<CaseStatusModel>>> CollectAsync(
        CollectCaseStatuses query,
        CancellationToken cancellation)
    {
        var models = await _caseStatusService.CollectAsync(query, cancellation);
        var count = await _caseStatusService.CountAsync(query, cancellation);
        Response.AddPagination(query.Filter, count);
        return Ok(models);
    }

    #endregion

    #region Search

    [HttpGet("workflow/case-statuses/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Search)]
    [ProducesResponseType(typeof(IEnumerable<CaseStatusMatch>), StatusCodes.Status200OK)]
    [EndpointName("searchCaseStatuses")]
    public async Task<ActionResult<IEnumerable<CaseStatusMatch>>> GetSearchAsync(
        [FromQuery] SearchCaseStatuses query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpPost("workflow/case-statuses/search")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Search)]
    [ProducesResponseType(typeof(IEnumerable<CaseStatusMatch>), StatusCodes.Status200OK)]
    [EndpointName("searchCaseStatuses_post")]
    [AliasFor("searchCaseStatuses")]
    public async Task<ActionResult<IEnumerable<CaseStatusMatch>>> PostSearchAsync(
        [FromBody] SearchCaseStatuses query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<CaseStatusMatch>>> SearchAsync(
        SearchCaseStatuses query,
        CancellationToken cancellation)
    {
        var matches = await _caseStatusService.SearchAsync(query, cancellation);
        var count = await _caseStatusService.CountAsync(query, cancellation);
        Response.AddPagination(query.Filter, count);
        return Ok(matches);
    }

    #endregion

    #region Create

    [HttpPost("workflow/case-statuses")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Create)]
    [ProducesResponseType<CaseStatusModel>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("createCaseStatus")]
    public async Task<ActionResult<CaseStatusModel>> CreateAsync(
        [FromBody] CreateCaseStatus command,
        CancellationToken cancellation = default)
    {
        var model = await _caseStatusService.CreateAsync(command, cancellation);
        return CreatedAtAction(
            nameof(RetrieveAsync),
            new { statusId = model?.StatusIdentifier },
            model);
    }

    #endregion

    #region Modify

    [HttpPut("workflow/case-statuses/{statusId:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Modify)]
    [ProducesResponseType<CaseStatusModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("modifyCaseStatus")]
    public async Task<ActionResult<CaseStatusModel>> ModifyAsync(
        [FromRoute] Guid statusId,
        [FromBody] ModifyCaseStatus command,
        CancellationToken cancellation = default)
    {
        var model = await _caseStatusService.UpdateAsync(statusId, command, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    #endregion

    #region Delete

    [HttpDelete("workflow/case-statuses/{statusId:guid}")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteCaseStatus")]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid statusId,
        CancellationToken cancellation = default)
    {
        var deleted = await _caseStatusService.DeleteAsync(statusId, cancellation);
        return deleted ? NoContent() : NotFound();
    }

    #endregion

    #region Download

    [HttpGet("workflow/case-statuses/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseStatuses")]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectCaseStatuses query,
        CancellationToken cancellation)
        => await DownloadAsync(query, cancellation);

    [HttpPost("workflow/case-statuses/download")]
    [HybridAuthorize(Policies.Workflow.Cases.CaseStatus.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseStatuses_post")]
    [AliasFor("downloadCaseStatuses")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectCaseStatuses query,
        CancellationToken cancellation)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        CollectCaseStatuses query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("workflow", "case-status", query.Filter.Format, User);
        var models = await _caseStatusService.DownloadAsync(query, cancellation);
        var content = _caseStatusService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var fileName = exporter.CreateFileName();
        var contentType = exporter.GetContentType(fileName);
        return File(contentBytes, contentType, fileName);
    }

    #endregion
}
