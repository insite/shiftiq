using System.Security.Principal;

using InSite.Domain.Organizations;

using UserPacket = InSite.Domain.Foundations.User;

namespace InSite.Api.Settings
{
    public class ApiUserIdentity : IIdentity, IPrincipal
    {
        public OrganizationState Organization { get; }

        public UserPacket User { get; }

        public bool IsAdministrator { get; }

        public ApiUserIdentity(OrganizationState organization, UserPacket user, bool isAdministrator)
        {
            Organization = organization;
            User = user;
            IsAdministrator = isAdministrator;
        }

        #region IIdentity

        string IIdentity.Name => User?.Email;

        string IIdentity.AuthenticationType => "API";

        bool IIdentity.IsAuthenticated => Organization != null && User != null;

        #endregion

        #region IPrincipal

        IIdentity IPrincipal.Identity => this;

        bool IPrincipal.IsInRole(string role) => false;

        #endregion
    }
}