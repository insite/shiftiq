using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Directory API: People")]
public class PersonController : ShiftControllerBase
{
    private readonly PersonService _personService;

    public PersonController(PersonService personService)
    {
        _personService = personService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific person
    /// </summary>
    [HttpHead("directory/people/{person:guid}")]
    [HybridAuthorize(Policies.Directory.People.Person.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertPerson")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid person, CancellationToken cancellation = default)
    {
        var exists = await _personService.AssertAsync(person, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of people that match specific criteria
    /// </summary>
    [HttpPost("directory/people/collect")]
    [HybridAuthorize(Policies.Directory.People.Person.Collect)]
    [ProducesResponseType<IEnumerable<PersonModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPeople")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectPeople query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("directory/people")]
    [HybridAuthorize(Policies.Directory.People.Person.Collect)]
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
        var models = await _personService.CollectAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the people that match specific criteria
    /// </summary>
    [HttpPost("directory/people/count")]
    [HybridAuthorize(Policies.Directory.People.Person.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPeople")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountPeople query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("directory/people/count")]
    [HybridAuthorize(Policies.Directory.People.Person.Count)]
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
        var count = await _personService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of people that match specific criteria
    /// </summary>    
    [HttpPost("directory/people/download")]
    [HybridAuthorize(Policies.Directory.People.Person.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPeople")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPeople query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("directory/people/download")]
    [HybridAuthorize(Policies.Directory.People.Person.Download)]
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
    [HttpGet("directory/people/{person:guid}")]
    [HybridAuthorize(Policies.Directory.People.Person.Retrieve)]
    [ProducesResponseType<PersonModel>(StatusCodes.Status200OK)]
    [EndpointName("retrievePerson")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid person, CancellationToken cancellation = default)
    {
        var model = await _personService.RetrieveAsync(person, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of people that match specific criteria
    /// </summary>
    [HttpPost("directory/people/search")]
    [HybridAuthorize(Policies.Directory.People.Person.Search)]
    [ProducesResponseType<IEnumerable<PersonMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPeople")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("directory/people/search")]
    [HybridAuthorize(Policies.Directory.People.Person.Search)]
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
        var matches = await _personService.SearchAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}