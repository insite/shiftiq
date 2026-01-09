using System;
using System.Web.UI;

using InSite.Application.Messages.Read;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class MessageDetailsInfo : UserControl
    {
        public void BindMessage(VMessage message, TimeZoneInfo tz)
        {
            MessageLink.HRef = $"/ui/admin/messages/outline?message={message.MessageIdentifier}";
            MessageTitle.Text = message.MessageTitle;
            MessageType.Text = message.MessageType;
            ApplicationChangeType.Text = message.MessageName;
        }
    }
}