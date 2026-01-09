using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Api.Settings
{
    public abstract class ApiBaseController : ApiController
    {
        protected OrganizationState GetOrganization() => ApiHelper.GetOrganization();

        protected static OrganizationState CurrentOrganization
        {
            get
            {
                var identity = HttpContext.Current?.User as ApiUserIdentity;
                return identity?.Organization;
            }
        }

        protected static UserModel CurrentUser
        {
            get
            {
                var identity = HttpContext.Current?.User as ApiUserIdentity;
                return identity?.User;
            }
        }

        protected static bool IsAdministrator
        {
            get
            {
                var identity = HttpContext.Current?.User as ApiUserIdentity;
                return identity?.IsAdministrator ?? false;
            }
        }

        protected HttpResponseMessage JsonBadRequest(object data)
            => JsonError(data, HttpStatusCode.BadRequest);

        protected HttpResponseMessage JsonError(object data, HttpStatusCode status = HttpStatusCode.InternalServerError)
            => DefaultJsonError(Request, data, status);

        protected HttpResponseMessage JsonSuccess(object data, HttpStatusCode status = HttpStatusCode.OK)
            => DefaultJsonSuccess(Request, data, status);

        protected HttpResponseMessage JsonUnauthorized(object data, HttpStatusCode status = HttpStatusCode.Unauthorized)
            => DefaultJsonError(Request, data, status);

        public static HttpResponseMessage DefaultJsonError(HttpRequestMessage request, object data, HttpStatusCode status = HttpStatusCode.InternalServerError)
        {
            var json = ServiceLocator.Serializer.Serialize(data);
            var response = request.CreateResponse(status);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        public static HttpResponseMessage DefaultJsonSuccess(HttpRequestMessage request, object data, HttpStatusCode status = HttpStatusCode.OK)
        {
            var json = ServiceLocator.Serializer.Serialize(data);
            var response = request.CreateResponse(status);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        public static HttpResponseMessage DefaultRedirect(HttpRequestMessage request, string redirectUrl, HttpStatusCode status = HttpStatusCode.Redirect)
        {
            var response = request.CreateResponse(status);
            response.Headers.Location = new Uri(redirectUrl);
            return response;
        }

        public static HttpResponseMessage DefaultStringSuccess(HttpRequestMessage request, string data, HttpStatusCode status = HttpStatusCode.OK)
        {
            var response = request.CreateResponse(status);
            response.Content = new StringContent(data, Encoding.UTF8, "application/json");
            return response;
        }

        protected HttpResponseMessage Redirect(string redirectUrl, HttpStatusCode status = HttpStatusCode.Redirect)
            => DefaultRedirect(Request, redirectUrl, status);

        protected static TCommand SetupCommandIdentity<TCommand>(TCommand command) where TCommand : ICommand
        {
            var organization = CurrentOrganization;
            if (organization != null)
                command.OriginOrganization = organization.Identifier;

            var user = CurrentUser;
            if (user != null)
                command.OriginUser = user.Identifier;

            return command;
        }

        protected static void SendCommand(ICommand command)
        {
            ServiceLocator.SendCommand(SetupCommandIdentity(command));
        }

        protected HttpResponseMessage StringSuccess(string data, HttpStatusCode status = HttpStatusCode.OK)
            => DefaultStringSuccess(Request, data, status);
    }

    public abstract class ApiOpenController : ApiBaseController { }
}