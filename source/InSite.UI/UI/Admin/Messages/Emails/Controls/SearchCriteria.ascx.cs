using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Messages.Emails.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<EmailFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            StatusCodeSelector.LoadItems(
                EmailSearch.Distinct(x => x.MailoutStatusCode, x => true).Where(x => x.IsNotEmpty()).OrderBy(x => x));

            StatusMessageSelector.LoadItems(
                EmailSearch.Distinct(x => x.MailoutStatus, x => true).Where(x => x.IsNotEmpty()).OrderBy(x => x));
        }

        public override EmailFilter Filter
        {
            get
            {
                var filter = new EmailFilter
                {
                    EmailTo = ToEmail.Text,
                    EmailSubject = Subject.Text,
                    SenderEmail = SenderEmail.Text,
                    EmailBody = Body.Text,
                    SenderName = SenderName.Text,
                    StatusMessage = StatusMessageSelector.Value,
                    StatusCode = StatusCodeSelector.Value,
                    DeliverySuccessful = IsSuccessful.ValueAsBoolean,
                    DeliveredSince = DeliveryTimeSince.Value,
                    DeliveredBefore = DeliveryTimeBefore.Value,
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ToName = ToName.Text,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ToEmail.Text = value.EmailTo;
                Subject.Text = value.EmailSubject;
                SenderEmail.Text = value.SenderEmail;
                Body.Text = value.EmailBody;
                SenderName.Text = value.SenderName;
                StatusMessageSelector.Value = value.StatusMessage;
                StatusCodeSelector.Value = value.StatusCode;
                DeliveryTimeSince.Value = value.DeliveredSince;
                DeliveryTimeBefore.Value = value.DeliveredBefore;
                IsSuccessful.ValueAsBoolean = value.DeliverySuccessful;
                ToName.Text = value.ToName;
            }
        }

        public override void Clear()
        {
            ToEmail.Text = string.Empty;
            ToName.Text = string.Empty;
            Subject.Text = string.Empty;
            SenderEmail.Text = string.Empty;
            Body.Text = string.Empty;
            SenderName.Text = string.Empty;
            StatusMessageSelector.Value = string.Empty;
            StatusCodeSelector.Value = string.Empty;
            IsSuccessful.ValueAsBoolean = null;
            DeliveryTimeSince.Value = null;
            DeliveryTimeBefore.Value = null;
        }
    }
}