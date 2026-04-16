using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Groups.Write;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Kernel;
using Shift.Common.Timeline.Changes;
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
    public class TimelineController : ApiBaseController
    {
        static readonly string[] _excludeNamespaces = new[]
        {
            "InSite.Application.Groups.Write.Old",
            "InSite.Application.People.Write.Old",
            "InSite.Application.Users.Write.Old"
        };

        static readonly CommandTypeCollection _types = new CommandTypeCollection(typeof(RenameGroup).Assembly, typeof(Command), _excludeNamespaces);

        static readonly JsonSerializer2 _serializer = new JsonSerializer2();

        static readonly CommandBuilder _commandBuilder = new CommandBuilder(_types, _serializer);

        [HttpPost]
        [Route("api/timeline/commands")]
        public HttpResponseMessage ExecuteCommands()
        {
            EnsureRootAccount();

            var requestBody = HttpHelper.ReadRequestBody(Request);
            var apiCommands = _serializer.Deserialize<List<ApiCommand>>(requestBody);
            var commands = new List<ICommand>();

            foreach (var apiCommand in apiCommands)
            {
                var commandType = _commandBuilder.GetCommandType(apiCommand.CommandName);
                var commandObject = (Command)_commandBuilder.BuildCommand(commandType, apiCommand.CommandData);

                commands.Add(commandObject);
            }

            try
            {
                foreach (var command in commands)
                    ServiceLocator.ExecuteCommand(command);

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

        [HttpGet]
        [Route("api/timeline/aggregates/{aggregateId}")]
        public HttpResponseMessage GetAggregateState(Guid aggregateId, string aggregateType)
        {
            EnsureRootAccount();

            AggregateState state;

            switch (aggregateType)
            {
                case "BankAggregate":
                    state = ServiceLocator.AggregateSearch.GetState<BankAggregate>(aggregateId);
                    break;
                default:
                    throw new ArgumentException($"aggregateType: {aggregateType}");
            }

            return state != null ? JsonSuccess(state) : JsonError(new { }, HttpStatusCode.NotFound);
        }

        private void EnsureRootAccount()
        {
            var root = Global.GetRootSentinel();
            var identity = HttpContext.Current.GetIdentity();
            if (identity.User.Identifier != root.Identifier)
                throw new ArgumentException("Only the root account is permitted to invoke this API method.");
        }
    }
}
