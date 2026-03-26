using System;
using System.Linq;
using System.Security.Principal;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

using PersonModel = InSite.Domain.Foundations.Person;
using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Domain.Foundations
{
    public interface ISecurityFramework : IIdentity, System.Security.Principal.IPrincipal, ISimplePrincipal
    {
        ClaimList Claims { get; set; }
        GroupList Groups { get; set; }

        bool HasAccessToAllCompanies { get; }

        Impersonator Impersonator { get; set; }

        bool IsImpersonating { get; }

        string Language { get; }

        OrganizationState Organization { get; set; }
        OrganizationList Organizations { get; set; }
        PersonModel Person { get; set; }
        PersonList Persons { get; set; }
        UserModel User { get; set; }

        string ChangeLanguage(string language);

        bool IsGranted(Guid? action, DataAccess? operation = null);
        bool IsGranted(string action, DataAccess? operation = null);

        string[] GetRoleNames();
    }

    /// <summary>
    /// This class represents the user/organization identity for a session. It is intended as a Data Transfer Object class only, therefore it has no
    /// methods except the methods required to implement IPrincipal.
    /// </summary>
    [Serializable]
    public class CurrentIdentity : IIdentity, System.Security.Principal.IPrincipal, ISecurityFramework
    {
        public static string Partition { get; set; }

        public static readonly Guid PortalActionIdentifier = Guid.Parse("20056572-CFE8-4049-9774-7876F33E0404");

        public string AuthenticationType
            => IsAuthenticated ? "InSite" : "None";

        /// <summary>
        /// The claims authorized for the current user, within the context of the current organization.
        /// </summary>
        public ClaimList Claims { get; set; }

        /// <summary>
        /// The language preference for the session identity.
        /// </summary>
        public string Language { get; private set; }

        IIdentity System.Security.Principal.IPrincipal.Identity
            => this;

        /// <summary>
        /// If the user is being impersonated by an administrator, this identifies the administrator who is doing the 
        /// impersonation.
        /// </summary>
        public Impersonator Impersonator { get; set; }

        /// <summary>
        /// An operator has full access to an organization account, regardless of permissions granted.
        /// </summary>
        public bool IsOperator => Person?.IsOperator ?? false;

        /// <summary>
        /// Returns true if the current user is an administrator impersonating another user.
        /// </summary>
        public bool IsImpersonating => Impersonator != null;

        /// <summary>
        /// Membership within a group is determined by the current organization.
        /// </summary>
        public GroupList Groups { get; set; }

        public Guid[] RoleIds
        {
            get
            {
                if (Groups == null || Groups.Count == 0)
                    return new Guid[0];

                return Groups.Select(x => x.Identifier).ToArray();
            }
        }

        /// <summary>
        /// A multi-organization user is a specific person within the context of a specific organization.
        /// </summary>
        public PersonModel Person { get; set; }

        /// <summary>
        /// A single user account may be represented as a distinct person in more than one organization.
        /// </summary>
        public PersonList Persons { get; set; }

        /// <summary>
        /// The current organization account selected for the user's current session.
        /// </summary>
        public OrganizationState Organization { get; set; }

        public Guid OrganizationId => Organization?.OrganizationIdentifier ?? Guid.Empty;

        /// <summary>
        /// The organization accounts to which the user has access.
        /// </summary>
        public OrganizationList Organizations { get; set; }

        /// <summary>
        /// The user account for the current session.
        /// </summary>
        public UserModel User { get; set; }

        public Guid UserId => User?.UserIdentifier ?? Guid.Empty;

        public TimeZoneInfo TimeZone => User?.TimeZone ?? TimeZoneInfo.Utc;

        /// <summary>
        /// The default constructor creates an empty list for each collection. Disallow construction 
        /// from outside this class.
        /// </summary>
        public CurrentIdentity()
        {
            Claims = new ClaimList();
            Groups = new GroupList();
            Organizations = new OrganizationList();
            Persons = new PersonList();
        }

        public static CurrentIdentity Create(
            string language,
            OrganizationState organization, OrganizationList organizations,
            UserModel user,
            Impersonator impersonator,
            PersonModel person, PersonList persons,
            GroupList groups,
            ClaimList claims)
        {
            var identity = new CurrentIdentity
            {
                Groups = groups,
                Impersonator = impersonator,
                Claims = claims,
                Person = person,
                Persons = persons,
                Organization = organization,
                Organizations = organizations,
                User = user,
            };

            AddAuthority(identity);

            identity.ChangeLanguage(language);

            return identity;
        }

        private static void AddAuthority(CurrentIdentity identity)
        {
            var access = AuthorityAccess.Unspecified;

            var roles = identity.Groups;

            AddSystemGroup(SystemRole.Any);

            var person = identity.Person;

            if (person == null)
                return;

            if (person.IsAdministrator)
            {
                access |= AuthorityAccess.Administrator;
                AddSystemGroup(SystemRole.Administrator);
            }

            if (person.IsDeveloper)
            {
                access |= AuthorityAccess.Developer;
                AddSystemGroup(SystemRole.Developer);
            }

            if (person.IsLearner)
            {
                access |= AuthorityAccess.Learner;
                AddSystemGroup(SystemRole.Learner);
            }

            if (person.IsOperator)
            {
                access |= AuthorityAccess.Operator;
                AddSystemGroup(SystemRole.Operator);
            }

            void AddSystemGroup(string name)
            {
                var role = new Group
                {
                    Identifier = UuidFactory.CreateV5(name),
                    Name = name,
                    Type = GroupType.Role
                };

                if (!roles.Any(r => r.Name == name))
                    roles.Add(role);
            }
        }

        public bool HasAccessToAllCompanies => IsOperator || IsInRole(CmdsRole.SystemAdministrators) || IsInRole(CmdsRole.Programmers);

        public bool IsAdministrator
            => Persons != null && Persons.Any(x => x.IsAdministrator);

        public bool IsDeveloper
            => Person?.IsDeveloper ?? false;

        public bool IsAuthenticated
            => User != null;

        public bool IsGranted(Guid? actionId, DataAccess? operation = null)
        {
            var isGrantedWithNewLogic = IsGrantedWithNewLogic(actionId, operation);

            if (CompareOldAndNewPermissionLogic)
            {
                var isGrantedWithOldLogic = IsGrantedWithOldLogic(actionId, operation);

                if (isGrantedWithOldLogic != isGrantedWithNewLogic)
                {
                    var action = ApplicationContext.GetAction(actionId.Value);

                    var resource = action.Url;

                    throw new InvalidOperationException($"The old authorization logic {(isGrantedWithOldLogic ? "grants" : "denies")} access"
                        + $" to {Name} on action"
                        + $" {actionId} ({resource}) and the new authorization logic {(isGrantedWithNewLogic ? "grants" : "denies")} access."
                        + " This means there is an unexpected problem in the new permission matrix.");
                }
            }

            return isGrantedWithNewLogic;
        }

        private bool IsGrantedWithOldLogic(Guid? actionId, DataAccess? operation = null)
        {
            if (IsOperator)
                return true;

            if (IsInRole(CmdsRole.Programmers))
                return true;

            if (actionId == null || actionId.Value == Guid.Empty)
                return true;

            var action = ApplicationContext.GetActionForPermission(actionId.Value);

            if (action == null)
                return false;

            var isPortal = action.Identifier == PortalActionIdentifier || action.Parent == PortalActionIdentifier;
            if (isPortal)
                return true;

            var parentId = action.Parent ?? Guid.Empty;

            if (operation != null)
                return Claims.IsGranted(action.Identifier, operation.Value) || Claims.IsGranted(parentId, operation.Value);

            if (Claims.Contains(action.Identifier) || Claims.Contains(parentId))
                return true;

            return false;
        }

        private bool IsGrantedWithNewLogic(Guid? actionId, DataAccess? operation)
        {
            if (actionId == null || actionId.Value == Guid.Empty)
                return true;

            var action = ApplicationContext.GetAction(actionId.Value);

            var resource = action.Url;

            var roleNames = GetRoleNames();

            var matrix = PermissionContext.GetMatrix();

            if (matrix.IsAllowed(Organization.Code, resource, roleNames, operation))
                return true;

            if (!string.IsNullOrEmpty(Partition))
                return matrix.IsAllowed(Partition, resource, roleNames, operation);

            return false;
        }

        public bool IsGranted(string actionUrl, DataAccess? operation = null)
        {
            Guid? actionId = null;

            if (!string.IsNullOrEmpty(actionUrl))
            {
                if (actionUrl.Contains("?"))
                    actionUrl = actionUrl.Substring(0, actionUrl.IndexOf("?"));

                if (actionUrl.StartsWith("/"))
                    actionUrl = actionUrl.Substring(1);

                var action = ApplicationContext.GetAction(actionUrl);

                actionId = action?.Identifier;
            }

            return IsGranted(actionId, operation);
        }

        public bool IsInRole(string role)
            => !string.IsNullOrEmpty(role) && Groups.Contains(role);

        public string Name
            => IsAuthenticated ? User.Email : UserNames.Someone;

        public static bool CompareOldAndNewPermissionLogic { get; set; }

        public string ChangeLanguage(string language)
        {
            if (!Organization.Languages.Any(x => string.Equals(x.TwoLetterISOLanguageName, language, StringComparison.OrdinalIgnoreCase)))
                language = Organization.Languages.Length > 0 ? Organization.Languages[0].TwoLetterISOLanguageName : "en";

            Language = language;
            return language;
        }

        public string[] GetRoleNames()
        {
            return Groups
                .Where(x => x.Type == GroupType.Role)
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToArray();
        }
    }
}