using Microsoft.AspNetCore.Mvc;

using Shift.Service.Progress;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Credentials")]
public class CredentialController : ShiftControllerBase
{
    private readonly CredentialService _credentialService;
    private readonly IPrincipalProvider _principalProvider;

    public CredentialController(CredentialService credentialService, IPrincipalProvider principalProvider)
    {
        _credentialService = credentialService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific credential
    /// </summary>
    [HttpHead("api/progress/credentials/{credential:guid}")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertCredential")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid credential, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _credentialService.AssertAsync(credential, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of credentials that match specific criteria
    /// </summary>
    [HttpPost("api/progress/credentials/collect")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CredentialModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCredentials")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectCredentials query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/progress/credentials")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CredentialModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectCredentials_get")]
    [AliasFor("collectCredentials")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectCredentials query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectCredentials query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _credentialService.CollectAsync(query, principal.TimeZone, cancellation);

        var count = await _credentialService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the credentials that match specific criteria
    /// </summary>
    [HttpPost("api/progress/credentials/count")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCredentials")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountCredentials query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/progress/credentials/count")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countCredentials_get")]
    [AliasFor("countCredentials")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountCredentials query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountCredentials query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _credentialService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of credentials that match specific criteria
    /// </summary>    
    [HttpPost("api/progress/credentials/download")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCredentials")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectCredentials query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/progress/credentials/download")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadCredentials_get")]
    [AliasFor("downloadCredentials")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectCredentials query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectCredentials query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Progress", "Credentials", query.Filter.Format, User);

        var models = await _credentialService
            .DownloadAsync(query, principal.TimeZone, cancellation)
            .ToListAsync(cancellation);

        var content = _credentialService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific credential
    /// </summary>
    [HttpGet("api/progress/credentials/{credential:guid}")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<CredentialModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveCredential")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid credential, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _credentialService.RetrieveAsync(credential, principal.TimeZone, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of credentials that match specific criteria
    /// </summary>
    [HttpPost("api/progress/credentials/search")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CredentialMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCredentials")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchCredentials query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/progress/credentials/search")]
    [HybridPermission("progress/credentials", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<CredentialMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchCredentials_get")]
    [AliasFor("searchCredentials")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchCredentials query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchCredentials query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _credentialService.SearchAsync(query, cancellation);

        var count = await _credentialService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}