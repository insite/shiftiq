using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Invoices.Read;

namespace InSite.Common.Web.UI
{
    public class FindProduct : BaseFindEntity<TProductFilter>
    {
        public TProductFilter Filter => (TProductFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new TProductFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier }));

        protected override string GetEntityName() => "Product";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (MaxSelectionCount != 1)
                throw new ArgumentException("FindProduct does not support multiple selection");
        }

        protected override TProductFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.ProductName = keyword;

            return filter;
        }

        protected override int Count(TProductFilter filter)
        {
            return ServiceLocator.InvoiceSearch.CountProducts(filter);
        }

        protected override DataItem[] Select(TProductFilter filter)
        {
            return ServiceLocator.InvoiceSearch
                .GetProducts(filter)
                .Select(x => new DataItem
                {
                    Value = x.ProductIdentifier,
                    Text = x.ProductName
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Array.Empty<DataItem>();

            var product = ServiceLocator.InvoiceSearch.GetProduct(ids[0]);

            if (product == null)
                return Array.Empty<DataItem>();

            return new[]
            {
                new DataItem
                {
                    Value = product.ProductIdentifier,
                    Text = product.ProductName
                }
            };
        }
    }
}