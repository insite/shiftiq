using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public class Principal : IShiftPrincipal, ISimplePrincipal
    {
        public Actor User { get; set; }
        public Model Person { get; set; }
        public Proxy Proxy { get; set; }

        public string IPAddress { get; set; }

        public Model Organization { get; set; }
        public Model Partition { get; set; }
        public List<Role> Roles { get; set; }
        public AuthorityAccess Authority { get; set; }

        public IJwt Claims { get; set; }

        public Principal()
        {
            Claims = new Jwt();
            Roles = new List<Role>();
        }

        #region IIdentity and IPrincipal

        public string AuthenticationType { get; set; }

        System.Security.Principal.IIdentity System.Security.Principal.IPrincipal.Identity => this;

        public bool IsAuthenticated { get; set; }

        public string Name => User.Email;

        public bool IsInRole(string role)
        {
            if (Roles == null || Roles.Count == 0)
                return false;

            var names = Roles.Select(x => x.Name).ToArray();

            if (role.MatchesAny(names))
                return true;

            var identifiers = Roles.Select(x => x.Identifier.ToString()).ToArray();

            if (role.MatchesAny(identifiers))
                return true;

            return false;
        }

        #endregion

        #region ISimplePrincipal

        public Guid? OrganizationId => Organization?.Identifier;

        public Guid? UserId => User?.Identifier;

        public bool IsAdministrator => Authority.HasFlag(AuthorityAccess.Administrator);

        public bool IsDeveloper => Authority.HasFlag(AuthorityAccess.Developer);

        public bool IsOperator => Authority.HasFlag(AuthorityAccess.Operator);

        public Guid[] RoleIds
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                    return new Guid[0];

                return Roles
                    .Select(x => x.Identifier)
                    .ToArray();
            }
        }

        #endregion
    }
}