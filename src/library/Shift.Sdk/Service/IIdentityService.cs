using System;
using System.Security.Principal;

namespace InSite.Application
{
    public interface IIdentityService
    {
        string GetCurrentHost();
        void SetCurrentHost(string host);

        string GetCurrentUrl();
        void SetCurrentUrl(string url);

        Guid GetCurrentOrganization();
        void SetCurrentOrganization(Guid organization);
        
        Guid GetCurrentUser();
        void SetCurrentUser(Guid user);

        ServiceIdentity GetCurrentService();
        void SetCurrentService(ServiceIdentity user);
    }

    public class ServiceIdentity : IPrincipal, IIdentity
    {
        public Guid Organization { get; set; }
        public Guid User { get; set; }

        public IIdentity Identity
            => this;

        public string Name { get; set; }

        public string AuthenticationType
            => "API";

        public bool IsAuthenticated
            => true;

        public bool IsInRole(string role)
            => throw new NotImplementedException();
    }
}