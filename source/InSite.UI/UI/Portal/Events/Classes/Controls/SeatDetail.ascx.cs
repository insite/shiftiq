using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class SeatDetail : UserControl
    {
        public event BooleanValueHandler AgreedChanged;

        public event EventHandler SeatChanged;

        private Guid EventIdentifier
        {
            get => (Guid)ViewState[nameof(EventIdentifier)];
            set => ViewState[nameof(EventIdentifier)] = value;
        }

        public Guid? SelectedSeatIdentifier
            => !string.IsNullOrEmpty(SelectedSeat.Value) ? Guid.Parse(SelectedSeat.Value) : (Guid?)null;

        public decimal Price
            => SinglePrice.Visible
                ? decimal.Parse(SinglePrice.Text, NumberStyles.Currency)
                : (MultiplePrice.Visible ? decimal.Parse(MultiplePrice.SelectedValue) : 0);

        public string BillingCustomer => BillingCustomerSelector.Value;

        public bool IsAgreed => Agreed.Checked;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ChangeSeatButton.Click += ChangeSeatButton_Click;

            MultiplePrice.AutoPostBack = true;
            MultiplePrice.SelectedIndexChanged += MultiplePrice_SelectedIndexChanged;

            IAgreeButton.Click += IAgreeButton_Click;
        }

        private void ChangeSeatButton_Click(object sender, EventArgs e)
            => SeatChanged?.Invoke(this, EventArgs.Empty);

        private void MultiplePrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Agreement.Visible = true;
        }

        private void IAgreeButton_Click(object sender, EventArgs e)
        {
            Agreed.Checked = true;
            AgreedChanged?.Invoke(this, new BooleanValueArgs(Agreed.Checked));
        }

        public void LoadData(QEvent @event)
        {
            EventIdentifier = @event.EventIdentifier;

            BindSeats(@event.EventIdentifier);
        }

        public void ShowSeatDetails(Guid? employerIdentifier)
        {
            BindSeats(EventIdentifier);

            PriceArea.Visible = true;
            FreePrice.Visible = false;
            SinglePrice.Visible = false;
            MultiplePrice.Visible = false;
            Agreement.Visible = false;

            var seatIdentifier = Guid.Parse(SelectedSeat.Value);
            var seat = ServiceLocator.EventSearch.GetSeat(seatIdentifier);
            var configuration = JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration);

            if (configuration.Prices == null)
            {
                FreePrice.Visible = true;
                Agreement.Visible = true;
            }
            else if (configuration.Prices.Count == 1 && !configuration.Prices[0].Name.HasValue())
            {
                SinglePrice.Visible = true;
                SinglePrice.Text = $"{configuration.Prices[0].Amount:c2}";

                Agreement.Visible = true;
            }
            else
            {
                BindMultiplePrice(configuration, employerIdentifier);
            }

            var content = ContentSeat.Deserialize(seat.Content);
            var agreement = content.GetAgreementHtml();

            AgreementText.Text = agreement;
            Agreed.Checked = false;

            BindBillingCustomers(configuration);
        }

        private void BindMultiplePrice(SeatConfiguration configuration, Guid? employerIdentifier)
        {
            MultiplePrice.Visible = true;
            MultiplePrice.Items.Clear();

            var employerStatusId = employerIdentifier.HasValue && (employerIdentifier.Value != Guid.Empty)
                ? ServiceLocator.GroupSearch.GetGroup(employerIdentifier.Value)?.GroupStatusItemIdentifier
                : null;
            var employerStatus = TCollectionItemCache.GetName(employerStatusId);

            var hideNonMemberPrices = CurrentSessionState.Identity.Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC
                && string.Equals(employerStatus, "Active Member", StringComparison.OrdinalIgnoreCase)
                && configuration.Prices.Any(x => string.Equals(x.GroupStatus, employerStatus, StringComparison.OrdinalIgnoreCase))
                && configuration.Prices.Any(x => !x.GroupStatus.HasValue());

            foreach (var price in configuration.Prices)
            {
                var item = new System.Web.UI.WebControls.ListItem($"{price.Name} {price.Amount:c2}", price.Amount.ToString());
                MultiplePrice.Items.Add(item);

                if (!string.IsNullOrEmpty(price.GroupStatus) && !StringHelper.Equals(price.GroupStatus, employerStatus)
                    || hideNonMemberPrices && !price.GroupStatus.HasValue()
                    )
                {
                    item.Enabled = false;
                }
            }
        }

        private void BindSeats(Guid eventIdentifier)
        {
            var seats = ServiceLocator.EventSearch
                .GetSeats(eventIdentifier, false);

            if (seats.Count == 1)
                SelectedSeat.Value = seats[0].SeatIdentifier.ToString();

            SeatRepeater.DataSource = seats;
            SeatRepeater.DataBind();
        }

        private void BindBillingCustomers(SeatConfiguration configuration)
        {
            var hasData = configuration.BillingCustomers.IsNotEmpty();

            BillingCustomerField.Visible = hasData;

            if (hasData)
                BillingCustomerSelector.LoadItems(configuration.BillingCustomers);
        }

        protected static string GetDescription(object item)
        {
            var seat = (QSeat)item;
            return ContentSeat.Deserialize(seat.Content).Description.Default;
        }
    }
}