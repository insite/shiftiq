using Microsoft.AspNetCore.Mvc;

using Shift.Service.Content;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Content API: Files")]
public class FileClaimController : ShiftControllerBase
{
    private readonly FileClaimService _fileClaimService;

    public FileClaimController(FileClaimService fileClaimService)
    {
        _fileClaimService = fileClaimService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific file claim
    /// </summary>
    [HttpHead("content/files-claims/{claim:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFileClaim")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid claim, CancellationToken cancellation = default)
    {
        var exists = await _fileClaimService.AssertAsync(claim, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of file claims that match specific criteria
    /// </summary>
    [HttpPost("content/files-claims/collect")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Collect)]
    [ProducesResponseType<IEnumerable<FileClaimModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileClaims")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFileClaims query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("content/files-claims")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Collect)]
    [ProducesResponseType<IEnumerable<FileClaimModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileClaims_get")]
    [AliasFor("collectFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFileClaims query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFileClaims query, CancellationToken cancellation)
    {
        var models = await _fileClaimService.CollectAsync(query, cancellation);

        var count = await _fileClaimService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the file claims that match specific criteria
    /// </summary>
    [HttpPost("content/files-claims/count")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileClaims")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFileClaims query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("content/files-claims/count")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileClaims_get")]
    [AliasFor("countFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFileClaims query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFileClaims query, CancellationToken cancellation)
    {
        var count = await _fileClaimService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of file claims that match specific criteria
    /// </summary>    
    [HttpPost("content/files-claims/download")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileClaims")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFileClaims query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("content/files-claims/download")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileClaims_get")]
    [AliasFor("downloadFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFileClaims query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFileClaims query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Content", "FileClaims", query.Filter.Format, User);

        var models = await _fileClaimService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _fileClaimService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific file claim
    /// </summary>
    [HttpGet("content/files-claims/{claim:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Retrieve)]
    [ProducesResponseType<FileClaimModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFileClaim")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid claim, CancellationToken cancellation = default)
    {
        var model = await _fileClaimService.RetrieveAsync(claim, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of file claims that match specific criteria
    /// </summary>
    [HttpPost("content/files-claims/search")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Search)]
    [ProducesResponseType<IEnumerable<FileClaimMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileClaims")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFileClaims query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("content/files-claims/search")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Search)]
    [ProducesResponseType<IEnumerable<FileClaimMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileClaims_get")]
    [AliasFor("searchFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFileClaims query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFileClaims query, CancellationToken cancellation)
    {
        var matches = await _fileClaimService.SearchAsync(query, cancellation);

        var count = await _fileClaimService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("content/files-claims")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Create)]
    [ProducesResponseType<FileClaimModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createFileClaim")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFileClaim create, CancellationToken cancellation = default)
    {
        var created = await _fileClaimService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: ClaimIdentifier {create.ClaimIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _fileClaimService.RetrieveAsync(create.ClaimIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("content/files-claims/{claim:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteFileClaim")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid claim, CancellationToken cancellation = default)
    {
        var deleted = await _fileClaimService.DeleteAsync(claim, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("content/files-claims/{claim:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyFileClaim")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid claim, [FromBody] ModifyFileClaim modify, CancellationToken cancellation = default)
    {
        var model = await _fileClaimService.RetrieveAsync(claim, cancellation);

        if (model is null)
            return NotFound($"FileClaim not found: ClaimIdentifier {modify.ClaimIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _fileClaimService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands
}