using System;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class FieldInputNotification : BaseUserControl
    {
        public event StringValueHandler NotificationChanged;

        private void OnNotificationChanged(string title) =>
            NotificationChanged?.Invoke(this, new StringValueArgs(title));

        public string Value
        {
            get => AlertInput.Value;
            set
            {
                AlertInput.Value = null;
                AlertDescription.InnerHtml = string.Empty;

                var type = value.ToEnumNullable<NotificationType>();
                if (!type.HasValue)
                    return;

                var notification = Notifications.Select(type.Value);
                if (notification != null)
                {
                    AlertInput.Value = notification.Slug;
                    AlertDescription.InnerHtml = MessageFormHelper.GetNotificationDescription(notification);
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AlertInput.AutoPostBack = true;
            AlertInput.ValueChanged += AlertInput_ValueChanged;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            AlertValidator.ValidationGroup = groupName;
        }

        private void AlertInput_ValueChanged(object sender, EventArgs e)
        {
            var type = AlertInput.Value.ToEnumNullable<NotificationType>();
            var notification = type.HasValue ? Notifications.Select(type.Value) : null;

            AlertDescription.InnerHtml = MessageFormHelper.GetNotificationDescription(notification);

            OnNotificationChanged(notification?.Subject);
        }
    }
}