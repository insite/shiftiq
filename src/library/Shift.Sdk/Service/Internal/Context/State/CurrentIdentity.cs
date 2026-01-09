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
    public interface ISecurityFramework : IIdentity, IPrincipal, ISimplePrincipal
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
        bool IsActionAuthorized(string actionName);
        bool IsGranted(Guid? action);
        bool IsGranted(Guid? action, PermissionOperation operation);
        bool IsGranted(string action);
        bool IsGranted(string action, PermissionOperation operation);
        bool IsInGroup(string group);
        bool IsInGroup(string[] groups);
        bool IsInGroup(Guid group);
    }

    /// <summary>
    /// This class represents the user/organization identity for a session. It is intended as a Data Transfer Object class only, therefore it has no
    /// methods except the methods required to implement IPrincipal.
    /// </summary>
    [Serializable]
    public class CurrentIdentity : IIdentity, IPrincipal, ISecurityFramework
    {
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

        IIdentity IPrincipal.Identity
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

        public Guid? OrganizationId => Organization?.OrganizationIdentifier;

        /// <summary>
        /// The organization accounts to which the user has access.
        /// </summary>
        public OrganizationList Organizations { get; set; }

        /// <summary>
        /// The user account for the current session.
        /// </summary>
        public UserModel User { get; set; }

        public Guid? UserId => User?.UserIdentifier;

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

            identity.ChangeLanguage(language);

            return identity;
        }

        public bool HasAccessToAllCompanies => IsOperator || IsInGroup(CmdsRole.SystemAdministrators) || IsInGroup(CmdsRole.Programmers);

        public bool IsActionAuthorized(string actionName)
        {
            if (IsOperator)
                return true;

            if (actionName.Contains("?"))
                actionName = actionName.Substring(0, actionName.IndexOf("?"));

            if (actionName.StartsWith("/"))
                actionName = actionName.Substring(1);

            var permission = ApplicationContext.GetPermission(actionName);
            if (permission == null)
                return false;

            if (IsGranted(actionName))
                return true;

            if (permission.Parent.HasValue)
            {
                var parent = ApplicationContext.GetAction(permission.Parent.Value);
                if (parent == null)
                    return false;

                return parent.Identifier == PortalActionIdentifier || IsGranted(parent.Identifier);
            }

            return false;
        }

        public bool IsAdministrator
            => Persons != null && Persons.Any(x => x.IsAdministrator);

        public bool IsDeveloper
            => Person?.IsDeveloper ?? false;

        public bool IsAuthenticated
            => User != null;

        public bool IsGranted(Guid? action)
        {
            // An operator (platform administrator) has access to all actions.
            if (IsOperator)
                return true;

            if (action == null || action.Value == Guid.Empty)
                return true;

            if (IsInRole(CmdsRole.Programmers))
                return true;

            if (Claims.Contains(action.Value))
                return true;

            var a = ApplicationContext.GetAction(action.Value);
            return HasPortalPermission(a);
        }

        public bool IsGranted(Guid? action, PermissionOperation operation)
        {
            // An operator (platform administrator) has access to all actions.
            if (IsOperator)
                return true;

            if (action == null)
                return true;

            var a = ApplicationContext.GetAction(action.Value);

            if (a == null)
                return false;

            if (HasPortalPermission(a))
                return true;

            return Claims.IsGranted(action.Value, operation);
        }

        public bool IsGranted(string action)
        {
            // An operator (platform administrator) has access to all actions.
            if (IsOperator)
                return true;

            if (Claims.Contains(action))
                return true;

            var a = ApplicationContext.GetAction(action);
            return HasPortalPermission(a);
        }

        public bool IsGranted(string action, PermissionOperation operation)
        {
            // An operator (platform administrator) has access to all actions.
            if (IsOperator)
                return true;

            var a = ApplicationContext.GetAction(action);

            if (a == null)
                return false;

            if (HasPortalPermission(a))
                return true;

            return Claims.IsGranted(action, operation);
        }

        private bool HasPortalPermission(ActionNode a)
        {
            if (a?.Url != null)
                return ApplicationContext.GetPermission(a.Url)?.Parent == PortalActionIdentifier;

            return false;
        }

        public bool IsInGroup(string group)
            => Groups.Contains(group);

        public bool IsInGroup(string[] groups)
            => groups.Any(group => Groups.Contains(group));

        public bool IsInGroup(Guid group)
            => Groups.Any(g => g.Identifier == group);

        public bool IsInRole(string role)
            => User != null && IsInGroup(role);

        public string Name
            => IsAuthenticated ? User.Email : UserNames.Someone;

        public string ChangeLanguage(string language)
        {
            if (!Organization.Languages.Any(x => string.Equals(x.TwoLetterISOLanguageName, language, StringComparison.OrdinalIgnoreCase)))
                language = Organization.Languages.Length > 0 ? Organization.Languages[0].TwoLetterISOLanguageName : "en";

            Language = language;
            return language;
        }
    }
}