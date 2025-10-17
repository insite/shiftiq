using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Content;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Content API: Files")]
public class FileClaimController : ControllerBase
{
    private readonly FileClaimService _fileClaimService;

    public FileClaimController(FileClaimService fileClaimService)
    {
        _fileClaimService = fileClaimService;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific file claim
    /// </summary>
    [HttpHead("content/files-claims/{claim:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFileClaim")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid claim,
        CancellationToken cancellation = default)
    {
        var exists = await _fileClaimService.AssertAsync(claim, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of file claims that match specific criteria
    /// </summary>
    [HttpPost("content/files-claims/collect")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Collect)]
    [ProducesResponseType<IEnumerable<FileClaimModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileClaims")]
    public async Task<ActionResult<IEnumerable<FileClaimModel>>> PostCollectAsync(
        [FromBody] CollectFileClaims query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpGet("content/files-claims")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Collect)]
    [ProducesResponseType<IEnumerable<FileClaimModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFileClaims_get")]
    [AliasFor("collectFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<FileClaimModel>>> GetCollectAsync(
        [FromQuery] CollectFileClaims query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<FileClaimModel>>> CollectAsync(
        CollectFileClaims query,
        CancellationToken cancellation)
    {
        var models = await _fileClaimService.CollectAsync(query, cancellation);

        var count = await _fileClaimService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the file claims that match specific criteria
    /// </summary>
    [HttpPost("content/files-claims/count")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileClaims")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountFileClaims query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpGet("content/files-claims/count")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFileClaims_get")]
    [AliasFor("countFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountFileClaims query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(
        CountFileClaims query,
        CancellationToken cancellation)
    {
        var count = await _fileClaimService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of file claims that match specific criteria
    /// </summary>    
    [HttpPost("content/files-claims/download")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileClaims")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectFileClaims query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    [HttpGet("content/files-claims/download")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFileClaims_get")]
    [AliasFor("downloadFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectFileClaims query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        CollectFileClaims query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Content", "FileClaims", query.Filter.Format, User);

        var models = await _fileClaimService.DownloadAsync(query, cancellation);

        var content = _fileClaimService.Serialize(models, exporter.GetFileFormat());

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific file claim
    /// </summary>
    [HttpGet("content/files-claims/{claim:guid}")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Retrieve)]
    [ProducesResponseType<FileClaimModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFileClaim")]
    public async Task<ActionResult<FileClaimModel>> RetrieveAsync(
        [FromRoute] Guid claim,
        CancellationToken cancellation = default)
    {
        var model = await _fileClaimService.RetrieveAsync(claim, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Search for the list of file claims that match specific criteria
    /// </summary>
    [HttpPost("content/files-claims/search")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Search)]
    [ProducesResponseType<IEnumerable<FileClaimMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileClaims")]
    public async Task<ActionResult<IEnumerable<FileClaimMatch>>> PostSearchAsync(
        [FromBody] SearchFileClaims query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpGet("content/files-claims/search")]
    [HybridAuthorize(Policies.Content.Files.FileClaim.Search)]
    [ProducesResponseType<IEnumerable<FileClaimMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFileClaims_get")]
    [AliasFor("searchFileClaims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<FileClaimMatch>>> GetSearchAsync(
        [FromQuery] SearchFileClaims query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<FileClaimMatch>>> SearchAsync(
        SearchFileClaims query,
        CancellationToken cancellation)
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
    public async Task<ActionResult<FileClaimModel>> CreateAsync(
        [FromBody] CreateFileClaim create,
        CancellationToken cancellation = default)
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
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid claim,
        CancellationToken cancellation = default)
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
    public async Task<IActionResult> ModifyAsync(
        [FromBody] ModifyFileClaim modify,
        CancellationToken cancellation = default)
    {
        var model = await _fileClaimService.RetrieveAsync(modify.ClaimIdentifier, cancellation);

        if (model is null)
            return NotFound($"FileClaim not found: ClaimIdentifier {modify.ClaimIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _fileClaimService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands
}