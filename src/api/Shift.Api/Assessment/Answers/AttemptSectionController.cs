using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class AttemptSectionController : ShiftControllerBase
{
    private readonly AttemptSectionService _attemptSectionService;

    public AttemptSectionController(AttemptSectionService attemptSectionService)
    {
        _attemptSectionService = attemptSectionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific attempt section
    /// </summary>
    [HttpHead("assessment/attempts-sections/{attempt:guid}/{section:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAttemptSection")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, [FromRoute] int sectionIndex, CancellationToken cancellation = default)
    {
        var exists = await _attemptSectionService.AssertAsync(attempt, sectionIndex, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of attempt sections that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-sections/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Collect)]
    [ProducesResponseType<IEnumerable<AttemptSectionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptSections")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAttemptSections query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-sections")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Collect)]
    [ProducesResponseType<IEnumerable<AttemptSectionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptSections_get")]
    [AliasFor("collectAttemptSections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAttemptSections query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAttemptSections query, CancellationToken cancellation)
    {
        var models = await _attemptSectionService.CollectAsync(query, cancellation);

        var count = await _attemptSectionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the attempt sections that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-sections/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptSections")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAttemptSections query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-sections/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptSections_get")]
    [AliasFor("countAttemptSections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAttemptSections query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAttemptSections query, CancellationToken cancellation)
    {
        var count = await _attemptSectionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of attempt sections that match specific criteria
    /// </summary>    
    [HttpPost("assessment/attempts-sections/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptSections")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAttemptSections query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-sections/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptSections_get")]
    [AliasFor("downloadAttemptSections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAttemptSections query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAttemptSections query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "AttemptSections", query.Filter.Format, User);

        var models = await _attemptSectionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _attemptSectionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific attempt section
    /// </summary>
    [HttpGet("assessment/attempts-sections/{attempt:guid}/{section:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Retrieve)]
    [ProducesResponseType<AttemptSectionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAttemptSection")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, [FromRoute] int sectionIndex, CancellationToken cancellation = default)
    {
        var model = await _attemptSectionService.RetrieveAsync(attempt, sectionIndex, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of attempt sections that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-sections/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Search)]
    [ProducesResponseType<IEnumerable<AttemptSectionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptSections")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAttemptSections query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-sections/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSection.Search)]
    [ProducesResponseType<IEnumerable<AttemptSectionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptSections_get")]
    [AliasFor("searchAttemptSections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAttemptSections query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAttemptSections query, CancellationToken cancellation)
    {
        var matches = await _attemptSectionService.SearchAsync(query, cancellation);

        var count = await _attemptSectionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}