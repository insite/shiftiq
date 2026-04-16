using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using InSite.Api.Models.MailgunWebhook;
using InSite.Api.Settings;
using InSite.Application.Messages.Write;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Api.Controllers
{
    [DisplayName("Message")]
    [RoutePrefix("api/messages")]
    public class MessageController : ApiBaseController
    {
        #region Mailgun Webhook

        private static readonly TimeSpan _timestampTolerance = TimeSpan.FromMinutes(5);
        private static readonly MemoryCache<string, bool> _tokenCache = new MemoryCache<string, bool>();

        [HttpPost]
        [Route("handle-webhook")]
        public async Task<HttpResponseMessage> HandleWebhook()
        {
            var webhook = await TryReadAndValidatePayload();
            if (webhook.Error != null)
                return webhook.Error;

            var mailout = ServiceLocator.MessageSearch.FindMailout(webhook.MailoutId);
            if (mailout == null || !mailout.MessageIdentifier.HasValue)
                return ErrorInvalidPayload(Request, 100);

            var (status, recipient, data) = GetStatus(webhook.Payload);

            ServiceLocator.SendCommand(new HandleMailoutCallback(
                mailout.MessageIdentifier.Value, mailout.MailoutIdentifier,
                webhook.EventId, recipient, webhook.Payload.EventData.Timestamp.Value,
                status, data));

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private (string Status, string Recipient, Dictionary<string, string> Data) GetStatus(Payload payload)
        {
            string status, recipient;
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (payload.EventData is EventDataAccepted accepted)
                GetStatus(accepted, data, out status, out recipient);

            else if (payload.EventData is EventDataDelivered delivered)
                GetStatus(delivered, data, out status, out recipient);

            else if (payload.EventData is EventDataFailed failed)
                GetStatus(failed, data, out status, out recipient);

            else if (payload.EventData is EventDataComplained complained)
                GetStatus(complained, data, out status, out recipient);

            else if (payload.EventData is EventDataOpened opened)
                GetStatus(opened, data, out status, out recipient);

            else if (payload.EventData is EventDataClicked clicked)
                GetStatus(clicked, data, out status, out recipient);

            else if (payload.EventData is EventDataUnsubscribed unsubscribed)
                GetStatus(unsubscribed, data, out status, out recipient);

            else
                throw ApplicationError.Create("The event data handler is not implemented.");

            return (status, recipient, data);
        }

        private void GetStatus(EventDataAccepted e, Dictionary<string, string> data, out string status, out string recipient)
        {
            status = MailoutCallbackStatus.Accepted;
            recipient = e.Recipient.IfNullOrEmpty(e.Envelope?.Recipient);
        }

        private void GetStatus(EventDataDelivered e, Dictionary<string, string> data, out string status, out string recipient)
        {
            status = MailoutCallbackStatus.Delivered;
            recipient = e.Recipient.IfNullOrEmpty(e.Envelope?.Recipient);

            var ds = e.DeliveryStatus;
            if (ds != null)
            {
                data["code"] = ds.SmtpStatusCode.ToString();
                data["message"] = ds.Message;
                data["description"] = ds.Description;
            }
        }

        private void GetStatus(EventDataFailed e, Dictionary<string, string> data, out string status, out string recipient)
        {
            status = e.IsTemporary ? MailoutCallbackStatus.TemporaryFailed : MailoutCallbackStatus.Failed;
            recipient = e.Recipient.IfNullOrEmpty(e.Envelope?.Recipient);

            var ds = e.DeliveryStatus;
            if (ds != null)
            {
                data["code"] = ds.SmtpStatusCode.ToString();
                data["reason"] = e.Reason;
                data["message"] = ds.Message;
                data["description"] = ds.Description;
            }
        }

        private void GetStatus(EventDataComplained e, Dictionary<string, string> data, out string status, out string recipient)
        {
            status = MailoutCallbackStatus.Complained;
            recipient = e.Recipient;
        }

        private void GetStatus(EventDataOpened e, Dictionary<string, string> data, out string status, out string recipient)
        {
            status = MailoutCallbackStatus.Opened;
            recipient = e.Recipient;

            FillClientInformation(data, e.IpAddress, e.Geolocation, e.ClientInfo);
        }

        private void GetStatus(EventDataClicked e, Dictionary<string, string> data, out string status, out string recipient)
        {
            status = MailoutCallbackStatus.Clicked;
            recipient = e.Recipient;
            data["url"] = e.Url;

            FillClientInformation(data, e.IpAddress, e.Geolocation, e.ClientInfo);
        }

        private void GetStatus(EventDataUnsubscribed e, Dictionary<string, string> data, out string status, out string recipient)
        {
            status = MailoutCallbackStatus.Unsubscribed;
            recipient = e.Recipient;

            FillClientInformation(data, e.IpAddress, e.Geolocation, e.ClientInfo);
        }

        private void FillClientInformation(Dictionary<string, string> data, string ip, GeolocationData geo, ClientInfo clientInfo)
        {
            data["ip"] = ip;

            if (geo != null)
            {
                data["country"] = geo.Country;
                data["region"] = geo.Region;
                data["city"] = geo.City;
            }

            if (clientInfo != null)
                data["ua"] = clientInfo.UserAgent;
        }

        private class ValidatedWebhook
        {
            public HttpResponseMessage Error { get; set; }
            public Payload Payload { get; set; }
            public Guid EventId { get; set; }
            public Guid MailoutId { get; set; }
            public string SenderDomain { get; set; }
            public MailgunDomain MailgunDomain { get; set; }

            public static ValidatedWebhook InvalidPayload(HttpRequestMessage request, int errorNumber) => new ValidatedWebhook
            {
                Error = ErrorInvalidPayload(request, errorNumber)
            };
        }

        public static HttpResponseMessage ErrorInvalidPayload(HttpRequestMessage request, int errorNumber) =>
            DefaultJsonError(request, $"Invalid payload ({errorNumber}).", HttpStatusCode.NotAcceptable);

        private async Task<ValidatedWebhook> TryReadAndValidatePayload()
        {
            var result = await TryDeserializePayload();
            if (result.Error != null)
                return result;

            result = ValidateUserVariables(result);
            if (result.Error != null)
                return result;

            result = ValidateSignature(result);
            if (result.Error != null)
                return result;

            _tokenCache.Add(result.Payload.Signature.Token, true, (int)_timestampTolerance.TotalSeconds + 60);

            return result;
        }

        private async Task<ValidatedWebhook> TryDeserializePayload()
        {
            var json = await Request.Content.ReadAsStringAsync();
            if (json.HasNoValue())
                return ValidatedWebhook.InvalidPayload(Request, 0);

            var payload = JsonConvert.DeserializeObject<Payload>(json);
            if (payload == null || payload.Signature == null)
                return ValidatedWebhook.InvalidPayload(Request, 1);

            if (payload.EventData == null)
                return new ValidatedWebhook { Error = Request.CreateResponse(HttpStatusCode.OK) };

            if (payload.EventData.UserVariables == null)
                return ValidatedWebhook.InvalidPayload(Request, 2);

            if (!payload.EventData.TryParseEventGuid(out var eventId))
                return ValidatedWebhook.InvalidPayload(Request, 3);

            return new ValidatedWebhook
            {
                Payload = payload,
                EventId = eventId
            };
        }

        private ValidatedWebhook ValidateUserVariables(ValidatedWebhook data)
        {
            var userVariables = data.Payload.EventData.UserVariables;

            if (!userVariables.ContainsKey("mailout-id") || !Guid.TryParse(userVariables["mailout-id"], out var mailoutId))
                return ValidatedWebhook.InvalidPayload(Request, 4);

            if (!userVariables.TryGetValue("environment-domain", out var environmentDomain) || environmentDomain.IsEmpty())
                return ValidatedWebhook.InvalidPayload(Request, 5);

            if (!string.Equals(environmentDomain, HttpContext.Current.Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                return ValidatedWebhook.InvalidPayload(Request, 6);

            if (!userVariables.TryGetValue("sender-domain", out var senderDomain) || senderDomain.IsEmpty())
                return ValidatedWebhook.InvalidPayload(Request, 7);

            data.MailoutId = mailoutId;
            data.SenderDomain = senderDomain;

            return data;
        }

        private ValidatedWebhook ValidateSignature(ValidatedWebhook data)
        {
            var signature = data.Payload.Signature;

            if (!signature.TryGetTimestamp(out var signatureTimestamp))
                return ValidatedWebhook.InvalidPayload(Request, 8);

            var signatureAge = DateTime.UtcNow - signatureTimestamp;
            if (signatureAge < TimeSpan.Zero || signatureAge > _timestampTolerance)
                return ValidatedWebhook.InvalidPayload(Request, 9);

            var signatureToken = signature.Token;
            if (signatureToken.IsEmpty() || _tokenCache.TryGet(signatureToken, out _))
                return ValidatedWebhook.InvalidPayload(Request, 10);

            var mailgunDomain = ServiceLocator.AppSettings.Integration.Mailgun.Domains
                .FirstOrDefault(x => string.Equals(x.Domain, data.SenderDomain, StringComparison.OrdinalIgnoreCase));
            if (mailgunDomain == null)
                return ValidatedWebhook.InvalidPayload(Request, 11);

            if (!signature.IsValid(mailgunDomain.Token))
                return ValidatedWebhook.InvalidPayload(Request, 12);

            data.MailgunDomain = mailgunDomain;

            return data;
        }

        #endregion
    }
}