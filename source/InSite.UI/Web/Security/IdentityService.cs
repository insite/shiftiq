using System;
using System.Web;

using InSite.Application;

namespace InSite.Web.Security
{
    public class IdentityService : IIdentityService
    {
        public string GetCurrentHost()
        {
            var url = HttpContext.Current?.Request?.Url;

            return url != null
                ? $"{url.Scheme}://{url.Host}"
                : null;
        }

        public Guid GetCurrentOrganization()
        {
            Guid id = CurrentSessionState.Identity?.Organization?.Identifier ?? Guid.Empty;

            if (id == Guid.Empty && HttpContext.Current?.User is ServiceIdentity service)
                id = service.Organization;

            if (id == Guid.Empty && HttpContext.Current?.User is Api.Settings.ApiUserIdentity api)
                id = api.Organization?.Identifier ?? Guid.Empty;

            return id;
        }

        public ServiceIdentity GetCurrentService()
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.User as ServiceIdentity;
            
            return null;
        }

        public string GetCurrentUrl() => HttpContext.Current?.Request?.RawUrl;

        public Guid GetCurrentUser()
        {
            Guid id = CurrentSessionState.Identity?.User?.UserIdentifier ?? Guid.Empty;

            if (id == Guid.Empty && HttpContext.Current?.User is ServiceIdentity service)
                id = service.User;

            if (id == Guid.Empty && HttpContext.Current?.User is Api.Settings.ApiUserIdentity api)
                id = api.User?.Identifier ?? Guid.Empty;

            return id;
        }

        public void SetCurrentHost(string host) => throw new NotImplementedException();

        public void SetCurrentOrganization(Guid organization) => throw new NotImplementedException();

        public void SetCurrentService(ServiceIdentity identity)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.User = identity;
        }

        public void SetCurrentUrl(string url) => throw new NotImplementedException();

        public void SetCurrentUser(Guid user) => throw new NotImplementedException();
    }
}