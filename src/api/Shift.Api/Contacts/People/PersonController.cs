using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Contacts API: People")]
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
    [HttpHead("api/contacts/people/{person:guid}")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertPerson")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid person, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _personService.AssertAsync(person, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of people that match specific criteria
    /// </summary>
    [HttpPost("api/contacts/people/collect")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("collectPeople")]
    public async Task<ActionResult<IEnumerable<PersonModel>>> PostCollectAsync([FromBody] CollectPeople query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/contacts/people")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("collectPeople_get")]
    [AliasFor("collectPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<PersonModel>>> GetCollectAsync([FromQuery] CollectPeople query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<PersonModel>>> CollectAsync(CollectPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _personService.CollectAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the people that match specific criteria
    /// </summary>
    [HttpPost("api/contacts/people/count")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("countPeople")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountPeople query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/contacts/people/count")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("countPeople_get")]
    [AliasFor("countPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountPeople query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _personService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of people that match specific criteria
    /// </summary>    
    [HttpPost("api/contacts/people/download")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPeople")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPeople query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/contacts/people/download")]
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
    [HttpGet("api/contacts/people/{person:guid}")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [ProducesResponseType(typeof(PersonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrievePerson")]
    public async Task<ActionResult<PersonModel>> RetrieveAsync([FromRoute] Guid person, CancellationToken cancellation = default)
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
    [HttpPost("api/contacts/people/search")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("searchPeople")]
    public async Task<ActionResult<IEnumerable<PersonMatch>>> PostSearchAsync([FromBody] SearchPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/contacts/people/search")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("searchPeople_get")]
    [AliasFor("searchPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<PersonMatch>>> GetSearchAsync([FromQuery] SearchPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<PersonMatch>>> SearchAsync(SearchPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _personService.SearchAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}