using Microsoft.AspNetCore.Mvc;

using Shift.Service.Progress;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Achievements")]
public class AchievementController : ShiftControllerBase
{
    private readonly AchievementService _achievementService;

    public AchievementController(AchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific achievement
    /// </summary>
    [HttpHead("progress/achievements/{achievement:guid}")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAchievement")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid achievement, CancellationToken cancellation = default)
    {
        var exists = await _achievementService.AssertAsync(achievement, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of achievements that match specific criteria
    /// </summary>
    [HttpPost("progress/achievements/collect")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Collect)]
    [ProducesResponseType<IEnumerable<AchievementModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAchievements")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAchievements query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("progress/achievements")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Collect)]
    [ProducesResponseType<IEnumerable<AchievementModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAchievements_get")]
    [AliasFor("collectAchievements")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAchievements query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAchievements query, CancellationToken cancellation)
    {
        var models = await _achievementService.CollectAsync(query, cancellation);

        var count = await _achievementService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the achievements that match specific criteria
    /// </summary>
    [HttpPost("progress/achievements/count")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAchievements")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAchievements query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("progress/achievements/count")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAchievements_get")]
    [AliasFor("countAchievements")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAchievements query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAchievements query, CancellationToken cancellation)
    {
        var count = await _achievementService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of achievements that match specific criteria
    /// </summary>    
    [HttpPost("progress/achievements/download")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAchievements")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAchievements query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("progress/achievements/download")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAchievements_get")]
    [AliasFor("downloadAchievements")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAchievements query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAchievements query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Progress", "Achievements", query.Filter.Format, User);

        var models = await _achievementService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _achievementService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific achievement
    /// </summary>
    [HttpGet("progress/achievements/{achievement:guid}")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Retrieve)]
    [ProducesResponseType<AchievementModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAchievement")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid achievement, CancellationToken cancellation = default)
    {
        var model = await _achievementService.RetrieveAsync(achievement, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of achievements that match specific criteria
    /// </summary>
    [HttpPost("progress/achievements/search")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Search)]
    [ProducesResponseType<IEnumerable<AchievementMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAchievements")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAchievements query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("progress/achievements/search")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Search)]
    [ProducesResponseType<IEnumerable<AchievementMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAchievements_get")]
    [AliasFor("searchAchievements")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAchievements query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAchievements query, CancellationToken cancellation)
    {
        var matches = await _achievementService.SearchAsync(query, cancellation);

        var count = await _achievementService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}