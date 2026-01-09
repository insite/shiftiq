using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Timeline API")]
public class QueryController : ControllerBase
{
    private readonly PermissionMatrix _authorizationService;
    private readonly Shift.Common.IClaimConverter _converter;
    private readonly QueryDispatcher _dispatcher;
    private readonly Shift.Common.Reflector _reflector;
    private readonly IJsonSerializerBase _serializer;
    private readonly QueryBuilder _builder;

    public QueryController(QueryTypeCollection queryTypes,
         PermissionMatrix authorizationService, Shift.Common.IClaimConverter converter, QueryDispatcher dispatcher, IJsonSerializerBase serializer)
    {
        _authorizationService = authorizationService;
        _converter = converter;
        _dispatcher = dispatcher;
        _reflector = new Shift.Common.Reflector();
        _serializer = serializer;
        _builder = new QueryBuilder(queryTypes, serializer);
    }

    [HttpPost("timeline/queries")]
    [HybridAuthorize(Policies.Timeline.Queries)]
    public async Task<IActionResult> RunQueryAsync([FromQuery] string q, [FromQuery] QueryFilter filter)
    {
        try
        {
            var queryType = _builder.GetQueryType(q);

            ConfirmPermission(queryType);

            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

            var resultType = _builder.GetResultType(queryType);

            var queryObject = _builder.BuildQuery(queryType, resultType, requestBody, filter);

            // TODO: Implement monitoring.

            var resultObject = RunQuery(queryObject, resultType);

            if (resultObject == null)
                return NotFound();

            return Ok(resultObject);
        }
        catch (Common.AccessDeniedException ad)
        {
            return Problem(ad.Message, null, 403);
        }
        catch (Common.BadQueryException bq)
        {
            return Problem(bq.Message, null, 400);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, 500);
        }
    }

    private void ConfirmPermission(Type queryType)
    {
        // Confirm the user's authorization token has permission to run this specific query.

        var principal = _converter.ToPrincipal(User.Claims);

        var roles = principal.Roles.Select(x => x.Name);

        var resourceName = _reflector.GetResourceName(queryType);

        if (IsGranted(principal, resourceName, roles, Common.SwitchAccess.On))
            return;

        var roleList = string.Join(", ", roles);

        var message = "None of the roles assigned to this token are granted access to run this"
            + $" query. Resource = {resourceName}. Roles = {roleList}.";

        throw new Common.AccessDeniedException(message);
    }

    private bool IsGranted(IShiftPrincipal principal, string resourceSlug, IEnumerable<string> roles, Common.SwitchAccess access)
    {
        var permissionList = _authorizationService.GetPermissions(principal.Organization.Slug);

        var relativeUrl = new RelativePath(resourceSlug);

        var granted =
            permissionList.IsAllowed("*", roles, access) ||
            permissionList.IsAllowed(relativeUrl.Value, roles, access);

        while (!granted && relativeUrl.HasSegments())
        {
            relativeUrl.RemoveLastSegment();

            granted = permissionList.IsAllowed(relativeUrl.Value, roles, access);
        }

        return granted;
    }

    private object? RunQuery(object queryObject, Type resultType)
    {
        var dispatchMethod = typeof(QueryDispatcher).GetMethod(nameof(QueryDispatcher.Dispatch))!;

        var genericDispatchMethod = dispatchMethod.MakeGenericMethod(resultType);

        var resultObject = genericDispatchMethod.Invoke(_dispatcher, [queryObject]);

        return resultObject;
    }
}