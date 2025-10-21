using System;
using System.Linq;

using InSite.Application.Invoices.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ProductPriceSelectorComboBox : ComboBox
    {
        public const string SubscribeValue = "__SUBSCRIBE__";

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(new ListItem() { Value = Guid.Empty.ToString(), Text = "A-la-Carte" });

            var products = ServiceLocator.InvoiceSearch
                .GetProducts(new TProductFilter 
                    { 
                        OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                        IsPublished = true,
                        ProductType = "Package"
                    })
                .OrderBy(x => x.ProductPrice);

            foreach (var product in products)
                list.Add(product.ProductIdentifier.ToString(), $"{product.ProductName} ({product.ProductQuantity}-pack) ${product.ProductPrice}");

            if (products != null && products.Count() > 0)
                list.Add(new ListItem() { Value = SubscribeValue, Text = "Subscribe & Choose later" });

            return list;
        }
    }
}