using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

using ApplicationError = Shift.Common.ApplicationError;

namespace Shift.Contract
{
    public class AuthorizerHolder
    {
        public Authorizer Instance { get; set; }
    }

    public class Authorizer
    {
        private static readonly Guid PortalActionIdentifier = Guid.Parse("20056572-CFE8-4049-9774-7876F33E0404");

        private readonly IActionService _actionService;

        private readonly List<Permission> _permissions = new List<Permission>();

        private readonly Dictionary<Guid, Resource> _resources = new Dictionary<Guid, Resource>();

        private readonly Dictionary<Guid, Role> _roles = new Dictionary<Guid, Role>();

        public string Domain { get; private set; }

        public Guid NamespaceId { get; private set; }

        public List<Requirement> Requirements { get; set; } = new List<Requirement>();

        public Authorizer(string domain, IActionService actionService)
        {
            Domain = domain;

            NamespaceId = UuidFactory.CreateV5ForDns(domain);

            _actionService = actionService;
        }

        public List<Permission> Add(PermissionBundle bundle)
        {
            var list = new List<Permission>();

            foreach (var policy in bundle.Policies)
            {
                foreach (var role in bundle.Roles)
                {
                    var item = Add(policy, role);

                    item.Access.Basic = BasicAccess.Allow;

                    list.Add(item);
                }
            }

            return list;
        }

        public Permission Add(string resourceSlug, string roleSlug)
        {
            var resource = Guid.TryParse(resourceSlug, out Guid resourceId)
                ? new Resource
                {
                    Identifier = resourceId,
                    Slug = resourceSlug,
                }
                : new Resource
                {
                    Identifier = UuidFactory.CreateV5(NamespaceId, resourceSlug),
                    Slug = resourceSlug,
                };

            var role = Guid.TryParse(roleSlug, out Guid roleId)
                ? new Role
                {
                    Identifier = roleId,
                    Slug = roleSlug,
                }
                : new Role
                {
                    Identifier = ResolveRoleId(roleSlug),
                    Slug = roleSlug,
                };

            return Add(resource, role);
        }

        private Guid ResolveRoleId(string roleSlug)
        {
            var role = _roles.Values.FirstOrDefault(x => string.Compare(roleSlug, x.Slug, true) == 0);

            if (role != null)
                return role.Identifier;

            return UuidFactory.CreateV5(NamespaceId, roleSlug);
        }

        public Permission Add(Resource resource, Role role)
        {
            var permission = GetOptional(resource.Identifier, role.Identifier);

            if (permission == null)
            {
                permission = new Permission
                {
                    Access = new Access(),
                    Resource = resource,
                    Role = role
                };

                _permissions.Add(permission);
            }

            if (!_resources.ContainsKey(resource.Identifier))
                _resources.Add(resource.Identifier, resource);

            if (!_roles.ContainsKey(role.Identifier))
                _roles.Add(role.Identifier, role);

            return permission;
        }

        public void AddResources(Dictionary<string, string> resources)
        {
            var list = new List<RelativePath>();
            foreach (var key in resources.Keys)
                list.Add(new RelativePath(key, resources[key]));

            var collection = new RelativePathCollection(list);

            foreach (var resourceSlug in resources.Keys)
            {
                var resource = new Resource
                {
                    Identifier = UuidFactory.CreateV5(NamespaceId, resourceSlug),
                    Slug = resourceSlug
                };

                if (!_resources.ContainsKey(resource.Identifier))
                    _resources.Add(resource.Identifier, resource);
            }
        }

        public void AddRoles(List<string> roles)
        {
            foreach (var roleSlug in roles)
            {
                var role = new Role
                {
                    Identifier = UuidFactory.CreateV5(NamespaceId, roleSlug),
                    Slug = roleSlug
                };

                if (!_roles.ContainsKey(role.Identifier))
                    _roles.Add(role.Identifier, role);
            }
        }

        public bool Contains(Guid resource, Guid role)
            => GetOptional(resource, role) != null;

        public bool Contains(string resource, string role)
        {
            var resourceId = UuidFactory.CreateV5(NamespaceId, resource);

            var roleId = UuidFactory.CreateV5(NamespaceId, role);

            return Contains(resourceId, roleId);
        }

        private Permission GetOptional(Guid resource, Guid role)
        {
            return _permissions.SingleOrDefault(
              p => p.Resource.Identifier == resource
                && p.Role.Identifier == role);
        }

        private Permission GetOptional(string resource, string role)
        {
            var resourceId = UuidFactory.CreateV5(NamespaceId, resource);

            var roleId = UuidFactory.CreateV5(NamespaceId, role);

            return GetOptional(resourceId, roleId);
        }

        private Permission GetOptional(string resource, Model role)
        {
            var resourceId = UuidFactory.CreateV5(NamespaceId, resource);

            return GetOptional(resourceId, role.Identifier);
        }

