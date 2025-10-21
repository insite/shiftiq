using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Sales.Orders.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TOrderFilter>
    {
        private Dictionary<Guid, string> _userCache;

        protected override int SelectCount(TOrderFilter filter)
        {
            return ServiceLocator.InvoiceSearch.CountOrders(filter);
        }

        protected override IListSource SelectData(TOrderFilter filter)
        {
            var data = ServiceLocator.InvoiceSearch.GetOrders(filter, x => x.Customer, x => x.OrderItems)
                .Select(x => new
                {
                    CustomerFullName = x.Customer.UserFullName,
                    ProductName = x.ProductName,
                    ProductUrl = x.ProductUrl,
                    OrderIdentifier = x.OrderIdentifier,
                    TotalQuantity = x.OrderItems.IsEmpty() ? 0 : x.OrderItems.Sum(y => y.OrderItemQuantity),
                    TaxRate = x.TaxRate,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    Created = x.Created,
                    CreatedBy = x.CreatedBy,
                    Modified = x.Modified,
                    ModifiedBy = x.ModifiedBy
                })
                .ToList();

            var userFilter = new UserFilter
            {
                IncludeUserIdentifiers = data.Select(x => x.CreatedBy)
                    .Concat(data.Select(x => x.ModifiedBy))
                    .Distinct().ToArray()
            };

            _userCache = UserSearch
                .Bind(x => new { x.UserIdentifier, x.FullName }, userFilter)
                .ToDictionary(x => x.UserIdentifier, x => x.FullName);

            return data.ToSearchResult();
        }

        protected string GetUserName(string fieldName) =>
            GetUserName(Page.GetDataItem(), fieldName);

        private string GetUserName(object dataItem, string fieldName)
        {
            var userId = (Guid?)DataBinder.Eval(dataItem, fieldName);
            return userId.HasValue
                ? _userCache.GetOrDefault(userId.Value, string.Empty)
                : string.Empty;
        }

        protected string GetTimestampHtml(string dateField, string userField)
        {
            var dataItem = Page.GetDataItem();
            var date = (DateTimeOffset?)DataBinder.Eval(dataItem, dateField);
            var name = GetUserName(dataItem, userField);

            var builder = new StringBuilder();

            if (date.HasValue)
                builder.Append(TimeZones.Format(date.Value, User.TimeZone, true));

            if (name.HasValue())
            {
                if (builder.Length > 0) builder.Append("<br/>");

                builder.Append($"<small class=\"text-body-secondary\">by {name}</small>");
            }

            return builder.ToString();
        }
    }
}