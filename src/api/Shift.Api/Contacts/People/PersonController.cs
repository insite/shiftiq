using InSite.Application.Files.Read;

using Microsoft.AspNetCore.Mvc;

using Shift.Service.Content;

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
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the result set to people modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
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

    private async Task<PersonModel[]> CollectAsync(CollectPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _personService.CollectAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return models;
    }

    /// <summary>
    /// Counts the people that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the count to people modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
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

        return new CountResult(count);
    }

    /// <summary>
    /// Downloads the list of people that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to download only people modified within a specific window,
    /// which is useful for incremental integrations that already hold most of the data. Both parameters accept
    /// ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00". LastChangeTimeSince is inclusive;
    /// LastChangeTimeBefore is exclusive. When polling for deltas, subtract a small overlap (for example five
    /// minutes) from the previous poll time to allow for projection lag.
    /// </remarks>
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

        return model;
    }

    /// <summary>
    /// Searches for the list of people that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the result set to people modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
    [HttpPost("api/contacts/people/search")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("searchPeople")]
    public async Task<ActionResult<PersonMatch[]>> PostSearchAsync([FromBody] SearchPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/contacts/people/search")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("searchPeople_get")]
    [AliasFor("searchPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<PersonMatch[]>> GetSearchAsync([FromQuery] SearchPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<PersonMatch[]> SearchAsync(SearchPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _personService.SearchAsync(query, cancellation);

        var count = await _personService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return matches;
    }

    #endregion Queries

    #region Import

    public class ImportResultItem
    {
        public required string PersonCode { get; init; }
        public required ImportPersonResult.StatusEnum Status { get; init; }
        public Guid? PendingPersonId { get; init; }
        public Guid? UserId { get; init; }
        public ValidationFailure? Failure { get; init; }
    }

    public class ImportResult
    {
        public required Guid? ReportFileId { get; init; }
        public required string? ReportFileName { get; init; }
        public required ImportResultItem[] ImportedPeople { get; init; }
    }

    [HttpPost("api/contacts/people/import")]
    [HybridPermission("directory/people", DataAccess.Update)]
    [EndpointName("importPeople")]
    public async Task<ActionResult<ImportResult>> ImportAsync(
        IPersonImporter importer,
        IPersonImportReporter reporter,
        OrganizationService organizationService,
        OrganizationAdapter organizationAdapter,
        ImportPerson[] imports
        )
    {
        var principal = _principalProvider.GetPrincipal();
        var submittedBy = principal.UserId;
        var submittedByName = principal.Name;

        var organizationId = principal.Organization.Identifier;
        var organization = await organizationService.RetrieveAsync(organizationId) ?? throw new ArgumentNullException($"Organization {organizationId} is not found");
        var organizationData = organizationAdapter.ToData(organization);
        var fullNamePolicy = organizationData.Toolkits?.Contacts?.FullNamePolicy;
        var claimGroupNames = organizationData.Toolkits?.Contacts?.ImportReportGroupNames;
        var timeZone = organizationData.TimeZone.Id;

        var result = await importer.ImportAsync(organizationId, fullNamePolicy, timeZone, submittedBy, submittedByName, imports);
        var file = await reporter.SaveReportAsync(organizationId, submittedBy, timeZone, claimGroupNames, result, true);

        return new ImportResult
        {
            ReportFileId = file?.FileIdentifier,
            ReportFileName = file?.FileName,
            ImportedPeople = result.Select(x => new ImportResultItem
            {
                PersonCode = x.Input.PersonCode,
                Status = x.Status,
                PendingPersonId = x.PendingPersonId,
                UserId = x.UserId,
                Failure = x.Failure
            })
            .ToArray(),
        };
    }

    public class ImportReport
    {
        public required Guid FileId { get; init; }
        public required string FileName { get; init; }
        public required string DocumentName { get; init; }
        public required DateTimeOffset FileUploaded { get; init; }
        public required int FileSize { get; init; }
        public required Guid UserId { get; init; }
        public required string UserFullName { get; init; }
    }

    public class SearchImportReports : Query<IEnumerable<ImportReport>>
    {
    }

    [HttpPost("api/contacts/people/search-import-report")]
    [HybridPermission("directory/people", DataAccess.Read)]
    [EndpointName("searchImportReport")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<ImportReport[]>> SearchImportReportAsync(FileService fileService, SearchImportReports criteria, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var searchFiles = new SearchFiles { OrganizationId = principal.OrganizationId, ObjectId = principal.OrganizationId, FileTag = FileTag.PersonImport };
        searchFiles.Filter.Page = criteria.Filter.Page;
        searchFiles.Filter.Sort = nameof(FileEntity.FileUploaded) + " desc";

        var files = await fileService.SearchAsync(searchFiles, cancellation);
        var count = await fileService.CountAsync(searchFiles, cancellation);

        Response.AddPagination(searchFiles.Filter, count);

        return files.Select(x => new ImportReport
        {
            FileId = x.FileId,
            FileName = x.FileName,
            DocumentName = x.DocumentName,
            FileUploaded = x.FileUploaded,
            FileSize = x.FileSize,
            UserId = x.UserId,
            UserFullName = x.UserFullName
        })
        .ToArray();
    }

    #endregion
}