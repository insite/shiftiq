using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Seats.Controls
{
    public partial class Detail : BaseUserControl
    {
        private List<SeatConfiguration.Price> Prices
        {
            get => (List<SeatConfiguration.Price>)ViewState[nameof(Prices)];
            set => ViewState[nameof(Prices)] = value;
        }

        private List<string> BillingCustomers
        {
            get => (List<string>)ViewState[nameof(BillingCustomers)];
            set => ViewState[nameof(BillingCustomers)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddPriceButton.Click += AddPriceButton_Click;
            PriceList.ItemCommand += PriceList_ItemCommand;

            AddCustomerButton.Click += AddCustomerButton_Click;
            CustomerList.ItemCommand += CustomerList_ItemCommand;

            SeatPrice.AutoPostBack = true;
            SeatPrice.SelectedIndexChanged += SeatPrice_SelectedIndexChanged;

            MultiplePriceValidator.ServerValidate += MultiplePriceValidator_ServerValidate;

            GroupStatus.Settings.OrganizationIdentifier = Organization.Identifier;
            GroupStatus.Settings.CollectionName = CollectionName.Contacts_Groups_Status_Name;
        }

        public void SetDefaultInputValues()
        {
            ShowPricePanels();
        }

        public void SetInputValues(QSeat seat)
        {
            var configuration = seat.Configuration.HasValue() ? JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration) : new SeatConfiguration();

            if (configuration.Prices == null)
            {
                SeatPrice.SelectedValue = "Free";
            }
            else if (configuration.Prices.Count == 1 && !configuration.Prices[0].Name.HasValue())
            {
                SeatPrice.SelectedValue = "Single";
                SinglePrice.ValueAsDecimal = configuration.Prices[0].Amount;
            }
            else
            {
                Prices = configuration.Prices;

                SeatPrice.SelectedValue = "Multiple";

                PriceList.DataSource = Prices;
                PriceList.DataBind();
            }

            ShowPricePanels();

            BillingCustomers = configuration.BillingCustomers;

            CustomerList.DataSource = BillingCustomers;
            CustomerList.DataBind();

            var content = ContentSeat.Deserialize(seat.Content);
            SeatName.Text = content.Title.Default;
            SeatDescription.Text = content.Description.Default;
            SeatAgreement.Text = content.Get("Agreement")?.Default;

            SeatAvailability.SelectedValue = seat.IsAvailable ? "Available" : "Hide";
            Taxes.SelectedValue = seat.IsTaxable ? "Yes" : "No";
            SeatOrderSequence.ValueAsInt = seat.OrderSequence;
        }

        public void GetInputValues(QSeat seat)
        {
            var configuration = new SeatConfiguration { BillingCustomers = BillingCustomers };

            if (SeatPrice.SelectedValue == "Single")
                configuration.Prices = new List<SeatConfiguration.Price> { new SeatConfiguration.Price { Amount = SinglePrice.ValueAsDecimal.Value } };
            else if (SeatPrice.SelectedValue == "Multiple")
                configuration.Prices = Prices;

            var content = new ContentSeat();
            content.Title.Default = SeatName.Text;
            content.Description.Default = SeatDescription.Text;
            content.AddOrGet("Agreement").Default = SeatAgreement.Text;

            seat.Configuration = JsonConvert.SerializeObject(configuration);
            seat.Content = content.Serialize();
            seat.IsAvailable = SeatAvailability.SelectedValue == "Available";
            seat.IsTaxable = Taxes.SelectedValue == "Yes";
            seat.OrderSequence = SeatOrderSequence.ValueAsInt;
            seat.SeatTitle = SeatName.Text;
        }

        private void AddPriceButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (Prices == null)
                Prices = new List<SeatConfiguration.Price>();

            var price = new SeatConfiguration.Price
            {
                Name = PriceName.Text,
                Amount = PriceAmount.ValueAsDecimal.Value,
                GroupStatus = GroupStatus.Value
            };

            Prices.Add(price);

            PriceList.DataSource = Prices;
            PriceList.DataBind();

            PriceName.Text = null;
            PriceAmount.ValueAsDecimal = null;
            GroupStatus.ClearSelection();
        }

        private void PriceList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Prices.RemoveAt(e.Item.ItemIndex);

            PriceList.DataSource = Prices;
            PriceList.DataBind();
        }

        private void AddCustomerButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (BillingCustomers == null)
                BillingCustomers = new List<string>();

            BillingCustomers.Add(CustomerName.Text);

            CustomerList.DataSource = BillingCustomers;
            CustomerList.DataBind();

            CustomerName.Text = null;
        }

        private void CustomerList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            BillingCustomers.RemoveAt(e.Item.ItemIndex);

            CustomerList.DataSource = BillingCustomers;
            CustomerList.DataBind();
        }

        private void SeatPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPricePanels();
        }

        private void MultiplePriceValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Prices.IsNotEmpty();
        }

        private void ShowPricePanels()
        {
            var isSingle = SeatPrice.SelectedValue == "Single";
            var isMultiple = SeatPrice.SelectedValue == "Multiple";

            SinglePriceArea.Visible = isSingle;
            SinglePriceRequiredValidator.Visible = isSingle;

            MultiplePriceArea.Visible = isMultiple;
        }
    }
}