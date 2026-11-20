using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using InSite.Application.Invoices.Read;

using Shift.Common;

namespace InSite.UI.Portal.Billing.Utilities
{
    public static class TaxHelper
    {
        public static ProvinceTax GetProvinceTax(Guid organizationId, string regionCode)
        {
            if (regionCode.IsEmpty() || regionCode == "--")
                return ProvinceTax.None;

            var taxes = ServiceLocator.InvoiceSearch.GetTaxes(new TTaxFilter
            {
                OrganizationIdentifier = organizationId,
                CountryCode = "CA",
                RegionCode = regionCode
            });

            var rate = taxes.Sum(x => x.TaxRate);
            if (rate > 1m)
                rate /= 100m;

            if (rate <= 0m)
                return ProvinceTax.None;

            var country = taxes.Select(x => x.CountryCode).FirstOrDefault();

            return new ProvinceTax(rate, country, regionCode);
        }

        public static decimal CalculateTax(decimal baseAmount, decimal? rate)
        {
            return rate.HasValue && rate.Value > 0m
                ? Math.Round(baseAmount * rate.Value, 2, MidpointRounding.AwayFromZero)
                : 0m;
        }

        public static string FormatRate(decimal rate)
        {
            return rate.ToString("0.##%", CultureInfo.InvariantCulture);
        }

        public static string FormatTaxAmount(decimal? amount, decimal? rate)
        {
            if (!amount.HasValue)
                return null;

            var result = amount.Value.ToString("c2");

            return rate.HasValue
                ? $"{result} ({FormatRate(rate.Value)})"
                : result;
        }

        public static (string Region, decimal? Rate, decimal? Amount) GetInvoiceTax(IEnumerable<QInvoiceItem> items)
        {
            var taxes = items?.Where(x => x.TaxRate > 0).ToArray();
            if (taxes.IsEmpty())
                return (null, null, null);

            var regions = taxes.Select(x => x.TaxRegion).Distinct().ToArray();
            var rates = taxes.Select(x => x.TaxRate).Distinct().ToArray();

            return (
                regions.Length == 1 ? regions[0] : null,
                rates.Length == 1 ? rates[0] : null,
                taxes.Sum(x => CalculateTax(x.ItemPrice * x.ItemQuantity, x.TaxRate))
            );
        }

        public static string GetRegionName(string regionCode)
        {
            return regionCode.IsEmpty() ? null : ServiceLocator.ProvinceSearch.Unabbreviate(regionCode);
        }

        public static bool IsProductTaxable(Guid? productId)
        {
            if (!productId.HasValue)
                return false;

            var product = ServiceLocator.InvoiceSearch.GetProduct(productId.Value);
            if (product == null)
                return false;

            if (!string.Equals(product.ProductType, "Seat", StringComparison.OrdinalIgnoreCase))
                return product.IsTaxable;

            var seat = ServiceLocator.EventSearch.GetSeat(productId.Value);

            return seat?.IsTaxable ?? false;
        }
    }
}
