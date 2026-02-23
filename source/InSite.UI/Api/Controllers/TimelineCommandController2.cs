using System;
using System.ComponentModel;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Groups.Write;

using Shift.Common;
using Shift.Common.Kernel;
using Shift.Common.Timeline.Commands;
using Shift.Common.Timeline.Exceptions;
using Shift.Sdk.UI;

namespace InSite.Api
{
    // HACK: Here I am disabling the default authorization attribute and enabling the attribute
    // copied from the InSite.Api prototype. The JwtAuthorize class must be consolidated into the
    // method ApiValidationFilter.AuthenticateJwt().

    /*
    
    Here is some additional clarification:

    When an HTTP request is submitted to the v1 API controller (CommandController), the 
    ApiValidationFilter.AuthenticateAsync is invoked because Global.Application_Start has added this 
    filter to the HttpConfiguration.

    The ApiAuthenticationRequirement attribute on the CommandController is set to None here, and 
    this causes the ApiValidationFilter.AuthenticateAsync invocation to exit immediately.

    The JwtAuthorize attribute on the CommandController forces execution to proceed and invoke
    JwtAuthorizeAttribute.OnAuthorization. Here the JWT is validated. If the JWT is missing, 
    expired, or malformed, then the controller responds with 401 Unauthorized.

    In other words, this is a temporary HACK, but it is not a security vulnerability.

     */

    [DisplayName("Timeline")]
    [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
    public class TimelineController : ApiController
    {
        static readonly string[] _excludeNamespaces = new[]
        {
            "InSite.Application.Groups.Write.Old",
            "InSite.Application.People.Write.Old",
            "InSite.Application.Users.Write.Old"
        };

        static readonly CommandTypeCollection _types = new CommandTypeCollection(typeof(RenameGroup).Assembly, typeof(Command), _excludeNamespaces);

        static readonly CommandBuilder _commandBuilder = new CommandBuilder(_types, new JsonSerializer2());

        [HttpPost]
        [Route("api/timeline/commands")]
        public HttpResponseMessage ExecuteCommand(string command)
        {
            var root = Global.GetRootSentinel();
            var identity = HttpContext.Current.GetIdentity();
            if (identity.User.Identifier != root.Identifier)
                throw new ArgumentException("Only the root account is permitted to invoke this API method.");

            var commandType = _commandBuilder.GetCommandType(command);

            var requestBody = HttpHelper.ReadRequestBody(Request);
            var commandObject = (Command)_commandBuilder.BuildCommand(commandType, requestBody);
            var principal = HttpContext.Current.User as IPrincipal;

            try
            {
                ServiceLocator.ExecuteCommand(commandObject);

                return HttpHelper.Ok(Request);
            }
            catch (UnhandledCommandException ex)
            {
                if (ex.InnerException is AggregateNotFoundException aex)
                    return HttpHelper.NotFound(Request, aex);

                return HttpHelper.ServerError(Request, ex);
            }
            catch (Exception ex)
            {
                return HttpHelper.ServerError(Request, ex);
            }
        }
    }
}
