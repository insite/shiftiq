using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.MailgunWebhook;

namespace Shift.Mailgun;

using Microsoft.Extensions.Caching.Memory;

internal class PayloadValidator
{
    private readonly ILogger<PayloadValidator> _logger;
    private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _tokenCache;
    private readonly AppSettings _settings;
    private readonly QueueHelper _queueHelper;

    private static readonly TimeSpan _timestampTolerance = TimeSpan.FromMinutes(5);

    public PayloadValidator(
        ILogger<PayloadValidator> logger, 
        Microsoft.Extensions.Caching.Memory.IMemoryCache tokenCache, 
        AppSettings settings,
        QueueHelper queueHelper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _queueHelper = queueHelper ?? throw new ArgumentNullException(nameof(queueHelper));
    }

    public IResult? ParseAndValidate(string json, out string? environmentDomain, out string? queueName)
    {
        environmentDomain = null;
        queueName = null;

        if (json.HasNoValue())
        {
            _logger.LogWarning("Empty request body");
            return InvalidPayload(0);
        }

        var payload = JsonConvert.DeserializeObject<Payload>(json);

        if (payload == null)
        {
            _logger.LogWarning("Payload is null");
            return InvalidPayload(1);
        }

        if (payload.Signature == null)
        {
            _logger.LogWarning("Signature is null");
            return InvalidPayload(2);
        }

        if (payload.EventData == null)
            return Results.Ok();

        if (payload.EventData.UserVariables == null)
        {
            _logger.LogWarning("UserVariables is null");
            return InvalidPayload(3);
        }

        if (!payload.EventData.TryParseEventGuid(out _))
        {
            _logger.LogWarning("Failed to parse event GUID from EventData.Id");
            return InvalidPayload(4);
        }

        var userVariables = payload.EventData.UserVariables;

        if (!userVariables.TryGetValue("mailout-id", out var mailoutId))
        {
            _logger.LogWarning("Missing mailout-id in user-variables");
            return InvalidPayload(5);
        }

        if (!Guid.TryParse(mailoutId, out _))
        {
            _logger.LogWarning("Invalid mailout-id");
            return InvalidPayload(6);
        }

        if (!userVariables.TryGetValue("environment-domain", out environmentDomain) || environmentDomain.IsEmpty())
        {
            _logger.LogWarning("Missing environment-domain in user-variables");
            return InvalidPayload(7);
        }

        if (!_queueHelper.TryGetQueueName(environmentDomain, out queueName))
        {
            _logger.LogWarning("Invalid environment-domain: {EnvironmentDomain}", environmentDomain);
            return InvalidPayload(8);
        }

        if (!userVariables.TryGetValue("sender-domain", out var senderDomain) || senderDomain.IsEmpty())
        {
            _logger.LogWarning("Missing sender-domain in user-variables");
            return InvalidPayload(9);
        }

        if (!payload.Signature.TryGetTimestamp(out var signatureTimestamp))
        {
            _logger.LogWarning("Failed to parse signature timestamp");
            return InvalidPayload(10);
        }

        var signatureAge = DateTime.UtcNow - signatureTimestamp;
        if (signatureAge < TimeSpan.Zero || signatureAge > _timestampTolerance)
        {
            _logger.LogWarning("Signature timestamp out of tolerance: age={SignatureAge}", signatureAge);
            return InvalidPayload(11);
        }

        var signatureToken = payload.Signature.Token;

        if (signatureToken.IsEmpty())
        {
            _logger.LogWarning("Signature token is empty");
            return InvalidPayload(12);
        }

        if (_tokenCache.TryGetValue(signatureToken, out _))
        {
            _logger.LogWarning("Duplicate signature token");
            return InvalidPayload(13);
        }

        var mailgunDomain = _settings.Integration.Mailgun.Domains
            .FirstOrDefault(x => string.Equals(x.Domain, senderDomain, StringComparison.OrdinalIgnoreCase));

        if (mailgunDomain == null)
        {
            _logger.LogWarning("Unknown sender-domain: {SenderDomain}", senderDomain);
            return InvalidPayload(14);
        }

        if (!payload.Signature.IsValid(_settings.Integration.Mailgun.WebhookSigningKey))
        {
            _logger.LogWarning("Invalid signature hash");
            return InvalidPayload(15);
        }

        _tokenCache.Set(signatureToken, true, _timestampTolerance + TimeSpan.FromSeconds(60));

        return null;
    }

    static IResult InvalidPayload(int errorNumber) =>
        Results.Json(
            new { error = $"Invalid payload ({errorNumber:00})" },
            statusCode: StatusCodes.Status406NotAcceptable);
}
