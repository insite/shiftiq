using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Forms")]
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
    [HttpHead("api/workflow/forms/{form:guid}")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertForm")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid surveyForm, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _formService.AssertAsync(surveyForm, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of forms that match specific criteria
    /// </summary>
    [HttpPost("api/workflow/forms/collect")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FormModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectForms")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workflow/forms")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FormModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectForms_get")]
    [AliasFor("collectForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectForms query, CancellationToken cancellation)
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
    [HttpPost("api/workflow/forms/count")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countForms")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workflow/forms/count")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countForms_get")]
    [AliasFor("countForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountForms query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _formService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of forms that match specific criteria
    /// </summary>    
    [HttpPost("api/workflow/forms/download")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadForms")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectForms query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workflow/forms/download")]
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
    [HttpGet("api/workflow/forms/{form:guid}")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<FormModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveForm")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid surveyForm, CancellationToken cancellation = default)
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
    [HttpPost("api/workflow/forms/search")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FormMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchForms")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workflow/forms/search")]
    [HybridPermission("workflow/forms", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<FormMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchForms_get")]
    [AliasFor("searchForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchForms query, CancellationToken cancellation)
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