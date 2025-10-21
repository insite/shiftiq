using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Invoices.Write;
using InSite.Application.Sales.Invoices.Write.Commands;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using InvoiceItem = InSite.Domain.Invoices.InvoiceItem;
using InvoiceStatusEnum = Shift.Constant.InvoiceStatus;

namespace InSite.Admin.Invoices.Forms
{
    public partial class Draft : AdminBasePage
    {
        private class Item
        {
            public Guid? ProductID { get; set; }
            public int? Quantity { get; set; }
            public decimal? Price { get; set; }
            public string Description { get; set; }
        }

        private List<Item> DataItems { get; set; }

        protected bool CanDeleteItem => DataItems.Count > 1;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemRepeater.ItemDataBound += ItemRepeater_ItemDataBound;
            ItemRepeater.ItemCommand += ItemRepeater_ItemCommand;

            AddItemButton.Click += AddItemButton_Click;

            SaveButton.Click += SaveButton_Click;

            BillingCustomerID.AutoPostBack = true;
            BillingCustomerID.ValueChanged += BillingCustomerID_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var title = "Draft Invoice";

                PageHelper.AutoBindHeader(this, null, title);

                DataItems = new List<Item>
                {
                    new Item()
                };

                BindItemRepeater();

                CancelButton.NavigateUrl = "/ui/admin/sales/invoices/search";

                BillingCustomerID.Filter.GroupType = GroupTypes.Employer;
                BillingCustomerID.Filter.OrganizationIdentifier = Organization.Key;
                EmployerContactID.Filter.OrganizationIdentifier = Organization.Key;

                InvoiceStatus.Value = InvoiceStatusEnum.Drafted.ToString();
            }
            else
            {
                ReadItemsFromReader();
            }
        }

        private void BillingCustomerID_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            if (BillingCustomerID.Value != null)
            {
                EmployerContactID.Value = null;
                EmployerContactID.Filter.EmployerGroupIdentifier = BillingCustomerID.Value;
            }
        }

        private void ItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (Item)e.Item.DataItem;

            var productIDComboBox = (ProductComboBox)e.Item.FindControl("ProductID");
            var quantityInput = (NumericBox)e.Item.FindControl("Quantity");
            var priceInput = (NumericBox)e.Item.FindControl("Price");
            var descriptionTextBox = (ITextBox)e.Item.FindControl("Description");

            productIDComboBox.ValueAsGuid = item.ProductID;
            quantityInput.ValueAsInt = item.Quantity;
            priceInput.ValueAsDecimal = item.Price;
            descriptionTextBox.Text = item.Description;
        }

        private void ItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var index = int.Parse((string)e.CommandArgument);

                DataItems.RemoveAt(index);

                BindItemRepeater();
            }
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            DataItems.Add(new Item());

            BindItemRepeater();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var invoiceItems = new List<InvoiceItem>();

            foreach (var item in DataItems)
            {
                invoiceItems.Add(new InvoiceItem
                {
                    Identifier = UniqueIdentifier.Create(),
                    Product = item.ProductID.Value,
                    Quantity = item.Quantity.Value,
                    Price = item.Price.Value,
                    Description = item.Description
                });
            }

            var invoiceId = UniqueIdentifier.Create();
            var invoiceNumber = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Invoice);

            ServiceLocator.SendCommand(new DraftInvoice(invoiceId, Organization.OrganizationIdentifier, invoiceNumber, EmployerContactID.Value.Value, invoiceItems.ToArray()));

            if (BillingCustomerID.Value != null)
                ServiceLocator.SendCommand(new ChangeBusinessCustomer(invoiceId, BillingCustomerID.Value));

            if (InvoiceStatus.Value != InvoiceStatusEnum.Drafted.ToString())
                ServiceLocator.SendCommand(new ChangeInvoiceStatus(invoiceId, InvoiceStatus.Value));

            Response.Redirect($"/ui/admin/sales/invoices/outline?id={invoiceId}");
        }

        private void BindItemRepeater()
        {
            ItemRepeater.DataSource = DataItems;
            ItemRepeater.DataBind();
        }

        private void ReadItemsFromReader()
        {
            DataItems = new List<Item>();

            foreach (RepeaterItem repeaterItem in ItemRepeater.Items)
            {
                var productIDComboBox = (ProductComboBox)repeaterItem.FindControl("ProductID");
                var quantityInput = (NumericBox)repeaterItem.FindControl("Quantity");
                var priceInput = (NumericBox)repeaterItem.FindControl("Price");
                var descriptionTextBox = (ITextBox)repeaterItem.FindControl("Description");
                if (productIDComboBox.ValueAsGuid.HasValue)
                {
                    var product = ServiceLocator.InvoiceSearch.GetProduct(productIDComboBox.ValueAsGuid.Value);
                    priceInput.ValueAsDecimal = product.ProductPrice ?? priceInput.ValueAsDecimal;
                }
                else
                {
                    priceInput.ValueAsDecimal = null;
                }
                var item = new Item
                {
                    ProductID = productIDComboBox.ValueAsGuid,
                    Quantity = quantityInput.ValueAsInt,
                    Price = priceInput.ValueAsDecimal,
                    Description = descriptionTextBox.Text
                };

                DataItems.Add(item);
            }
        }
    }
}