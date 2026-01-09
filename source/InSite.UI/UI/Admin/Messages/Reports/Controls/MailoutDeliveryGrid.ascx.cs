using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Messages.Reports.Controls
{
    public partial class MailoutDeliveryGrid : SearchResultsGridViewController<DeliveryFilter>
    {
        #region Classes

        private class DeliveryGridItem
        {
            public string FormattedStatusDate { get; set; }
            public DateTimeOffset? StatusDate { get; set; }
            public string RecipientName { get; set; }
            public string RecipientAddress { get; internal set; }
            public string DeliveryComment { get; set; }
            public string CarbonCopies { get; set; }
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        public string DeliveryStatus
        {
            get => ViewState[nameof(DeliveryStatus)] as string;
            set => ViewState[nameof(DeliveryStatus)] = value;
        }

        protected Guid MailoutIdentifier
        {
            get => (Guid)ViewState[nameof(MailoutIdentifier)];
            set => ViewState[nameof(MailoutIdentifier)] = value;
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
            Search(new DeliveryFilter() { MailoutIdentifier = MailoutIdentifier, Status = DeliveryStatus });
        }

        #endregion

        #region Methods (loading)

        public void LoadData(Guid mailout)
        {
            MailoutIdentifier = mailout;

            Search(new DeliveryFilter() { MailoutIdentifier = MailoutIdentifier, Status = DeliveryStatus });
        }

        #endregion

        #region Methods (data binding)

        protected override void Search(DeliveryFilter filter, int pageIndex)
        {
            if (!string.IsNullOrEmpty(FilterText.Text))
                filter.Keyword = FilterText.Text;

            base.Search(filter, pageIndex);
        }

        protected override int SelectCount(DeliveryFilter filter)
        {
            return ServiceLocator.MessageSearch.CountDeliveries(filter);
        }

        protected override IListSource SelectData(DeliveryFilter filter)
        {
            var deliveries = ServiceLocator.MessageSearch.GetDeliveries(filter);

            var items = deliveries
                .OrderBy(x => x.DeliveryCompleted)
                .ApplyPaging(filter)
                .ToList()
                .Select(delivery =>
                {
                    var deliveryComment = delivery.DeliveryError ?? "None";
                    var statusDate = delivery.DeliveryCompleted;
                    var deliveryStatusDate = Format(delivery.DeliveryCompleted, User.TimeZone);
                    
                    var carbonCopyEmails = ServiceLocator.MessageSearch.GetCarbonCopyEmails(delivery.CarbonCopies);

                    return new DeliveryGridItem
                    {
                        FormattedStatusDate = deliveryStatusDate,
                        StatusDate = statusDate,
                        RecipientName = delivery.PersonName,
                        RecipientAddress = delivery.UserEmail,
                        DeliveryComment = deliveryComment,
                        CarbonCopies = carbonCopyEmails
                    };
                })
                .ToList();

            return items.ToSearchResult();
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