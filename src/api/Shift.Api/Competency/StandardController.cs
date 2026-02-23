using Microsoft.AspNetCore.Mvc;

using Shift.Service.Competency;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Competency API: Standards")]
public class StandardController : ShiftControllerBase
{
    private readonly StandardService _standardService;
    private readonly IPrincipalProvider _principalProvider;

    public StandardController(StandardService standardService, IPrincipalProvider principalProvider)
    {
        _standardService = standardService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific standard
    /// </summary>
    [HttpHead("api/competency/standards/{standard:guid}")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertStandard")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid standard, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _standardService.AssertAsync(standard, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of standards that match specific criteria
    /// </summary>
    [HttpPost("api/competency/standards/collect")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<StandardModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectStandards")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectStandards query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/competency/standards")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<StandardModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectStandards_get")]
    [AliasFor("collectStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectStandards query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectStandards query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _standardService.CollectAsync(query, cancellation);

        var count = await _standardService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the standards that match specific criteria
    /// </summary>
    [HttpPost("api/competency/standards/count")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countStandards")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountStandards query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/competency/standards/count")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countStandards_get")]
    [AliasFor("countStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountStandards query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountStandards query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _standardService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of standards that match specific criteria
    /// </summary>    
    [HttpPost("api/competency/standards/download")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadStandards")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectStandards query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/competency/standards/download")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadStandards_get")]
    [AliasFor("downloadStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectStandards query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectStandards query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Competency", "Standards", query.Filter.Format, User);

        var models = await _standardService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _standardService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific standard
    /// </summary>
    [HttpGet("api/competency/standards/{standard:guid}")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<StandardModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveStandard")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid standard, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _standardService.RetrieveAsync(standard, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of standards that match specific criteria
    /// </summary>
    [HttpPost("api/competency/standards/search")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<StandardMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchStandards")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchStandards query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/competency/standards/search")]
    [HybridPermission("competency/standards", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<StandardMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchStandards_get")]
    [AliasFor("searchStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchStandards query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchStandards query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _standardService.SearchAsync(query, cancellation);

        var count = await _standardService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}