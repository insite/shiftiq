using Microsoft.AspNetCore.Mvc;

using Shift.Service.Progress;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Achievements")]
public class AchievementController : ShiftControllerBase
{
    private readonly AchievementService _achievementService;
    private readonly IPrincipalProvider _principalProvider;

    public AchievementController(AchievementService achievementService, IPrincipalProvider principalProvider)
    {
        _achievementService = achievementService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific achievement
    /// </summary>
    [HttpHead("api/progress/achievements/{achievement:guid}")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAchievement")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid achievement, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _achievementService.AssertAsync(achievement, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of achievements that match specific criteria
    /// </summary>
    [HttpPost("api/progress/achievements/collect")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<AchievementModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAchievements")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAchievements query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/progress/achievements")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _achievementService.CollectAsync(query, cancellation);

        var count = await _achievementService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the achievements that match specific criteria
    /// </summary>
    [HttpPost("api/progress/achievements/count")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAchievements")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAchievements query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/progress/achievements/count")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _achievementService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of achievements that match specific criteria
    /// </summary>    
    [HttpPost("api/progress/achievements/download")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAchievements")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAchievements query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/progress/achievements/download")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

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
    [HttpGet("api/progress/achievements/{achievement:guid}")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
    [ProducesResponseType<AchievementModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAchievement")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid achievement, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _achievementService.RetrieveAsync(achievement, cancellation);

        if (model == null)
            return NotFound();

        // If the achievement exists in another organization then we could return Unauthorized here - but this would
        // reveal that the ID exists. Returning Not Found is more secure and also helps to prevent enumeration attacks.

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of achievements that match specific criteria
    /// </summary>
    [HttpPost("api/progress/achievements/search")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<AchievementMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAchievements")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAchievements query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/progress/achievements/search")]
    [HybridPermission("progress/achievements", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _achievementService.SearchAsync(query, cancellation);

        var count = await _achievementService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}