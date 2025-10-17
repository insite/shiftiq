namespace Shift.Api;

public class AuthorizerInitializationService : IHostedService
{
    private readonly AuthorizerFactory _authorizerFactory;
    private readonly AuthorizerHolder _holder;
    private readonly ILogger<AuthorizerInitializationService> _logger;

    public AuthorizerInitializationService(
        AuthorizerFactory authorizerFactory,
        AuthorizerHolder holder,
        ILogger<AuthorizerInitializationService> logger)
    {
        _authorizerFactory = authorizerFactory;
        _holder = holder;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing Authorizer...");

        var authorizer = await _authorizerFactory.CreateAsync(cancellationToken);

        var requirements = new RequirementList();

        authorizer.Requirements.AddRange(requirements.Items);

        _holder.Instance = authorizer;

        _logger.LogInformation("Authorizer initialized.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
