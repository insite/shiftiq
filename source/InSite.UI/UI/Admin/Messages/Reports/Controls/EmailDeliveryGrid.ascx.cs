using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Messages.Reports.Controls
{
    public partial class EmailDeliveryGrid : SearchResultsGridViewController<EmailFilter>
    {
        #region Classes

        private class DeliveryGridItem
        {
            public string FormattedStatusDate { get; set; }
            public DateTimeOffset? StatusDate { get; set; }
            public string RecipientName { get; set; }
            public string RecipientAddress { get; internal set; }
            public string DeliveryComment { get; set; }
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        public string DeliveryStatus
        {
            get => ViewState[nameof(DeliveryStatus)] as string;
            set => ViewState[nameof(DeliveryStatus)] = value;
        }

        protected Guid EmailIdentifier
        {
            get => (Guid)ViewState[nameof(EmailIdentifier)];
            set => ViewState[nameof(EmailIdentifier)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;
        }

        #endregion

        #region Event handlers

        private void FilterButton_Click(object sender, EventArgs e)
        {
            Search(new EmailFilter() { EmailIdentifier = EmailIdentifier, StatusCode = DeliveryStatus });
        }

        #endregion

        #region Methods (loading)

        public void LoadData(Guid mailout)
        {
            EmailIdentifier = mailout;

            Search(new EmailFilter() { EmailIdentifier = EmailIdentifier, StatusCode = DeliveryStatus });
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(EmailFilter filter)
        {
            return EmailSearch.Count(filter);
        }

        protected override IListSource SelectData(EmailFilter filter)
        {
            var deliveries = EmailSearch.Select(filter.EmailIdentifier);

            return deliveries
                .OrderBy(x => x.MailoutScheduled)
                .ApplyPaging(filter)
                .ToList()
                .Select(delivery =>
                {
                    var deliveryComment = delivery.MailoutStatusCode ?? "None";
                    var statusDate = delivery.MailoutScheduled;
                    var deliveryStatusDate = Format(delivery.MailoutScheduled, User.TimeZone);

                    var item = new DeliveryGridItem
                    {
                        FormattedStatusDate = deliveryStatusDate,
                        StatusDate = statusDate,
                        DeliveryComment = deliveryComment
                    };

                    if (delivery.RecipientListTo != null)
                    {
                        var to = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(delivery.RecipientListTo);

                        if (to != null && to.Count == 1)
                        {
                            item.RecipientName = to.Single().Value;
                            item.RecipientAddress = to.Single().Value;
                        }
                    }

                    return item;
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Helpers

        private string Format(DateTimeOffset? value, TimeZoneInfo timezone)
        {
            return value.Format(timezone, nullValue: string.Empty);
        }

        #endregion
    }
}