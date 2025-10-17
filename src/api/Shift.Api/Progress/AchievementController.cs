using Microsoft.AspNetCore.Mvc;

using Shift.Service.Achievement;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Achievements")]
public class AchievementController : ControllerBase
{
    private readonly AchievementService _achievementService;

    public AchievementController(AchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpHead("progress/achievements/{achievement:guid}")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid achievement, CancellationToken cancellation = default)
    {
        var exists = await _achievementService.AssertAsync(achievement, cancellation);

        return Ok(exists);
    }

    [HttpGet("progress/achievements/{achievement:guid}")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Retrieve)]
    [ProducesResponseType(typeof(AchievementModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<AchievementModel>> RetrieveAsync([FromRoute] Guid achievement, CancellationToken cancellation = default)
    {
        var model = await _achievementService.RetrieveAsync(achievement, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("progress/achievements/count")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountAchievements query, CancellationToken cancellation = default)
    {
        var count = await _achievementService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("progress/achievements")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Collect)]
    [ProducesResponseType(typeof(IEnumerable<AchievementModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AchievementModel>>> CollectAsync([FromQuery] CollectAchievements query, CancellationToken cancellation = default)
    {
        var models = await _achievementService.CollectAsync(query, cancellation);

        var count = await _achievementService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("progress/achievements/search")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Search)]
    [ProducesResponseType(typeof(IEnumerable<AchievementMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AchievementMatch>>> GetSearchAsync([FromQuery] SearchAchievements query, CancellationToken cancellation = default)
    {
        var matches = await _achievementService.SearchAsync(query, cancellation);

        var count = await _achievementService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("progress/achievements/search")]
    [HybridAuthorize(Policies.Progress.Achievements.Achievement.Search)]
    [ProducesResponseType(typeof(IEnumerable<AchievementMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AchievementMatch>>> SearchAsync([FromBody] SearchAchievements query, CancellationToken cancellation = default)
    {
        var matches = await _achievementService.SearchAsync(query, cancellation);

        var count = await _achievementService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    // This entity is a current-state projection of an aggregate event/change stream. This is the reason you do not see
    // any controller actions implemented here to create, modify, or delete this entity. Data changes to this entity are 
    // permitted only using Timeline commands.
}