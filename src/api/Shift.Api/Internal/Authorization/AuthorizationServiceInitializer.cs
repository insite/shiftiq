namespace Shift.Api;

public class AuthorizationServiceInitializer : IHostedService
{
    private readonly PermissionMatrixLoader _permissiongMatrixLoader;
    private readonly PermissionMatrixProvider _permissionMatrixProvider;

    public AuthorizationServiceInitializer(
        PermissionMatrixLoader permissionMatrixLoader,
        PermissionMatrixProvider permissionMatrixProvider,
        ILogger<AuthorizationServiceInitializer> logger)
    {
        _permissiongMatrixLoader = permissionMatrixLoader;
        _permissionMatrixProvider = permissionMatrixProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var matrix = new PermissionMatrix();

        await _permissiongMatrixLoader.LoadAsync(matrix, cancellationToken);

        // TODO: Add resources from the permission matrix. Resources = Authorization Requirement policies!
        // foreach (var requirement in _authorizationRequirements)
        //    matrix.AddResource(requirement.Policy);

        _permissionMatrixProvider.SetMatrix(matrix);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
