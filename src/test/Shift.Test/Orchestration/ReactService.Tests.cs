using Microsoft.Extensions.DependencyInjection;

using Shift.Common;
using Shift.Contract;
using Shift.Contract.Presentation;
using Shift.Service.Content;
using Shift.Service.Presentation;
using Shift.Service.Security;

namespace Shift.Test.Orchestration
{
    [Collection(PlatformFixtures.PlatformTest)]
    public class ReactServiceTests : IClassFixture<PlatformFixture>
    {
        private readonly IActionService _actionService;
        private readonly QueryTypeCollection _queryTypeCollection;
        private readonly OrganizationService _organizationService;
        private readonly PermissionService _permissionService;
        private readonly ReleaseSettings _releaseSettings;
        private readonly SecuritySettings _securitySettings;

        private readonly AppSettings _appSettings;
        private readonly ReactStartupOptions _startupOptions;
        private readonly Platform _platform;
        private readonly OrganizationAdapter _organizationAdapter;
        private readonly UserService _userService;
        private readonly INavigationService _navigationService;
        private readonly ILabelService _labelService;
        private readonly IPageService _pageService;
        private readonly TInputReader _inputReader;
        private readonly PartitionFieldService _partitionService;

        public ReactServiceTests(PlatformFixture fixture)
        {
            _actionService = fixture.ServiceProvider.GetRequiredService<IActionService>();
            _organizationService = fixture.ServiceProvider.GetRequiredService<OrganizationService>();
            _permissionService = fixture.ServiceProvider.GetRequiredService<PermissionService>();
            _queryTypeCollection = fixture.ServiceProvider.GetRequiredService<QueryTypeCollection>();
            _releaseSettings = fixture.ServiceProvider.GetRequiredService<ReleaseSettings>();
            _securitySettings = fixture.ServiceProvider.GetRequiredService<SecuritySettings>();

            _appSettings = fixture.ServiceProvider.GetRequiredService<AppSettings>();
            _startupOptions = fixture.ServiceProvider.GetRequiredService<ReactStartupOptions>();
            _platform = fixture.ServiceProvider.GetRequiredService<Platform>();
            _organizationAdapter = fixture.ServiceProvider.GetRequiredService<OrganizationAdapter>();
            _userService = fixture.ServiceProvider.GetRequiredService<UserService>();
            _navigationService = fixture.ServiceProvider.GetRequiredService<INavigationService>();
            _labelService = fixture.ServiceProvider.GetRequiredService<ILabelService>();
            _pageService = fixture.ServiceProvider.GetRequiredService<IPageService>();
            _inputReader = fixture.ServiceProvider.GetRequiredService<TInputReader>();
            _partitionService = fixture.ServiceProvider.GetRequiredService<PartitionFieldService>();
        }

        [Fact]
        public async Task Constructor_TestOperatorPermission_Success()
        {
            var matrix = new PermissionMatrix();

            var loader = new PermissionMatrixLoader(
                _releaseSettings,
                _securitySettings,
                _organizationService,
                _actionService,
                _permissionService,
                _queryTypeCollection
                );

            await loader.LoadAsync(matrix);

            var permissionMatrixProvider = new PermissionMatrixProvider();

            permissionMatrixProvider.SetMatrix(matrix);

            var react = new ReactService(
                _appSettings,
                _startupOptions,
                _platform,
                permissionMatrixProvider,
                _actionService,
                _organizationService,
                _organizationAdapter,
                _userService,
                _navigationService,
                _labelService,
                _pageService,
                _inputReader,
                _partitionService
                );

            var testPrincipal = new TestPrincipal();

            Assert.True(testPrincipal.IsOperator);

            var siteSettings = await react.RetrieveSiteSettingsAsync(testPrincipal, true);

            Assert.True(siteSettings.Permissions.Accounts);
            Assert.True(siteSettings.Permissions.Integrations);
            Assert.True(siteSettings.Permissions.Settings);
        }
    }
}
