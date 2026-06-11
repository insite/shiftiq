using Shift.Common.MailgunWebhook;
using Shift.Constant;

namespace Shift.Api;

internal static class MailgunWebhookHelper
{
    #region Status

    public class CallbackInfo
    {
        public string Status { get; }
        public string Recipient { get; }
        public Dictionary<string, string> Data { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public CallbackInfo(string status, string recipient)
        {
            Status = status;
            Recipient = recipient;
        }
    }

    public static CallbackInfo GetCallbackInfo(Payload payload)
    {
        return payload.EventData switch
        {
            EventDataAccepted e => GetCallbackInfo(e),
            EventDataDelivered e => GetCallbackInfo(e),
            EventDataFailed e => GetCallbackInfo(e),
            EventDataComplained e => GetCallbackInfo(e),
            EventDataOpened e => GetCallbackInfo(e),
            EventDataClicked e => GetCallbackInfo(e),
            EventDataUnsubscribed e => GetCallbackInfo(e),
            _ => throw new InvalidOperationException($"Unhandled event data type: {payload.EventData.GetType().Name}")
        };
    }

    private static CallbackInfo GetCallbackInfo(EventDataAccepted e)
    {
        return new CallbackInfo(MailoutCallbackStatus.Accepted, e.Recipient.IfNullOrEmpty(e.Envelope?.Recipient));
    }

    private static CallbackInfo GetCallbackInfo(EventDataDelivered e)
    {
        var result = new CallbackInfo(MailoutCallbackStatus.Delivered, e.Recipient.IfNullOrEmpty(e.Envelope?.Recipient));

        if (e.DeliveryStatus is { } ds)
        {
            result.Data["code"] = ds.SmtpStatusCode.ToString();
            result.Data["message"] = ds.Message;
            result.Data["description"] = ds.Description;
        }

        return result;
    }

    private static CallbackInfo GetCallbackInfo(EventDataFailed e)
    {
        var result = new CallbackInfo(
            e.IsTemporary ? MailoutCallbackStatus.TemporaryFailed : MailoutCallbackStatus.Failed,
            e.Recipient.IfNullOrEmpty(e.Envelope?.Recipient));

        if (e.DeliveryStatus is { } ds)
        {
            result.Data["code"] = ds.SmtpStatusCode.ToString();
            result.Data["reason"] = e.Reason;
            result.Data["message"] = ds.Message;
            result.Data["description"] = ds.Description;
        }

        return result;
    }

    private static CallbackInfo GetCallbackInfo(EventDataComplained e)
    {
        return new CallbackInfo(MailoutCallbackStatus.Complained, e.Recipient);
    }

    private static CallbackInfo GetCallbackInfo(EventDataOpened e)
    {
        var result = new CallbackInfo(MailoutCallbackStatus.Opened, e.Recipient);

        FillClientInfo(result, e.IpAddress, e.Geolocation, e.ClientInfo);

        return result;
    }

    private static CallbackInfo GetCallbackInfo(EventDataClicked e)
    {
        var result = new CallbackInfo(MailoutCallbackStatus.Clicked, e.Recipient);
        result.Data["url"] = e.Url;

        FillClientInfo(result, e.IpAddress, e.Geolocation, e.ClientInfo);

        return result;
    }

    private static CallbackInfo GetCallbackInfo(EventDataUnsubscribed e)
    {
        var result = new CallbackInfo(MailoutCallbackStatus.Unsubscribed, e.Recipient);

        FillClientInfo(result, e.IpAddress, e.Geolocation, e.ClientInfo);

        return result;
    }

    private static void FillClientInfo(CallbackInfo info, string ip, GeolocationData geo, ClientInfo clientInfo)
    {
        info.Data["ip"] = ip;

        if (geo != null)
        {
            info.Data["country"] = geo.Country;
            info.Data["region"] = geo.Region;
            info.Data["city"] = geo.City;
        }

        if (clientInfo != null)
            info.Data["ua"] = clientInfo.UserAgent;
    }

    #endregion
}
