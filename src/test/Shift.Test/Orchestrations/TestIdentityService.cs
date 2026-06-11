
using System.Security.Principal;

using Shift.Common;
using Shift.Contract;
using Shift.Contract.Presentation;
using Shift.Sdk.UI.Navigation;

namespace Shift.Test
{
    public class TestOrganization : Model
    {
        private static Guid TestId = Guid.Parse("4d22c740-88ed-4dde-9de1-ac3a011805cf"); // Demo
        private static string TestSlug = "demo";

        public TestOrganization()
        {
            Identifier = TestId;
            Name = "Test Organization";
            Slug = TestSlug;
        }
    }

    public class TestPartition : Model
    {
        private static Guid TestId = Guid.Parse("0c071b03-6fe1-400f-82f4-78ff6f751ae7"); // E01
        private static string TestSlug = "e01";

        public TestPartition()
        {
            Identifier = TestId;
            Name = "Test Partition";
            Slug = TestSlug;
        }
    }

    public class TestIdentityService
    {
        public Guid OrganizationId
        {
            get
            {
                var org = new TestOrganization();

                return org.Identifier;
            }
        }

        public bool AllowOrganizationAccess(Shift.Common.IPrincipal principal, Guid organization)
        {
            throw new NotImplementedException();
        }

        public void ClearCacheByUserId(Guid userId)
        {

        }

        public Guid? GetOrganizationId(Shift.Common.IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Shift.Common.IPrincipal GetPrincipal()
        {
            return new TestPrincipal();
        }

        public TimeZoneInfo GetTimeZone()
        {
            return TimeZones.Mountain;
        }

        public void ValidateOrganizationId(Shift.Common.IPrincipal principal, IQueryByOrganization query)
        {
            throw new NotImplementedException();
        }
    }

    public class TestPrincipal : Shift.Common.IPrincipal
    {
        private Actor _actor;
        private Model _organization;
        private Model _partition;

        private AuthorityAccess _authority = AuthorityAccess.Operator;
        private List<Role> _roles = new List<Role>();
        private string _ip = "127.0.0.1";

        public TestPrincipal()
        {
            var name = "Test Operator";

            _actor = new Actor
            {
                Identifier = UuidFactory.CreateV5(name),
                Name = name,
                TimeZone = TimeZones.Mountain.StandardName
            };

            _organization = new TestOrganization();

            _partition = new TestPartition();

            IsOperator = true;

            if (IsAdministrator)
                Roles.Add(new Role(SystemRole.Administrator));

            if (IsDeveloper)
                Roles.Add(new Role(SystemRole.Developer));

            if (IsOperator)
                Roles.Add(new Role(SystemRole.Operator));
        }

        public Guid CookieId
        {
            get => Guid.Empty;
            set { }
        }

        public IJwt Claims { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Actor User
        {
            get
            {
                return _actor;
            }
            set
            {

            }
        }

        public Model Organization
        {
            get => _organization;
            set => _organization = value;
        }

        public Model Person { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AuthorityAccess Authority
        {
            get => _authority;
            set => _authority = value;
        }

        public List<Role> Roles { get => _roles; set => _roles = value; }

        public Model Partition
        {
            get => _partition;
            set => _partition = value;
        }

        public string IPAddress
        {
            get => _ip;
            set => _ip = value;
        }

        public Proxy Proxy { get { return null!; } set { } }

        public string? AuthenticationType => throw new NotImplementedException();

        public bool IsAuthenticated => throw new NotImplementedException();

        public string? Name => _actor.Name;

        public IIdentity? Identity => throw new NotImplementedException();

        public Guid OrganizationId => _organization.Identifier;

        public Guid UserId => _actor.Identifier;

        public TimeZoneInfo TimeZone
        {
            get
            {
                if (User?.TimeZone == null)
                    return TimeZoneInfo.Utc;

                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone);
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.Utc;
                }
            }
        }

        public Guid[] RoleIds => _roles.Select(x => x.Identifier).ToArray();

        public bool IsAdministrator { get; set; }

        public bool IsDeveloper { get; set; }

        public bool IsOperator { get; set; }

        public bool IsInRole(string role)
        {
            return _roles.Any(x => x.Name == role);
        }
    }

    public class TestNavigationService : INavigationService
    {
        public NavigationHome? GetHome(Shift.Common.IPrincipal principal) => null;

        public List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action, Shift.Common.IPrincipal principal)
        {
            var list = new List<BreadcrumbItem>();

            return list;
        }

        public List<NavigationList> SearchMenus(Shift.Common.IPrincipal principal, bool isCmds)
        {
            var list = new List<NavigationList>();

            return list;
        }

        public List<NavigationList> SearchAdminMenus(Shift.Common.IPrincipal principal, bool isCmds)
        {
            var list = new List<NavigationList>();

            return list;
        }

        public Task<List<NavigationItem>> SearchShortcutsAsync(Shift.Common.IPrincipal principal)
        {
            var list = new List<NavigationItem>();

            return Task.FromResult(list);
        }
    }
}