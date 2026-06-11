using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflows API: Forms")]
public class FormController : ShiftControllerBase
{
    private readonly FormService _formService;
    private readonly IPrincipalProvider _principalProvider;

    public FormController(FormService formService, IPrincipalProvider principalProvider)
    {
        _formService = formService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific form
    /// </summary>
    [HttpHead("api/workflows/forms/{form:guid}")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertForm")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid surveyForm, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _formService.AssertAsync(surveyForm, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of forms that match specific criteria
    /// </summary>
    [HttpPost("api/workflows/forms/collect")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [EndpointName("collectForms")]
    public async Task<ActionResult<IEnumerable<FormModel>>> PostCollectAsync([FromBody] CollectForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workflows/forms")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [EndpointName("collectForms_get")]
    [AliasFor("collectForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<FormModel>>> GetCollectAsync([FromQuery] CollectForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<FormModel>>> CollectAsync(CollectForms query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _formService.CollectAsync(query, cancellation);

        var count = await _formService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the forms that match specific criteria
    /// </summary>
    [HttpPost("api/workflows/forms/count")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [EndpointName("countForms")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workflows/forms/count")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [EndpointName("countForms_get")]
    [AliasFor("countForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountForms query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _formService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of forms that match specific criteria
    /// </summary>    
    [HttpPost("api/workflows/forms/download")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadForms")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectForms query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workflows/forms/download")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadForms_get")]
    [AliasFor("downloadForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectForms query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectForms query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Workflow", "Forms", query.Filter.Format, User);

        var models = await _formService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _formService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific form
    /// </summary>
    [HttpGet("api/workflows/forms/{form:guid}")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType(typeof(FormModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveForm")]
    public async Task<ActionResult<FormModel>> RetrieveAsync([FromRoute] Guid surveyForm, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _formService.RetrieveAsync(surveyForm, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of forms that match specific criteria
    /// </summary>
    [HttpPost("api/workflows/forms/search")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [EndpointName("searchForms")]
    public async Task<ActionResult<IEnumerable<FormMatch>>> PostSearchAsync([FromBody] SearchForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workflows/forms/search")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [EndpointName("searchForms_get")]
    [AliasFor("searchForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<FormMatch>>> GetSearchAsync([FromQuery] SearchForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<FormMatch>>> SearchAsync(SearchForms query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _formService.SearchAsync(query, cancellation);

        var count = await _formService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}