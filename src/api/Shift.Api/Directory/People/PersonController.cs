using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Directory API: People")]
public class PersonController : ShiftControllerBase
{
    private readonly PersonService _personService;
    private readonly IPrincipalProvider _principalProvider;

    public PersonController(PersonService personService, IPrincipalProvider principalProvider)
    {
        _personService = personService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific person
    /// </summary>
    [HttpHead("api/directory/people/{person:guid}")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertPerson")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid person, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _personService.AssertAsync(person, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of people that match specific criteria
    /// </summary>
    [HttpPost("api/directory/people/collect")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PersonModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPeople")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectPeople query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/directory/people")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PersonModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPeople_get")]
    [AliasFor("collectPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectPeople query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _personService.CollectAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the people that match specific criteria
    /// </summary>
    [HttpPost("api/directory/people/count")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPeople")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountPeople query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/directory/people/count")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPeople_get")]
    [AliasFor("countPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountPeople query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _personService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of people that match specific criteria
    /// </summary>    
    [HttpPost("api/directory/people/download")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPeople")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPeople query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/directory/people/download")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPeople_get")]
    [AliasFor("downloadPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectPeople query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Directory", "People", query.Filter.Format, User);

        var models = await _personService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _personService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific person
    /// </summary>
    [HttpGet("api/directory/people/{person:guid}")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<PersonModel>(StatusCodes.Status200OK)]
    [EndpointName("retrievePerson")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid person, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _personService.RetrieveAsync(person, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of people that match specific criteria
    /// </summary>
    [HttpPost("api/directory/people/search")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PersonMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPeople")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/directory/people/search")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PersonMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPeople_get")]
    [AliasFor("searchPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _personService.SearchAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}