using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Contacts API: People")]
public class PendingPersonController : ShiftControllerBase
{
    private readonly PendingPersonService _pendingPersonService;
    private readonly IPrincipalProvider _principalProvider;

    public PendingPersonController(PendingPersonService pendingPersonService, IPrincipalProvider principalProvider)
    {
        _pendingPersonService = pendingPersonService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific pending person
    /// </summary>
    [HttpHead("api/contacts/pending-people/{pending:guid}")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertPendingPerson")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid pending, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _pendingPersonService.AssertAsync(pending, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of pending people that match specific criteria
    /// </summary>
    [HttpPost("api/contacts/pending-people/collect")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [EndpointName("collectPendingPeople")]
    public async Task<ActionResult<PendingPersonModel[]>> PostCollectAsync([FromBody] CollectPendingPeople query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/contacts/pending-people")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [EndpointName("collectPendingPeople_get")]
    [AliasFor("collectPendingPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<PendingPersonModel[]>> GetCollectAsync([FromQuery] CollectPendingPeople query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<PendingPersonModel[]>> CollectAsync(CollectPendingPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _pendingPersonService.CollectAsync(query, cancellation);

        var count = await _pendingPersonService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return models;
    }

    /// <summary>
    /// Counts the pending people that match specific criteria
    /// </summary>
    [HttpPost("api/contacts/pending-people/count")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [EndpointName("countPendingPeople")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountPendingPeople query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/contacts/pending-people/count")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [EndpointName("countPendingPeople_get")]
    [AliasFor("countPendingPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountPendingPeople query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountPendingPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _pendingPersonService.CountAsync(query, cancellation);

        return new CountResult(count);
    }

    /// <summary>
    /// Downloads the list of pending people that match specific criteria
    /// </summary>    
    [HttpPost("api/contacts/pending-people/download")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPendingPeople")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPendingPeople query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/contacts/pending-people/download")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPendingPeople_get")]
    [AliasFor("downloadPendingPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectPendingPeople query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectPendingPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Directory", "PendingPeople", query.Filter.Format, User);

        var models = await _pendingPersonService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _pendingPersonService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific pending person
    /// </summary>
    [HttpGet("api/contacts/pending-people/{pending:guid}")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [ProducesResponseType(typeof(PendingPersonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrievePendingPerson")]
    public async Task<ActionResult<PendingPersonModel>> RetrieveAsync([FromRoute] Guid pending, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _pendingPersonService.RetrieveAsync(pending, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationIdentifier))
            return NotFound();

        return model;
    }

    /// <summary>
    /// Searches for the list of pending people that match specific criteria
    /// </summary>
    [HttpPost("api/contacts/pending-people/search")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [EndpointName("searchPendingPeople")]
    public async Task<ActionResult<PendingPersonModel[]>> PostSearchAsync([FromBody] CollectPendingPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/contacts/pending-people/search")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [EndpointName("searchPendingPeople_get")]
    [AliasFor("searchPendingPeople")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<PendingPersonModel[]>> GetSearchAsync([FromQuery] CollectPendingPeople query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<PendingPersonModel[]> SearchAsync(CollectPendingPeople query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _pendingPersonService.CollectAsync(query, cancellation);

        var count = await _pendingPersonService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return matches;
    }

    #endregion Queries

    #region Commands

    [HttpPost("api/contacts/pending-people")]
    [HybridPermission("directory/people", DataAccess.Create)]
    [ProducesResponseType<PendingPersonModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createPendingPerson")]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePendingPerson create, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var id = await _pendingPersonService.CreateAsync(create, principal, cancellation);

        if (id == null)
            return BadRequest($"Failed to create.");

        var model = await _pendingPersonService.RetrieveAsync(id.Value, cancellation);

        return Created($"api/contacts/pending-people/{id.Value}", model);
    }

    [HttpDelete("api/contacts/pending-people/{pending:guid}")]
    [HybridPermission("directory/people", DataAccess.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deletePendingPerson")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid pending, CancellationToken cancellation = default)
    {
        var deleted = await _pendingPersonService.DeleteAsync(pending, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("api/contacts/pending-people/{pending:guid}")]
    [HybridPermission("directory/people", DataAccess.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyPendingPerson")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid pending, [FromBody] ModifyPendingPerson modify, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _pendingPersonService.RetrieveAsync(pending, cancellation);

        if (model is null)
            return NotFound($"PendingPerson not found: PendingId {modify.PendingPersonIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _pendingPersonService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands

    #region Import

    public class SearchImports : Query<IEnumerable<PendingPersonImportModel>>
    {
    }

    [HttpPost("api/contacts/pending-people/search-import")]
    [HybridPermission("directory/pending-people", DataAccess.Read)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<PendingPersonImportModel[]>> SearchImportAsync(SearchImports criteria, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var collectPeople = new CollectPendingPeople { OrganizationId = principal.OrganizationId };
        collectPeople.Filter.Page = criteria.Filter.Page;
        collectPeople.Filter.Sort = nameof(PendingPersonEntity.PersonCode);

        var matches = await _pendingPersonService.CollectImportAsync(collectPeople, cancellation);
        var count = await _pendingPersonService.CountAsync(collectPeople, cancellation);

        Response.AddPagination(collectPeople.Filter, count);

        return matches;
    }

    public class ImportResultItem
    {
        public required string PersonCode { get; init; }
        public required ImportPersonResult.StatusEnum Status { get; init; }
        public Guid PendingPersonId { get; init; }
        public Guid? UserId { get; init; }
        public ValidationFailure? Failure { get; init; }
    }

    public class ImportResult
    {
        public required Guid? ReportFileId { get; init; }
        public required string? ReportFileName { get; init; }
        public required ImportResultItem[] ImportedPeople { get; init; }
    }

    [HttpPost("api/contacts/pending-people/import")]
    [HybridPermission("directory/pending-people", DataAccess.Update)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<ImportResult>> ImportAsync(
        IPersonImporter importer,
        IPersonImportReporter reporter,
        OrganizationService organizationService,
        OrganizationAdapter organizationAdapter,
        ImportPendingPerson[] imports
        )
    {
        var principal = _principalProvider.GetPrincipal();
        var submittedBy = principal.UserId;
        var submittedByName = principal.Name;

        var organizationId = principal.Organization.Identifier;
        var organization = await organizationService.RetrieveAsync(organizationId) ?? throw new ArgumentNullException($"Organization {organizationId} is not found");
        var organizationData = organizationAdapter.ToData(organization);
        var fullNamePolicy = organizationData.Toolkits?.Contacts?.FullNamePolicy;
        var timeZone = organizationData.TimeZone.Id;

        var result = await importer.ImportPendingPeopleAsync(organizationId, fullNamePolicy, timeZone, submittedByName, imports);
        var file = await reporter.SaveReportAsync(organizationId, submittedBy, timeZone, result, false);

        return new ImportResult
        {
            ReportFileId = file?.FileIdentifier,
            ReportFileName = file?.FileName,
            ImportedPeople = result.Select(x => new ImportResultItem
            {
                PersonCode = x.Input.PersonCode,
                Status = x.Status,
                PendingPersonId = x.PendingPersonId!.Value,
                UserId = x.UserId,
                Failure = x.Failure
            })
            .ToArray(),
        };
    }

    #endregion
}
