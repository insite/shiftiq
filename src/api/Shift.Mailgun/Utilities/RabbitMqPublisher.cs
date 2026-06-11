using System.Text;

using Newtonsoft.Json;

using RabbitMQ.Client;

using Shift.Common;

namespace Shift.Mailgun;

internal class RabbitMqPublisher : IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;

    private RabbitMqPublisher(IConnection connection, IChannel channel, ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _channel = channel;
        _logger = logger;
    }

    public static async Task<RabbitMqPublisher> CreateAsync(
        AppSettings settings, 
        ILogger<RabbitMqPublisher> logger, 
        QueueHelper queueHelper)
    {
        var rabbitMq = settings.Integration.RabbitMq;
        if (rabbitMq.Disabled)
            throw ApplicationError.Create("Cannot initialize {0}: RabbitMQ is disabled in configuration", nameof(RabbitMqPublisher));

        var factory = new ConnectionFactory
        {
            HostName = rabbitMq.Host,
            UserName = rabbitMq.Username,
            Password = rabbitMq.Password
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        foreach (var queue in queueHelper.EnumerateAllQueues())
        {
            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        logger.LogInformation("Connected to RabbitMQ at {Host}", rabbitMq.Host);

        return new RabbitMqPublisher(connection, channel, logger);
    }

    public async Task PublishAsync<T>(string queue, T message)
    {
        var json = JsonConvert.SerializeObject(message);

        await PublishJsonAsync(queue, json);
    }

    public async Task PublishJsonAsync(string queue, string json)
    {
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: queue,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error closing RabbitMQ connection");
        }
    }
}