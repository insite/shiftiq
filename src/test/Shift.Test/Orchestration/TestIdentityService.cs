
using System.Security.Principal;

using Shift.Common;
using Shift.Contract;
using Shift.Contract.Presentation;

namespace Shift.Test
{
    public class TestOrganization : Model
    {
        private static Guid TestId = Guid.Parse("657B5405-68D3-4A7A-95A3-C4E19D101352"); // Keyera
        private static string TestSlug = "keyera";

        public TestOrganization()
        {
            Identifier = TestId;
            Name = "Test Organization";
            Slug = TestSlug;
        }
    }

    public class TestPartition : Model
    {
        private static Guid TestId = Guid.Parse("8258CB0A-D1E8-4BC1-94B3-E70652503437"); // CMDS
        private static string TestSlug = "E03";

        public TestPartition()
        {
            Identifier = TestId;
            Name = "Test Partition";
            Slug = TestSlug;
        }
    }

    public class TestIdentityService : IShiftIdentityService
    {
        public Guid OrganizationId
        {
            get
            {
                var org = new TestOrganization();

                return org.Identifier;
            }
        }

        public IShiftPrincipal GetPrincipal()
        {
            return new TestPrincipal();
        }

        public TimeZoneInfo GetTimeZone()
        {
            return TimeZones.Mountain;
        }
    }

    public class TestPrincipal : IShiftPrincipal
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

        public Guid? OrganizationId => _organization.Identifier;

        public Guid? UserId => _actor.Identifier;

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
        public List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action)
        {
            var list = new List<BreadcrumbItem>();

            return list;
        }

        public List<NavigationList> SearchMenus(IShiftPrincipal principal, bool isCmds)
        {
            var list = new List<NavigationList>();

            return list;
        }

        public Task<List<NavigationItem>> SearchShortcutsAsync(IShiftPrincipal principal)
        {
            var list = new List<NavigationItem>();

            return Task.FromResult(list);
        }
    }
}