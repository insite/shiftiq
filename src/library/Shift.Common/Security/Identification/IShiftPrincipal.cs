using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Common
{
    public interface IShiftPrincipal : System.Security.Principal.IIdentity, System.Security.Principal.IPrincipal, ISimplePrincipal
    {
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

namespace Shift.Common
{
    public interface ISimplePrincipal
    {
        Guid? OrganizationId { get; }

        Guid? UserId { get; }

        Guid[] RoleIds { get; }

        bool IsOperator { get; }
    }
}