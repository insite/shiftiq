namespace Shift.Api;

/// <remarks>
/// Eager loading ensures the app absorbs the initialization cost during startup. No user is penalized, and we'll find 
/// out immediately if loading fails - rather than on the first request to the permission cache.
/// </remarks>
public class AuthorizationServiceInitializer : IHostedService
{
    private readonly PermissionCache _cache;

    public AuthorizationServiceInitializer(PermissionCache cache)
    {
        _cache = cache;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cache.Refresh(null);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
