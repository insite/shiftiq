using Microsoft.Extensions.DependencyInjection;

using Shift.Common;
using Shift.Contract;
using Shift.Service.Security;

namespace Shift.Test.Security
{
    [Collection(PlatformFixtures.PlatformTest)]
    public class PermissionMatrixLoaderTests : IClassFixture<PlatformFixture>
    {
        private readonly IActionService _actionService;
        private readonly QueryTypeCollection _queryTypeCollection;
        private readonly OrganizationService _organizationService;
        private readonly PermissionService _permissionService;
        private readonly ReleaseSettings _releaseSettings;
        private readonly SecuritySettings _securitySettings;

        public PermissionMatrixLoaderTests(PlatformFixture fixture)
        {
            _actionService = fixture.ServiceProvider.GetRequiredService<IActionService>();
            _organizationService = fixture.ServiceProvider.GetRequiredService<OrganizationService>();
            _permissionService = fixture.ServiceProvider.GetRequiredService<PermissionService>();
            _queryTypeCollection = fixture.ServiceProvider.GetRequiredService<QueryTypeCollection>();
            _releaseSettings = fixture.ServiceProvider.GetRequiredService<ReleaseSettings>();
            _securitySettings = fixture.ServiceProvider.GetRequiredService<SecuritySettings>();
        }

        [Fact]
        public async Task Load()
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
        }
    }
}
