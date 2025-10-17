using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Content;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Content API: Files")]
public class FileActivityController : ControllerBase
{
    private readonly FileActivityService _fileActivityService;

    public FileActivityController(FileActivityService fileActivityService)
    {
        _fileActivityService = fileActivityService;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific file activity
    /// </summary>
    [HttpHead("content/files-activities/{activity:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFileActivity")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid activity,
        CancellationToken cancellation = default)
    {
        var exists = await _fileActivityService.AssertAsync(activity, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of file activities that match specific criteria
    /// </summary>
    [HttpPost("content/files-activities/collect")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Collect)]
    [ProducesResponseType<IEnumerable<FileActivityModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileActivities")]
    public async Task<ActionResult<IEnumerable<FileActivityModel>>> PostCollectAsync(
        [FromBody] CollectFileActivities query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpGet("content/files-activities")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Collect)]
    [ProducesResponseType<IEnumerable<FileActivityModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileActivities_get")]
    [AliasFor("collectFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<FileActivityModel>>> GetCollectAsync(
        [FromQuery] CollectFileActivities query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<FileActivityModel>>> CollectAsync(
        CollectFileActivities query,
        CancellationToken cancellation)
    {
        var models = await _fileActivityService.CollectAsync(query, cancellation);

        var count = await _fileActivityService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the file activities that match specific criteria
    /// </summary>
    [HttpPost("content/files-activities/count")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileActivities")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountFileActivities query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpGet("content/files-activities/count")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileActivities_get")]
    [AliasFor("countFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountFileActivities query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(
        CountFileActivities query,
        CancellationToken cancellation)
    {
        var count = await _fileActivityService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of file activities that match specific criteria
    /// </summary>    
    [HttpPost("content/files-activities/download")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileActivities")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectFileActivities query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    [HttpGet("content/files-activities/download")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileActivities_get")]
    [AliasFor("downloadFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectFileActivities query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        CollectFileActivities query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Content", "FileActivities", query.Filter.Format, User);

        var models = await _fileActivityService.DownloadAsync(query, cancellation);

        var content = _fileActivityService.Serialize(models, exporter.GetFileFormat());

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific file activity
    /// </summary>
    [HttpGet("content/files-activities/{activity:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Retrieve)]
    [ProducesResponseType<FileActivityModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFileActivity")]
    public async Task<ActionResult<FileActivityModel>> RetrieveAsync(
        [FromRoute] Guid activity,
        CancellationToken cancellation = default)
    {
        var model = await _fileActivityService.RetrieveAsync(activity, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Search for the list of file activities that match specific criteria
    /// </summary>
    [HttpPost("content/files-activities/search")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Search)]
    [ProducesResponseType<IEnumerable<FileActivityMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileActivities")]
    public async Task<ActionResult<IEnumerable<FileActivityMatch>>> PostSearchAsync(
        [FromBody] SearchFileActivities query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpGet("content/files-activities/search")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Search)]
    [ProducesResponseType<IEnumerable<FileActivityMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileActivities_get")]
    [AliasFor("searchFileActivities")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<FileActivityMatch>>> GetSearchAsync(
        [FromQuery] SearchFileActivities query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<FileActivityMatch>>> SearchAsync(
        SearchFileActivities query,
        CancellationToken cancellation)
    {
        var matches = await _fileActivityService.SearchAsync(query, cancellation);

        var count = await _fileActivityService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("content/files-activities")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Create)]
    [ProducesResponseType<FileActivityModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createFileActivity")]
    public async Task<ActionResult<FileActivityModel>> CreateAsync(
        [FromBody] CreateFileActivity create,
        CancellationToken cancellation = default)
    {
        var created = await _fileActivityService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: ActivityIdentifier {create.ActivityIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _fileActivityService.RetrieveAsync(create.ActivityIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("content/files-activities/{activity:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteFileActivity")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid activity,
        CancellationToken cancellation = default)
    {
        var deleted = await _fileActivityService.DeleteAsync(activity, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("content/files-activities/{activity:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileActivity.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyFileActivity")]
    public async Task<IActionResult> ModifyAsync(
        [FromBody] ModifyFileActivity modify,
        CancellationToken cancellation = default)
    {
        var model = await _fileActivityService.RetrieveAsync(modify.ActivityIdentifier, cancellation);

        if (model is null)
            return NotFound($"FileActivity not found: ActivityIdentifier {modify.ActivityIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _fileActivityService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands
}