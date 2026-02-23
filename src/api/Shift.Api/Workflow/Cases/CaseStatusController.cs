using System.Text;

using Microsoft.AspNetCore.Mvc;

using Shift.Service.Cases;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Cases")]
public class CaseStatusController : ControllerBase
{
    private readonly ILogger<CaseStatusController> _logger;
    private readonly ReleaseSettings _releaseSettings;
    private readonly DatabaseSettings _databaseSettings;
    private readonly TCaseStatusService _caseStatusService;
    private readonly IPrincipalProvider _principalProvider;

    public CaseStatusController(
        ILogger<CaseStatusController> logger,
        ReleaseSettings releaseSettings,
        DatabaseSettings databaseSettings,
        TCaseStatusService caseStatusService,
        IPrincipalProvider principalProvider)
    {
        _logger = logger;
        _releaseSettings = releaseSettings;
        _databaseSettings = databaseSettings;
        _caseStatusService = caseStatusService;
        _principalProvider = principalProvider;
    }

    #region Retrieve

    [HttpHead("api/workflow/cases-statuses/{statusId:guid}")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCaseStatus")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid statusId,
        CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var organizationId = _principalProvider.GetOrganizationId(principal);
        var exists = await _caseStatusService.AssertAsync(statusId, organizationId, cancellation);
        return exists ? Ok() : NotFound();
    }

    [HttpGet("api/workflow/cases-statuses/{statusId:guid}")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
    [ProducesResponseType<CaseStatusModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCaseStatus")]
    [ActionName("RetrieveAsync")]
    public async Task<ActionResult<CaseStatusModel>> RetrieveAsync(
        [FromRoute] Guid statusId,
        CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var model = await _caseStatusService.RetrieveAsync(statusId, cancellation);
        if (model == null)
            return NotFound();
        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();
        return Ok(model);
    }

    #endregion

    #region Count

    [HttpGet("api/workflow/cases-statuses/count")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCaseStatuses")]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountCaseStatuses query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpPost("api/workflow/cases-statuses/count")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);
        var count = await _caseStatusService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    #endregion

    #region Collect

    [HttpGet("api/workflow/cases-statuses")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
    [ProducesResponseType(typeof(IEnumerable<CaseStatusModel>), StatusCodes.Status200OK)]
    [EndpointName("collectCaseStatuses")]
    public async Task<ActionResult<IEnumerable<CaseStatusModel>>> GetCollectAsync(
        [FromQuery] CollectCaseStatuses query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpPost("api/workflow/cases-statuses/collect")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);
        var models = await _caseStatusService.CollectAsync(query, cancellation);
        var count = await _caseStatusService.CountAsync(query, cancellation);
        Response.AddPagination(query.Filter, count);
        return Ok(models);
    }

    #endregion

    #region Search

    [HttpGet("api/workflow/cases-statuses/search")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
    [ProducesResponseType(typeof(IEnumerable<CaseStatusMatch>), StatusCodes.Status200OK)]
    [EndpointName("searchCaseStatuses")]
    public async Task<ActionResult<IEnumerable<CaseStatusMatch>>> GetSearchAsync(
        [FromQuery] SearchCaseStatuses query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpPost("api/workflow/cases-statuses/search")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);
        var matches = await _caseStatusService.SearchAsync(query, cancellation);
        var count = await _caseStatusService.CountAsync(query, cancellation);
        Response.AddPagination(query.Filter, count);
        return Ok(matches);
    }

    #endregion

    #region Create

    [HttpPost("api/workflow/cases-statuses")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Update)]
    [ProducesResponseType<CaseStatusModel>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("createCaseStatus")]
    public async Task<ActionResult<CaseStatusModel>> CreateAsync(
        [FromBody] CreateCaseStatus command,
        CancellationToken cancellation = default)
    {
        var organizationId = _principalProvider.OrganizationId;

        var model = await _caseStatusService.CreateAsync(command, organizationId, cancellation);

        return CreatedAtAction(
            nameof(RetrieveAsync),
            new { statusId = model?.StatusId },
            model);
    }

    #endregion

    #region Modify

    [HttpPut("api/workflow/cases-statuses/{statusId:guid}")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Update)]
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

    [HttpDelete("api/workflow/cases-statuses/{statusId:guid}")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteCaseStatus")]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid statusId,
        CancellationToken cancellation = default)
    {
        var organizationId = _principalProvider.OrganizationId;

        var deleted = await _caseStatusService.DeleteAsync(statusId, organizationId, cancellation);

        return deleted ? NoContent() : NotFound();
    }

    #endregion

    #region Download

    [HttpGet("api/workflow/cases-statuses/download")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCaseStatuses")]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectCaseStatuses query,
        CancellationToken cancellation)
        => await DownloadAsync(query, cancellation);

    [HttpPost("api/workflow/cases-statuses/download")]
    [HybridPermission("workflow/cases-statuses", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);
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
