using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Timeline API")]
public class QueryController : ControllerBase
{
    private readonly AuthorizerFactory _authorizer;
    private readonly Shift.Common.IClaimConverter _converter;
    private readonly QueryDispatcher _dispatcher;
    private readonly Shift.Common.Reflector _reflector;
    private readonly IJsonSerializerBase _serializer;
    private readonly QueryBuilder _builder;

    public QueryController(QueryTypeCollection queryTypes,
         AuthorizerFactory authorizer, Shift.Common.IClaimConverter converter, QueryDispatcher dispatcher, IJsonSerializerBase serializer)
    {
        _authorizer = authorizer;
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

            await ConfirmPermissionAsync(queryType);

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

    private async Task ConfirmPermissionAsync(Type queryType)
    {
        // Confirm the user's authorization token has permission to run this specific query.

        var principal = _converter.ToPrincipal(User.Claims);

        var resourceSlug = _reflector.GetResourceSlug(queryType);

        if (await IsGrantedAsync(resourceSlug, principal.Roles, Common.BasicAccess.Allow))
            return;

        var roles = string.Join(", ", principal.Roles.Select(x => x.Identifier.ToString()));

        var message = "None of the roles assigned to this token are granted access to run this"
            + $" query. Resource = {resourceSlug}. Roles = {roles}.";

        throw new Common.AccessDeniedException(message);
    }

    private async Task<bool> IsGrantedAsync(string resourceSlug, IEnumerable<Model> roles, Common.BasicAccess access)
    {
        var authorizer = await _authorizer.CreateAsync();

        var relativeUrl = new RelativePath(resourceSlug);

        var granted =
            authorizer.IsGranted("*", roles, access) ||
            authorizer.IsGranted(relativeUrl.Value, roles, access);

        while (!granted && relativeUrl.HasSegments())
        {
            relativeUrl.RemoveLastSegment();

            granted = authorizer.IsGranted(relativeUrl.Value, roles, access);
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