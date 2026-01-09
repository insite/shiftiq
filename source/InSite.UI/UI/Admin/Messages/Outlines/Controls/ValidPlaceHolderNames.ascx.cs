using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Admin.Messages.Outlines.Controls
{
    public partial class ValidPlaceHolderNames : UserControl
    {
        public void LoadData(VMessage message)
        {
            Visible = false;

            if (message.MessageType != MessageTypeName.Notification && message.MessageType != MessageTypeName.Alert)
                return;

            if (message.MessageType == MessageTypeName.Alert)
                MessageVariablesLabel.Text = "Available Alert Variables";

            var notificationType = message.MessageName?.Replace(" ", string.Empty).ToEnumNullable<NotificationType>();
            if (!notificationType.HasValue)
                return;

            var changeType = Notifications.Select(notificationType.Value);
            if (changeType == null)
                return;

            var messageVariables = changeType.Variables;
            var recipientVariables = changeType.RecipientVariables;

            var hasMessageVariables = messageVariables.IsNotEmpty();
            var hasRecipientVariables = recipientVariables.IsNotEmpty();

            MessageVariablesField.Visible = hasMessageVariables;
            if (hasMessageVariables)
                MessageVariables.Text = FormatVariables(messageVariables);

            RecipientVariablesField.Visible = hasRecipientVariables;
            if (hasRecipientVariables)
                RecipientVariables.Text = FormatVariables(recipientVariables);

            Visible = hasMessageVariables || hasRecipientVariables;

            string FormatVariables(string value)
            {
                return string.Join(
                    ", ", 
                    value.Split(',').Select(x => x.Trim())
                        .Where(x => x.IsNotEmpty())
                        .Select(x => HttpUtility.HtmlEncode("$" + x))
                        .Select(x => $"<span class='message-variable' data-value='{x}' title='Click to copy {x}'>{x}</span>"));
            }
        }
    }
}