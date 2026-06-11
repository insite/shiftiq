using System.Diagnostics.CodeAnalysis;

using InSite.Application.Messages.Write;
using InSite.Application.People.Write;
using InSite.Domain.Contacts;

using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

using Shift.Common.MailgunWebhook;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Sdk.Service;
using Shift.Service.Directory;
using Shift.Service.Messaging;

namespace Shift.Api;

public class MailgunWebhookHandler(
    AppSettings appSettings,
    ICommanderAsync commander,
    MailoutService mailoutService,
    RecipientService recipientService,
    PersonService personService,
    IMemoryCache tokenCache,
    ILogger<MailgunWebhookHandler> logger
)
{
    private static readonly TimeSpan TimestampTolerance = TimeSpan.FromMinutes(5);
    private readonly string Domain = GetDomain(appSettings);

    public async Task ProcessAsync(string json)
    {
        if (!DeserializePayload(json, out var payload))
            return;

        if (!ValidateUserVariables(payload, out var mailoutId, out var senderDomain))
            return;

        if (!ValidateSenderDomain(senderDomain))
            return;

        if (!ValidateSignature(payload))
            return;

        var mailout = await mailoutService.RetrieveAsync(mailoutId);
        if (mailout == null || !mailout.MessageId.HasValue)
        {
            logger.LogWarning("Mailout {MailoutId} not found or has no MessageIdentifier", mailoutId);
            return;
        }

        if (!payload.EventData.TryParseEventGuid(out var eventId))
        {
            logger.LogWarning("Failed to parse event GUID from EventData.Id");
            return;
        }

        var info = MailgunWebhookHelper.GetCallbackInfo(payload);
        var commands = new List<ICommand>
        {
            new HandleMailoutCallback(
                mailout.MessageId.Value,
                mailout.MailoutId,
                "Mailgun",
                eventId,
                info.Recipient,
                payload.EventData.Timestamp!.Value,
                info.Status,
                info.Data
            )
        };

        if (info.Status == "Failed" || info.Status == "Delivered")
            await DisabledRecipientEmailAsync(mailout.MailoutId, info.Recipient, commands);

        await commander.SendCommandsAsync(OrganizationIdentifiers.Global, UserIdentifiers.Root, commands);

        logger.LogInformation(
            "Processed mailgun webhook: Mailout={MailoutId}, Status={Status}, Recipient={Recipient}",
            mailoutId, info.Status, info.Recipient);
    }

    private static string GetDomain(AppSettings appSettings)
    {
        var env = appSettings.Release.GetEnvironment();
        var part = appSettings.Partition;
        return (env.GetSubdomainPrefix() + part.Slug + "." + part.Domain).ToLowerInvariant();
    }

    private bool DeserializePayload(string json, [NotNullWhen(true)] out Payload? payload)
    {
        payload = null;

        if (json.HasNoValue())
        {
            logger.LogWarning("Empty RawJson in message");
            return false;
        }

        payload = JsonConvert.DeserializeObject<Payload>(json);

        if (payload == null)
        {
            logger.LogWarning("Payload is null");
            return false;
        }

        if (payload.Signature == null)
        {
            logger.LogWarning("Signature is null");
            return false;
        }

        if (payload.EventData == null)
        {
            logger.LogWarning("EventData is null");
            return false;
        }

        if (payload.EventData.UserVariables == null)
        {
            logger.LogWarning("UserVariables is null");
            return false;
        }

        return true;
    }

    private bool ValidateSignature(Payload payload)
    {
        var signature = payload.Signature;
        var config = appSettings.Integration.Mailgun;

        if (!signature.TryGetTimestamp(out var signatureTimestamp))
        {
            logger.LogWarning("Failed to parse signature timestamp");
            return false;
        }

        var signatureAge = DateTime.UtcNow - signatureTimestamp;
        if (signatureAge < TimeSpan.Zero || signatureAge > TimestampTolerance)
        {
            logger.LogWarning("Signature timestamp out of tolerance: age={SignatureAge}", signatureAge);
            return false;
        }

        var signatureToken = signature.Token;
        if (signatureToken.IsEmpty())
        {
            logger.LogWarning("Signature token is empty");
            return false;
        }

        if (tokenCache.TryGetValue(signatureToken, out _))
        {
            logger.LogWarning("Duplicate signature token: {SignatureToken}", signatureToken);
            return false;
        }

        if (!signature.IsValid(config.WebhookSigningKey))
        {
            logger.LogWarning("Invalid signature hash");
            return false;
        }

        var cacheExpiry = TimestampTolerance + TimeSpan.FromSeconds(60);
        tokenCache.Set(signatureToken, true, cacheExpiry);

        return true;
    }

    private bool ValidateUserVariables(Payload payload, out Guid mailoutId, [NotNullWhen(true)] out string senderDomain)
    {
        mailoutId = Guid.Empty;
        senderDomain = null!;

        var userVariables = payload.EventData.UserVariables;

        if (!userVariables.ContainsKey("mailout-id") || !Guid.TryParse(userVariables["mailout-id"], out mailoutId))
        {
            logger.LogWarning("Missing or invalid mailout-id in user-variables");
            return false;
        }

        if (!userVariables.TryGetValue("environment-domain", out var environmentDomain) || environmentDomain.IsEmpty())
        {
            logger.LogWarning("Missing environment-domain in user-variables");
            return false;
        }

        if (!string.Equals(environmentDomain, Domain, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("environment-domain mismatch: expected={Domain}, got={EnvironmentDomain}", Domain, environmentDomain);
            return false;
        }

        if (!userVariables.TryGetValue("sender-domain", out senderDomain!) || senderDomain.IsEmpty())
        {
            logger.LogWarning("Missing sender-domain in user-variables");
            return false;
        }

        return true;
    }

    private bool ValidateSenderDomain(string senderDomain)
    {
        var config = appSettings.Integration.Mailgun;
        var known = config.Domains.Any(x => string.Equals(x.Domain, senderDomain, StringComparison.OrdinalIgnoreCase));

        if (!known)
            logger.LogWarning("Unknown sender-domain: {SenderDomain}", senderDomain);

        return known;
    }

    private async Task DisabledRecipientEmailAsync(Guid mailoutId, string email, List<ICommand> commands)
    {
        if (email.IsEmpty())
            return;

        var recipient = (await recipientService.CollectAsync(new CollectRecipients
        {
            MailoutId = mailoutId,
            UserEmail = email
        })).FirstOrDefault();

        if (recipient == null)
            return;

        var person = (await personService.CollectAsync(new CollectPeople
        {
            UserId = recipient.UserId,
            OrganizationId = recipient.OrganizationId
        })).SingleOrDefault();

        if (person == null)
            return;

        if (person.EmailEnabled && string.Equals(person.UserEmail, email, StringComparison.OrdinalIgnoreCase))
        {
            var comment = GetComment("email", person.UserEmail);

            commands.Add(new ModifyPersonFieldBool(person.PersonId, PersonField.EmailEnabled, false));
            commands.Add(new ModifyPersonComment(person.PersonId, CommentActionType.Author, comment));
        }

        if (person.EmailAlternateEnabled && string.Equals(person.UserEmailAlternate, email, StringComparison.OrdinalIgnoreCase))
        {
            var comment = GetComment("alternate email", person.UserEmailAlternate);

            commands.Add(new ModifyPersonFieldBool(person.PersonId, PersonField.EmailAlternateEnabled, false));
            commands.Add(new ModifyPersonComment(person.PersonId, CommentActionType.Author, comment));
        }

        PersonComment GetComment(string emailType, string emailAddress)
        {
            return new PersonComment
            {
                Comment = UniqueIdentifier.Create(),
                Text = $"The {emailType} address {emailAddress} has been disabled due to a permanent delivery failure (mailout {mailoutId}).",

                Container = person.UserId,
                ContainerType = "Person",

                Author = UserIdentifiers.Root,
                AuthorName = "System",

                Organization = person.OrganizationId,

                Topic = person.UserId
            };
        }
    }
}