using Microsoft.AspNetCore.Mvc;

using Shift.Service.Content;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Content API: Files")]
public class FileController : ShiftControllerBase
{
    private readonly FileService _fileService;

    public FileController(FileService fileService)
    {
        _fileService = fileService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific file
    /// </summary>
    [HttpHead("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFile")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid file, CancellationToken cancellation = default)
    {
        var exists = await _fileService.AssertAsync(file, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of files that match specific criteria
    /// </summary>
    [HttpPost("content/files/collect")]
    [HybridAuthorize(Policies.Content.Files.File.Collect)]
    [ProducesResponseType<IEnumerable<FileModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFiles")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFiles query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("content/files")]
    [HybridAuthorize(Policies.Content.Files.File.Collect)]
    [ProducesResponseType<IEnumerable<FileModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFiles_get")]
    [AliasFor("collectFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFiles query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFiles query, CancellationToken cancellation)
    {
        var models = await _fileService.CollectAsync(query, cancellation);

        var count = await _fileService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the files that match specific criteria
    /// </summary>
    [HttpPost("content/files/count")]
    [HybridAuthorize(Policies.Content.Files.File.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFiles")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFiles query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("content/files/count")]
    [HybridAuthorize(Policies.Content.Files.File.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFiles_get")]
    [AliasFor("countFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFiles query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFiles query, CancellationToken cancellation)
    {
        var count = await _fileService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of files that match specific criteria
    /// </summary>    
    [HttpPost("content/files/download")]
    [HybridAuthorize(Policies.Content.Files.File.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFiles")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFiles query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("content/files/download")]
    [HybridAuthorize(Policies.Content.Files.File.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFiles_get")]
    [AliasFor("downloadFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFiles query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFiles query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Content", "Files", query.Filter.Format, User);

        var models = await _fileService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _fileService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific file
    /// </summary>
    [HttpGet("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Retrieve)]
    [ProducesResponseType<FileModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFile")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid file, CancellationToken cancellation = default)
    {
        var model = await _fileService.RetrieveAsync(file, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of files that match specific criteria
    /// </summary>
    [HttpPost("content/files/search")]
    [HybridAuthorize(Policies.Content.Files.File.Search)]
    [ProducesResponseType<IEnumerable<FileMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFiles")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFiles query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("content/files/search")]
    [HybridAuthorize(Policies.Content.Files.File.Search)]
    [ProducesResponseType<IEnumerable<FileMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFiles_get")]
    [AliasFor("searchFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFiles query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFiles query, CancellationToken cancellation)
    {
        var matches = await _fileService.SearchAsync(query, cancellation);

        var count = await _fileService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("content/files")]
    [HybridAuthorize(Policies.Content.Files.File.Create)]
    [ProducesResponseType<FileModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createFile")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFile create, CancellationToken cancellation = default)
    {
        var created = await _fileService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: FileIdentifier {create.FileIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _fileService.RetrieveAsync(create.FileIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteFile")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid file, CancellationToken cancellation = default)
    {
        var deleted = await _fileService.DeleteAsync(file, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyFile")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid file, [FromBody] ModifyFile modify, CancellationToken cancellation = default)
    {
        var model = await _fileService.RetrieveAsync(file, cancellation);

        if (model is null)
            return NotFound($"File not found: FileIdentifier {modify.FileIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _fileService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands
}