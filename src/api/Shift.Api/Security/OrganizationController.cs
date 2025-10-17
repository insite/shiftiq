using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Organizations")]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationService _organizationService;

    public OrganizationController(OrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpHead("security/organizations/{organization:guid}")]
    [HybridAuthorize(Policies.Security.Organizations.Organization.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid organization, CancellationToken cancellation = default)
    {
        var exists = await _organizationService.AssertAsync(organization, cancellation);

        return Ok(exists);
    }

    [HttpGet("security/organizations/{organization:guid}")]
    [HybridAuthorize(Policies.Security.Organizations.Organization.Retrieve)]
    [ProducesResponseType(typeof(OrganizationModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationModel>> RetrieveAsync([FromRoute] Guid organization, CancellationToken cancellation = default)
    {
        var model = await _organizationService.RetrieveAsync(organization, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("security/organizations/count")]
    [HybridAuthorize(Policies.Security.Organizations.Organization.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountOrganizations query, CancellationToken cancellation = default)
    {
        var count = await _organizationService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("security/organizations")]
    [HybridAuthorize(Policies.Security.Organizations.Organization.Collect)]
    [ProducesResponseType(typeof(IEnumerable<OrganizationModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrganizationModel>>> CollectAsync([FromQuery] CollectOrganizations query, CancellationToken cancellation = default)
    {
        var models = await _organizationService.CollectAsync(query, cancellation);

        var count = await _organizationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("security/organizations/search")]
    [HybridAuthorize(Policies.Security.Organizations.Organization.Search)]
    [ProducesResponseType(typeof(IEnumerable<OrganizationMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrganizationMatch>>> GetSearchAsync([FromQuery] SearchOrganizations query, CancellationToken cancellation = default)
    {
        var matches = await _organizationService.SearchAsync(query, cancellation);

        var count = await _organizationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("security/organizations/search")]
    [HybridAuthorize(Policies.Security.Organizations.Organization.Search)]
    [ProducesResponseType(typeof(IEnumerable<OrganizationMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrganizationMatch>>> SearchAsync([FromBody] SearchOrganizations query, CancellationToken cancellation = default)
    {
        var matches = await _organizationService.SearchAsync(query, cancellation);

        var count = await _organizationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    // This entity is a current-state projection of an aggregate event/change stream. This is the reason you do not see
    // any controller actions implemented here to create, modify, or delete this entity. Data changes to this entity are 
    // permitted only using Timeline commands.
}