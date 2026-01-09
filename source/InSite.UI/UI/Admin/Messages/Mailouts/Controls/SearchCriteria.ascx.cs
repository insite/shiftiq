using System;
using System.Web;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Admin.Messages.Mailouts.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<MailoutFilter>
    {

        private string DefaultMessageName
        {
            get
            {
                var type = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(type))
                    return HttpUtility.UrlDecode(type);
                return null;
            }
        }

        private string DefaultOrganization
        {
            get
            {
                var organization = Request.QueryString["organization"];
                if (!string.IsNullOrEmpty(organization))
                    return HttpUtility.UrlDecode(organization);
                return null;
            }
        }

        public bool HasDefaultCriteria => (!string.IsNullOrEmpty(DefaultMessageName) & !string.IsNullOrEmpty(DefaultOrganization));

        public override MailoutFilter Filter
        {
            get
            {
                var filter = new MailoutFilter()
                {
                    OrganizationIdentifier = !IsPostBack && !string.IsNullOrEmpty(DefaultOrganization) ? new Guid(DefaultOrganization) : Organization.Identifier,
                    MessageName = !IsPostBack && !string.IsNullOrEmpty(DefaultMessageName) ? DefaultMessageName : null,
                    Sender = Sender.Text,
                    Recipient = Recipient.Text,
                    Subject = Subject.Text,
                    Status = Status.Value,
                    Scheduled = new DateTimeOffsetRange(ScheduledSince.Value, ScheduledBefore.Value),
                    Completed = new DateTimeOffsetRange(CompletedSince.Value, CompletedBefore.Value),
                    MessageType = MessageType.Value,
                    MinDeliveryCount = MinDeliveryCount.ValueAsInt,
                    MaxDeliveryCount = MaxDeliveryCount.ValueAsInt
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Sender.Text = value.Sender;
                Recipient.Text = value.Recipient;
                Subject.Text = value.Subject;
                Status.Value = value.Status;

                ScheduledSince.Value = value.Scheduled.Since;
                ScheduledBefore.Value = value.Scheduled.Before;

                CompletedSince.Value = value.Completed.Since;
                CompletedBefore.Value = value.Completed.Before;

                MessageType.Value = value.MessageType;

                MinDeliveryCount.ValueAsInt = value.MinDeliveryCount;
                MaxDeliveryCount.ValueAsInt = value.MaxDeliveryCount;
            }
        }

        public override void Clear()
        {
            Sender.Text = null;
            Recipient.Text = null;
            Subject.Text = null;
            Status.ClearSelection();
            ScheduledSince.Value = null;
            ScheduledBefore.Value = null;
            CompletedSince.Value = null;
            CompletedBefore.Value = null;
            MessageType.ClearSelection();
            MinDeliveryCount.ValueAsInt = null;
            MaxDeliveryCount.ValueAsInt = null;
        }
    }
}