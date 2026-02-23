using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Shift.Service.Variant.CMDS;

namespace Shift.Api.Variant.CMDS;

[ApiController]
[ApiExplorerSettings(GroupName = "Variant API: CMDS")]
public class ComplianceSummaryController : ShiftControllerBase
{
    private readonly IMonitor _monitor;
    private readonly ComplianceSummaryReader _search;
    private readonly IPrincipalProvider _principalProvider;

    public ComplianceSummaryController(IMonitor monitor, ComplianceSummaryReader search, IPrincipalProvider principalProvider)
    {
        _monitor = monitor;
        _search = search;
        _principalProvider = principalProvider;
    }

    [HttpPost("api/plugin/cmds/compliance-summaries")]
    [HttpPost("api/variant/cmds/compliance-summaries")]
    [BearerPermission("variant/cmds")]
    [ProducesResponseType(typeof(ComplianceSummaryModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportAsync([FromBody] ComplianceSummaryCriteria criteria, CancellationToken token)
    {
        var doingWhat = "Generating CMDS compliance summaries";

        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        try
        {
            var entities = await _search.ExportAsync(criteria, organizationId!.Value, token);

            var adapter = new ComplianceSummaryAdapter();

            var model = adapter.ToModel(criteria, entities);

            return Ok(model);
        }
        catch (Exception ex)
        {
            var uri = _monitor.Error(ex.Message);

            var problem = ExceptionHandlingMiddleware
                .ReportUnexpectedProblem(ex, doingWhat, HttpContext, _monitor);

            problem.Extensions.Add("Criteria", JsonConvert.SerializeObject(criteria));

            return problem.ToActionResult(this);
        }
    }
}