        private Permission GetRequired(Guid resource, Guid role)
        {
            var permission = GetOptional(resource, role);

            if (permission == null)
                throw new ArgumentException($"Resource {resource} and Role {role} must be added before granting access.");

            return permission;
        }

        public void Grant(Guid resource, Guid role, BasicAccess access)
        {
            var permission = GetRequired(resource, role);

            permission.Access.Basic = access;
        }

        public void Grant(Guid resource, Guid role, DataAccess access)
        {
            var permission = GetRequired(resource, role);

            permission.Access.Data = access;
        }

        public void Grant(Guid resource, Guid role, HttpAccess access)
        {
            var permission = GetRequired(resource, role);

            permission.Access.Http = access;
        }

        public void Grant(Guid resource, Guid role, AuthorityAccess access)
        {
            var permission = GetRequired(resource, role);

            permission.Access.Authority = access;
        }

        public bool IsGranted(Guid resource, IEnumerable<Guid> roles)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                return (int)permission.Access.Basic > 0
                    || (int)permission.Access.Data > 0
                    || (int)permission.Access.Http > 0
                    || (int)permission.Access.Authority > 0;
            }

            return false;
        }

        public bool IsGranted(Guid resource, IEnumerable<Role> roles)
        {
            return IsGranted(resource, roles.Select(x => x.Identifier));
        }

        public bool IsGranted(Guid resource, IShiftPrincipal principal)
        {
            return principal.Authority >= AuthorityAccess.Operator
                || IsGranted(resource, principal.Roles);
        }

        public bool IsGranted(Guid resource, IEnumerable<Guid> roles, BasicAccess access)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                var helper = new BasicAccessHelper(permission.Access.Basic);

                if (helper.IsGranted(access))
                    return true;
            }

            return false;
        }

        public bool IsGranted(string resource, IEnumerable<Role> roles)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                return (int)permission.Access.Basic > 0
                    || (int)permission.Access.Data > 0
                    || (int)permission.Access.Http > 0
                    || (int)permission.Access.Authority > 0;
            }

            return false;
        }

        public bool IsGranted(string resource, IEnumerable<Role> roles, BasicAccess access)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                var helper = new BasicAccessHelper(permission.Access.Basic);

                if (helper.IsGranted(access))
                    return true;
            }

            return false;
        }

        public bool IsGranted(string resource, IShiftPrincipal principal)
        {
            return principal.Authority >= AuthorityAccess.Operator
                || IsGranted(resource, principal.Roles);
        }

        public bool IsGranted(string resource, IEnumerable<Model> roles, BasicAccess access)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                var helper = new BasicAccessHelper(permission.Access.Basic);

                if (helper.IsGranted(access))
                    return true;
            }

            return false;
        }

        public bool IsGranted(Guid resource, IEnumerable<Guid> roles, DataAccess access)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                var helper = new DataAccessHelper(permission.Access.Data);

                if (helper.IsGranted(access))
                    return true;
            }

            return false;
        }

        public bool IsGranted(Guid resource, IEnumerable<Guid> roles, HttpAccess access)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                var helper = new HttpAccessHelper(permission.Access.Http);

                if (helper.IsGranted(access))
                    return true;
            }

            return false;
        }

        public bool IsGranted(string resource, IEnumerable<Model> roles, HttpAccess access)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                var helper = new HttpAccessHelper(permission.Access.Http);

                if (helper.IsGranted(access))
                    return true;
            }

            return false;
        }

        public bool IsGranted(string resource, IEnumerable<string> roles, HttpAccess access)
        {
            foreach (var role in roles)
            {
                var permission = GetOptional(resource, role);

                if (permission == null)
                    continue;

                var helper = new HttpAccessHelper(permission.Access.Http);

                if (helper.IsGranted(access))
                    return true;
            }

            return false;
        }

        public bool IsActionAuthorized(string actionName, IShiftPrincipal principal)
        {
            if (principal.Authority >= AuthorityAccess.Operator)
                return true;

            if (actionName.Contains("?"))
                actionName = actionName.Substring(0, actionName.IndexOf("?"));

            if (actionName.StartsWith("/"))
                actionName = actionName.Substring(1);

            if (IsGranted(actionName, principal))
                return true;

            var action = _actionService.Retrieve(actionName);
            if (action?.PermissionParentActionIdentifier == null)
                return false;

            var parent = _actionService.Retrieve(action.PermissionParentActionIdentifier.Value)
                ?? throw new ApplicationError($"Action is not found: {action.PermissionParentActionIdentifier}");

            return parent.ActionIdentifier == PortalActionIdentifier || IsGranted(parent.ActionIdentifier, principal);
        }

        public List<Permission> GetPermissions()
            => _permissions;

        public List<Resource> GetResources()
            => _resources.Values.OrderBy(x => x.Slug).ToList();

        public List<Role> GetRoles()
            => _roles.Values.OrderBy(x => x.Slug).ToList();
    }
}