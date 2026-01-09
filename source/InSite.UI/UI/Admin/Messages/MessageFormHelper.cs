using System;
using System.Collections.Generic;
using System.Web;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;
using InSite.Persistence;

namespace InSite.Admin.Messages
{
    public static class MessageFormHelper
    {
        public static void SetupTitle(VMessage message, Action<string, string> setMethod)
        {
            var timestamp = UserSearch.GetTimestampHtml(message.LastChangeUserName, message.LastChangeType, null, message.LastChangeTime);
            setMethod($"{message.MessageName} <span class='form-text'>{message.MessageType}</span>", timestamp);
        }

        public static string GetNotificationDescription(NotificationType name)
            => GetNotificationDescription(Notifications.Select(name));

        public static string GetNotificationDescription(Notification notification)
        {
            if (notification == null)
                return string.Empty;

            var elements = new List<string>();
            if (!string.IsNullOrEmpty(notification.Purpose))
                elements.Add(HttpUtility.HtmlEncode(notification.Purpose));

            return string.Join(" ", elements);
        }
    }
}