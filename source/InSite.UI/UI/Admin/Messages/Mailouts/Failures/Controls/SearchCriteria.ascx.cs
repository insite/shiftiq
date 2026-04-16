using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.UI.Admin.Messages.Mailouts.Failures.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<MailoutFailureFilter>
    {
        public override MailoutFailureFilter Filter
        {
            get
            {
                var filter = new MailoutFailureFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    Sender = Sender.Text,
                    Recipient = Recipient.Text,
                    Subject = Subject.Text,
                    MessageType = MessageType.Value,
                    Scheduled = new DateTimeOffsetRange(ScheduledSince.Value, ScheduledBefore.Value),
                    Failed = new DateTimeOffsetRange(FailedSince.Value, FailedBefore.Value)
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Sender.Text = value.Sender;
                Recipient.Text = value.Recipient;
                Subject.Text = value.Subject;
                MessageType.Value = value.MessageType;

                ScheduledSince.Value = value.Scheduled?.Since;
                ScheduledBefore.Value = value.Scheduled?.Before;

                FailedSince.Value = value.Failed?.Since;
                FailedBefore.Value = value.Failed?.Before;
            }
        }

        public override void Clear()
        {
            Sender.Text = null;
            Recipient.Text = null;
            Subject.Text = null;
            MessageType.ClearSelection();
            ScheduledSince.Value = null;
            ScheduledBefore.Value = null;
            FailedSince.Value = null;
            FailedBefore.Value = null;
        }
    }
}