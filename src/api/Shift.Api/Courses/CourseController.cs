using Microsoft.AspNetCore.Mvc;

using Shift.Service.Learning;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Courses API: Courses")]
public class CourseController : ShiftControllerBase
{
    private readonly CourseService _courseService;
    private readonly IPrincipalProvider _principalProvider;

    public CourseController(CourseService courseService, IPrincipalProvider principalProvider)
    {
        _courseService = courseService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific course
    /// </summary>
    [HttpHead("api/courses/{course:guid}")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertCourse")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid course, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _courseService.AssertAsync(course, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of courses that match specific criteria
    /// </summary>
    [HttpPost("api/courses/collect")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [EndpointName("collectCourses")]
    public async Task<ActionResult<IEnumerable<CourseModel>>> PostCollectAsync([FromBody] CollectCourses query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/courses")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [EndpointName("collectCourses_get")]
    [AliasFor("collectCourses")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<CourseModel>>> GetCollectAsync([FromQuery] CollectCourses query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<CourseModel>>> CollectAsync(CollectCourses query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _courseService.CollectAsync(query, principal.TimeZone, cancellation);

        var count = await _courseService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the courses that match specific criteria
    /// </summary>
    [HttpPost("api/courses/count")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [EndpointName("countCourses")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountCourses query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/courses/count")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [EndpointName("countCourses_get")]
    [AliasFor("countCourses")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountCourses query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountCourses query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _courseService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of courses that match specific criteria
    /// </summary>    
    [HttpPost("api/courses/download")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCourses")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCourses query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/courses/download")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCourses_get")]
    [AliasFor("downloadCourses")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectCourses query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectCourses query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Learning", "Courses", query.Filter.Format, User);

        var models = await _courseService
            .DownloadAsync(query, principal.TimeZone, cancellation)
            .ToListAsync(cancellation);

        var content = _courseService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific course
    /// </summary>
    [HttpGet("api/courses/{course:guid}")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [ProducesResponseType(typeof(CourseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveCourse")]
    public async Task<ActionResult<CourseModel>> RetrieveAsync([FromRoute] Guid course, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _courseService.RetrieveAsync(course, principal.TimeZone, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of courses that match specific criteria
    /// </summary>
    [HttpPost("api/courses/search")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [EndpointName("searchCourses")]
    public async Task<ActionResult<IEnumerable<CourseMatch>>> PostSearchAsync([FromBody] SearchCourses query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/courses/search")]
    [HybridPermission("learning/courses", DataAccess.Read)]
    [EndpointName("searchCourses_get")]
    [AliasFor("searchCourses")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<CourseMatch>>> GetSearchAsync([FromQuery] SearchCourses query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<CourseMatch>>> SearchAsync(SearchCourses query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _courseService.SearchAsync(query, cancellation);

        var count = await _courseService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}