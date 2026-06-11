using System;
using System.Collections.Generic;

namespace Shift.Common
{
    public interface ISimplePrincipal
    {
        Guid OrganizationId { get; }

        Guid UserId { get; }

        Guid[] RoleIds { get; }

        bool IsAdministrator { get; }

        bool IsDeveloper { get; }

        bool IsOperator { get; }

        TimeZoneInfo TimeZone { get; }
    }

    public interface IPrincipal : System.Security.Principal.IIdentity, System.Security.Principal.IPrincipal, ISimplePrincipal
    {
        Guid CookieId { get; set; }

        IJwt Claims { get; set; }

        Actor User { get; set; }

        Model Organization { get; set; }

        Model Person { get; set; }

        AuthorityAccess Authority { get; set; }

        List<Role> Roles { get; set; }

        Model Partition { get; set; }

        string IPAddress { get; set; }

        Proxy Proxy { get; set; }
    }
}
