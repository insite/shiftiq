using System;
using System.ComponentModel;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Shift.Common.Timeline.Commands;
using Shift.Common.Timeline.Exceptions;

using InSite.Application.Groups.Write;

using Shift.Common;
using Shift.Common.Kernel;
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
    [ApiAuthenticationRequirement(ApiAuthenticationType.None)]
    [JwtAuthorize]
    public class TimelineController : ApiController
    {
        readonly string[] _excludeNamespaces = new[]
        {
            "InSite.Application.Groups.Write.Old",
            "InSite.Application.People.Write.Old",
            "InSite.Application.Users.Write.Old"
        };

        [HttpPost]
        [Route("api/timeline/commands")]
        public HttpResponseMessage ExecuteCommand(string command)
        {
            var types = new CommandTypeCollection(typeof(RenameGroup).Assembly, typeof(Command), _excludeNamespaces);

            var serializer = new JsonSerializer2();

            var commandBuilder = new CommandBuilder(types, serializer);

            var commandType = commandBuilder.GetCommandType(command);

            // FIXME: Implement access control. We cannot permit every user to execute every command.
            // ConfirmPermission(commandType);

            var status = $"{HttpContext.Current.User.Identity.Name} is executing command {commandType.FullName}";

            var requestBody = HttpHelper.ReadRequestBody(Request);

            var commandObject = commandBuilder.BuildCommand(commandType, requestBody);

            var co = (Command)commandObject;

            var principal = HttpContext.Current.User as IShiftPrincipal;

            co.OriginOrganization = principal.Organization.Identifier;

            co.OriginUser = principal.User.Identifier;

            try
            {
                ServiceLocator.ExecuteCommand(co);

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
