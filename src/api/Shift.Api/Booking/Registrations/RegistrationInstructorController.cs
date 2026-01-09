using Microsoft.AspNetCore.Mvc;

using Shift.Service.Booking;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Booking API: Registrations")]
public class RegistrationInstructorController : ShiftControllerBase
{
    private readonly RegistrationInstructorService _registrationInstructorService;

    public RegistrationInstructorController(RegistrationInstructorService registrationInstructorService)
    {
        _registrationInstructorService = registrationInstructorService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific registration instructor
    /// </summary>
    [HttpHead("booking/registrations-instructors/{registration:guid}/{instructor:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertRegistrationInstructor")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid instructor, [FromRoute] Guid registration, CancellationToken cancellation = default)
    {
        var exists = await _registrationInstructorService.AssertAsync(instructor, registration, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of registration instructors that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-instructors/collect")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationInstructorModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrationInstructors")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-instructors")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationInstructorModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrationInstructors_get")]
    [AliasFor("collectRegistrationInstructors")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectRegistrationInstructors query, CancellationToken cancellation)
    {
        var models = await _registrationInstructorService.CollectAsync(query, cancellation);

        var count = await _registrationInstructorService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the registration instructors that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-instructors/count")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrationInstructors")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-instructors/count")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrationInstructors_get")]
    [AliasFor("countRegistrationInstructors")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountRegistrationInstructors query, CancellationToken cancellation)
    {
        var count = await _registrationInstructorService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of registration instructors that match specific criteria
    /// </summary>    
    [HttpPost("booking/registrations-instructors/download")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrationInstructors")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-instructors/download")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrationInstructors_get")]
    [AliasFor("downloadRegistrationInstructors")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectRegistrationInstructors query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Booking", "RegistrationInstructors", query.Filter.Format, User);

        var models = await _registrationInstructorService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _registrationInstructorService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific registration instructor
    /// </summary>
    [HttpGet("booking/registrations-instructors/{registration:guid}/{instructor:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Retrieve)]
    [ProducesResponseType<RegistrationInstructorModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveRegistrationInstructor")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid instructor, [FromRoute] Guid registration, CancellationToken cancellation = default)
    {
        var model = await _registrationInstructorService.RetrieveAsync(instructor, registration, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of registration instructors that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-instructors/search")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Search)]
    [ProducesResponseType<IEnumerable<RegistrationInstructorMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrationInstructors")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-instructors/search")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationInstructor.Search)]
    [ProducesResponseType<IEnumerable<RegistrationInstructorMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrationInstructors_get")]
    [AliasFor("searchRegistrationInstructors")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchRegistrationInstructors query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchRegistrationInstructors query, CancellationToken cancellation)
    {
        var matches = await _registrationInstructorService.SearchAsync(query, cancellation);

        var count = await _registrationInstructorService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}