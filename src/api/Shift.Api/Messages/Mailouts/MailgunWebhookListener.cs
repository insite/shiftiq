using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Shift.Api;

internal class MailgunWebhookListener : BackgroundService
{
    private readonly RabbitMq _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MailgunWebhookListener> _logger;
    private readonly string _queueName;

    private IConnection? _connection;
    private IChannel? _channel;

    public MailgunWebhookListener(
        AppSettings settings,
        IServiceProvider serviceProvider,
        ILogger<MailgunWebhookListener> logger)
    {
        _settings = settings.Integration.RabbitMq;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _queueName = $"mailgun-webhooks-{settings.Environment.Slug}-{settings.Partition.Slug}";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_settings.Disabled)
            return;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (s, e) => await HandleMessageAsync(e);

            await _channel.BasicConsumeAsync(
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

    private async Task HandleMessageAsync(BasicDeliverEventArgs e)
    {
        try
        {
            var json = Encoding.UTF8.GetString(e.Body.ToArray());

            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<MailgunWebhookHandler>();

                await handler.ProcessAsync(json);
            }

            await _channel!.BasicAckAsync(e.DeliveryTag, multiple: false);
        }
        catch (Exception ex) when (IsTransient(ex))
        {
            _logger.LogWarning(ex, "Transient error processing webhook, returning to queue");
            await _channel!.BasicNackAsync(e.DeliveryTag, multiple: false, requeue: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook, discarding message");
            await _channel!.BasicAckAsync(e.DeliveryTag, multiple: false);
        }
    }

    private static bool IsTransient(Exception ex)
    {
        return false; // TODO: Declare transient error types
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_channel != null)
                await _channel.CloseAsync(cancellationToken);

            if (_connection != null)
                await _connection.CloseAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error closing RabbitMQ connection");
        }

        await base.StopAsync(cancellationToken);
    }
}
