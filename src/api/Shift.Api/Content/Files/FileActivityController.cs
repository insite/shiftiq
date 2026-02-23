using Microsoft.AspNetCore.Mvc;

using Shift.Service.Content;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Content API: Files")]
public class FileActivityController : ShiftControllerBase
{
    private readonly FileActivityService _fileActivityService;
    private readonly IPrincipalProvider _principalProvider;

    public FileActivityController(FileActivityService fileActivityService, IPrincipalProvider principalProvider)
    {
        _fileActivityService = fileActivityService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific file activity
    /// </summary>
    [HttpHead("api/content/files-activities/{activity:guid}")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFileActivity")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid activity, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _fileActivityService.AssertAsync(activity, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of file activities that match specific criteria
    /// </summary>
    [HttpPost("api/content/files-activities/collect")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FileActivityModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileActivities")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFileActivities query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/content/files-activities")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FileActivityModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileActivities_get")]
    [AliasFor("collectFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFileActivities query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFileActivities query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _fileActivityService.CollectAsync(query, cancellation);

        var count = await _fileActivityService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the file activities that match specific criteria
    /// </summary>
    [HttpPost("api/content/files-activities/count")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileActivities")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFileActivities query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/content/files-activities/count")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileActivities_get")]
    [AliasFor("countFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFileActivities query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFileActivities query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _fileActivityService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of file activities that match specific criteria
    /// </summary>    
    [HttpPost("api/content/files-activities/download")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileActivities")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFileActivities query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/content/files-activities/download")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileActivities_get")]
    [AliasFor("downloadFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFileActivities query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFileActivities query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Content", "FileActivities", query.Filter.Format, User);

        var models = await _fileActivityService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _fileActivityService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific file activity
    /// </summary>
    [HttpGet("api/content/files-activities/{activity:guid}")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<FileActivityModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFileActivity")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid activity, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _fileActivityService.RetrieveAsync(activity, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of file activities that match specific criteria
    /// </summary>
    [HttpPost("api/content/files-activities/search")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FileActivityMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileActivities")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFileActivities query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/content/files-activities/search")]
    [HybridPermission("content/files-activities", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FileActivityMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileActivities_get")]
    [AliasFor("searchFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFileActivities query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFileActivities query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _fileActivityService.SearchAsync(query, cancellation);

        var count = await _fileActivityService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("api/content/files-activities")]
    [HybridPermission("content/files-activities", DataAccess.Update)]
    [ProducesResponseType<FileActivityModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createFileActivity")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFileActivity create, CancellationToken cancellation = default)
    {
        var created = await _fileActivityService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: ActivityIdentifier {create.ActivityId}. You cannot insert a duplicate object with the same primary key.");

        var model = await _fileActivityService.RetrieveAsync(create.ActivityId, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("api/content/files-activities/{activity:guid}")]
    [HybridPermission("content/files-activities", DataAccess.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteFileActivity")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid activity, CancellationToken cancellation = default)
    {
        var deleted = await _fileActivityService.DeleteAsync(activity, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("api/content/files-activities/{activity:guid}")]
    [HybridPermission("content/files-activities", DataAccess.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyFileActivity")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid activity, [FromBody] ModifyFileActivity modify, CancellationToken cancellation = default)
    {
        var model = await _fileActivityService.RetrieveAsync(activity, cancellation);

        if (model is null)
            return NotFound($"FileActivity not found: ActivityIdentifier {modify.ActivityId}. You cannot modify an object that is not in the database.");

        var modified = await _fileActivityService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands
}