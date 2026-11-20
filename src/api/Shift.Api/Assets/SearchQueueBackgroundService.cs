using Shift.Service.Content.Searches;

namespace Shift.Api;

internal class SearchQueueBackgroundService(
    SearchEntryService searchEntryService,
    ILogger<SearchQueueBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[SearchQueueBackgroundService] Service is started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processedCount = await searchEntryService.ProcessSearchQueueAsync(stoppingToken);

                logger.LogDebug($"[SearchQueueBackgroundService] Processed objects and permissions: {processedCount}");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SearchQueueBackgroundService] Service is failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }

        logger.LogInformation("[SearchQueueBackgroundService] Service is stopped");
    }
}