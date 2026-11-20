using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Shift.Api;

internal class MailgunWebhookListener : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MailgunWebhookListener> _logger;
    private readonly RabbitMq _settings;
    private readonly string _queueName;

    public MailgunWebhookListener(
        IServiceProvider serviceProvider,
        ILogger<MailgunWebhookListener> logger,
        AppSettings settings,
        string queueName)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Integration.RabbitMq;
        _queueName = queueName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port ?? AmqpTcpEndpoint.UseDefaultPort,
                UserName = _settings.Username,
                Password = _settings.Password,
                Ssl = new SslOption
                {
                    Enabled = _settings.Mode == RabbitMqMode.Tls,
                    ServerName = _settings.Host
                }
            };

            await using var connection = await factory.CreateConnectionAsync(stoppingToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (s, e) => await HandleMessageAsync(channel, e, stoppingToken);

            await channel.BasicConsumeAsync(
                _queueName,
                autoAck: false,
                consumer,
                stoppingToken);

            _logger.LogInformation("Listening on RabbitMQ queue {QueueName}", _queueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal shutdown
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogWarning(ex, "RabbitMQ is not reachable: MailgunWebhookListener is disabled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in {NameOf}", nameof(MailgunWebhookListener));
            throw;
        }
    }

    private async Task HandleMessageAsync(IChannel channel, BasicDeliverEventArgs e, CancellationToken stoppingToken)
    {
        try
        {
            var json = Encoding.UTF8.GetString(e.Body.ToArray());

            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<MailgunWebhookHandler>();

                await handler.ProcessAsync(json);
            }

            await channel.BasicAckAsync(e.DeliveryTag, multiple: false, stoppingToken);
        }
        catch (Exception ex) when (IsTransient(ex))
        {
            _logger.LogWarning(ex, "Transient error processing webhook, returning to queue");
            await channel.BasicNackAsync(e.DeliveryTag, multiple: false, requeue: true, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook, discarding message");
            await channel.BasicAckAsync(e.DeliveryTag, multiple: false, stoppingToken);
        }
    }

    private static bool IsTransient(Exception ex)
    {
        return false; // TODO: Declare transient error types
    }

    internal static void RegisterService(IServiceCollection services, Serilog.ILogger logger, AppSettings settings)
    {
        if (settings.Integration.RabbitMq.Mode == RabbitMqMode.Off)
        {
            logger.Information("MailgunWebhookListener is disabled (RabbitMQ mode is Off)");
            return;
        }

        var queueHelper = new MailgunQueueHelper(settings);
        var shortName = MailgunQueueHelper.GetQueueShortName(settings);

        if (!queueHelper.TryGetQueueFullName(shortName, out var fullName))
        {
            logger.Information(
                "MailgunWebhookListener is disabled ({ShortName} partition is not supported)",
                shortName);
            return;
        }

        logger.Information("MailgunWebhookListener is enabled for queue {QueueName}", fullName);

        services.AddHostedService(sp => ActivatorUtilities.CreateInstance<MailgunWebhookListener>(sp, fullName));
    }
}
