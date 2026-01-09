using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Invoices.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class InvoiceSearch : IInvoiceSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        #region Invoices

        public bool InvoiceIsPaid(Guid invoiceIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QPayments
                    .Where(x =>
                        x.InvoiceIdentifier == invoiceIdentifier
                        && x.PaymentStatus == "Completed"
                    )
                    .Any();
            }
        }

        public bool InvoiceHasRegistration(Guid invoiceIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QPayments
                    .Where(x =>
                        x.InvoiceIdentifier == invoiceIdentifier
                        && x.PaymentStatus == "Completed"
                        && x.Registrations.Any()
                    )
                    .Any();
            }
        }

        public VInvoice GetInvoice(Guid invoice, params Expression<Func<VInvoice, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.VInvoices.ApplyIncludes(includes).FirstOrDefault(x => x.InvoiceIdentifier == invoice);
            }
        }

        public List<VInvoice> GetInvoices()
        {
            using (var db = CreateContext())
            {
                return db.VInvoices.ToList();
            }
        }

        public int CountInvoices(VInvoiceFilter filter)
        {
            using (var db = CreateContext())
                return CreateInvoiceQuery(filter, db).Count();
        }

        public List<VInvoice> GetInvoices(VInvoiceFilter filter, params Expression<Func<VInvoice, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateInvoiceQuery(filter, db, includes)
                    .OrderByDescending(x => x.InvoiceSubmitted)
                    .ThenBy(x => x.CustomerFullName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<VInvoice> CreateInvoiceQuery(VInvoiceFilter filter, InternalDbContext db, params Expression<Func<VInvoice, object>>[] includes)
        {
            var query = db.VInvoices.ApplyIncludes(includes);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.CustomerIdentifier.HasValue)
                query = query.Where(x => x.CustomerIdentifier == filter.CustomerIdentifier);

            if (filter.ProductIdentifier.HasValue)
                query = query.Where(x => x.InvoiceItems.Any(y => y.ProductIdentifier == filter.ProductIdentifier));

            if (filter.CustomerName.HasValue())
                query = query.Where(x => x.CustomerFullName.Contains(filter.CustomerName));

            if (filter.CustomerEmployer.HasValue())
                query = query.Where(x => x.CustomerEmployer.Contains(filter.CustomerEmployer));

            if (filter.CustomerPersonCode.HasValue())
                query = query.Where(x => x.CustomerPersonCode.Contains(filter.CustomerPersonCode));

            if (filter.CustomerEmail.HasValue())
                query = query.Where(x => x.CustomerEmail.Contains(filter.CustomerEmail));

            if (filter.InvoiceStatus.IsNotEmpty())
                query = query.Where(x => x.InvoiceStatus == filter.InvoiceStatus);

            if (filter.ExcludeInvoiceStatuses.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeInvoiceStatuses.Contains(x.InvoiceStatus));

            if (filter.InvoiceDraftedSince.HasValue)
                query = query.Where(x => filter.InvoiceDraftedSince <= x.InvoiceDrafted);

            if (filter.InvoiceDraftedBefore.HasValue)
                query = query.Where(x => x.InvoiceDrafted < filter.InvoiceDraftedBefore);

            if (filter.InvoiceSubmittedSince.HasValue)
                query = query.Where(x => filter.InvoiceSubmittedSince <= x.InvoiceSubmitted);

            if (filter.InvoiceSubmittedBefore.HasValue)
                query = query.Where(x => x.InvoiceSubmitted < filter.InvoiceSubmittedBefore);

            if (filter.InvoicePaidSince.HasValue)
                query = query.Where(x => filter.InvoicePaidSince <= x.InvoicePaid);

            if (filter.InvoicePaidBefore.HasValue)
                query = query.Where(x => x.InvoicePaid < filter.InvoicePaidBefore);

            if (filter.InvoiceNumber.HasValue)
                query = query.Where(x => x.InvoiceNumber == filter.InvoiceNumber.Value);

            if (filter.TransactionIdentifier.HasValue())
                query = query.Where(x => x.Payments.Any(y => y.TransactionId.Contains(filter.TransactionIdentifier)));

            return query;
        }

        #endregion

        #region Invoice Items

        public List<QInvoiceItem> GetInvoiceItems()
        {
            using (var db = CreateContext())
            {
                return db.QInvoiceItems.ToList();
            }
        }

        public int CountInvoiceItems(Guid invoiceIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QInvoiceItems.Count(x => x.InvoiceIdentifier == invoiceIdentifier);
            }
        }

        public List<QInvoiceItem> GetInvoiceItems(Guid invoiceIdentifier, params Expression<Func<QInvoiceItem, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QInvoiceItems
                    .Where(x => x.InvoiceIdentifier == invoiceIdentifier)
                    .ApplyIncludes(includes);

                return query
                    .OrderBy(x => x.ItemSequence)
                    .ToList();
            }
        }

        public QInvoiceItem GetInvoiceItem(Guid invoiceIdentifier, Guid itemIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QInvoiceItems.FirstOrDefault(x => x.InvoiceIdentifier == invoiceIdentifier && x.ItemIdentifier == itemIdentifier);
            }
        }

        public decimal GetInvoiceTotalAmount(Guid invoice)
        {
            using (var db = CreateContext())
            {
                var items = db.QInvoiceItems
                    .Where(x => x.InvoiceIdentifier == invoice)
                    .Select(x => new { x.ItemPrice, x.ItemQuantity, x.TaxRate })
                    .ToList();

                if (items.Count == 0) return 0m;

                decimal total = 0m;
                foreach (var i in items)
                {
                    var baseAmt = i.ItemPrice * i.ItemQuantity;
                    var rate = i.TaxRate ?? 0m;

                    var tax = Math.Round(baseAmt * rate, 2, MidpointRounding.AwayFromZero);
                    total += baseAmt + tax;
                }

                return total;
            }
        }

        #endregion

        #region Products

        public TProduct GetProduct(Guid product)
        {
            using (var db = CreateContext())
            {
                return db.TProducts.FirstOrDefault(x => x.ProductIdentifier == product);
            }
        }

        public int CountProducts(TProductFilter filter)
        {
            using (var db = CreateContext())
                return CreateProductQuery(filter, db).Count();
        }

        public List<TProduct> GetProducts(TProductFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateProductQuery(filter, db)
                    .OrderBy(x => x.ProductName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<TProduct> CreateProductQuery(TProductFilter filter, InternalDbContext db)
        {
            var query = db.TProducts.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.ProductIdentifier.HasValue)
                query = query.Where(x => x.ProductIdentifier == filter.ProductIdentifier);

            if (filter.ProductName.HasValue())
                query = query.Where(x => x.ProductName.Contains(filter.ProductName));

            if (filter.ProductDescription.HasValue())
                query = query.Where(x => x.ProductDescription.Contains(filter.ProductDescription));

            if (filter.ProductType.HasValue())
                query = query.Where(x => x.ProductType == filter.ProductType);

            if (filter.IsPublished.HasValue)
                query = query.Where(x => filter.IsPublished.Value ? x.Published.HasValue : !x.Published.HasValue);

            if (filter.IsAvailableForSale)
                query = query.Where(x => x.ObjectIdentifier.HasValue);

            if (filter.ProductQuantity.HasValue)
                query = query.Where(x => x.ProductQuantity == filter.ProductQuantity.Value);

            if (filter.ProductPrice.HasValue)
                query = query.Where(x => x.ProductPrice == filter.ProductPrice.Value);

            return query;
        }

        public int CountOrders(TOrderFilter filter)
        {
            using (var db = CreateContext())
                return CreateOrderQuery(filter, db).Count();
        }

        public List<TOrder> GetOrders(TOrderFilter filter, params Expression<Func<TOrder, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateOrderQuery(filter, db, includes)
                    .OrderBy(x => x.OrderCompleted)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<TOrder> CreateOrderQuery(TOrderFilter filter, InternalDbContext db, params Expression<Func<TOrder, object>>[] includes)
        {
            var query = db.TOrders.ApplyIncludes(includes).AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.CustomerIdentifier.HasValue)
                query = query.Where(x => x.CustomerUserIdentifier == filter.CustomerIdentifier);

            if (filter.FullName.HasValue())
            {
                query = query.Where(x =>
                            (x.Customer.UserFirstName + " " + x.Customer.UserLastName).Contains(filter.FullName)
                            || (x.Customer.UserLastName + ", " + x.Customer.UserFirstName).Contains(filter.FullName)
                            || x.Customer.UserFullName.Contains(filter.FullName));
            }

            return query;
        }

        public static MissingInvoiceNumber[] GetMissingInvoiceNumbers()
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Database.SqlQuery<MissingInvoiceNumber>("SELECT OrganizationIdentifier, InvoiceIdentifier, InvoiceDrafted FROM invoices.QInvoice WHERE InvoiceNumber IS NULL ORDER BY InvoiceDrafted").ToArray();
            }
        }

        #endregion

        #region Orders

        public Dictionary<Guid, Guid> GetInvoiceItemMap(Guid invoiceId)
        {
            using (var db = CreateContext())
            {
                return db.QInvoiceItems
                         .Where(x => x.InvoiceIdentifier == invoiceId)
                         .ToDictionary(x => x.ProductIdentifier, x => x.ItemIdentifier);
            }
        }

        #endregion

        #region Taxes

        public TTax GetTax(Guid taxId)
        {
            using (var db = new InternalDbContext(false))
                return db.TTaxes.Where(x => x.TaxIdentifier == taxId).FirstOrDefault();
        }

        public int CountTaxes(TTaxFilter filter)
        {
            using (var db = CreateContext())
                return CreateTaxQuery(filter, db).Count();
        }

        public List<TTax> GetTaxes(TTaxFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateTaxQuery(filter, db)
                    .OrderBy(x => x.TaxName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<TTax> CreateTaxQuery(TTaxFilter filter, InternalDbContext db)
        {
            var query = db.TTaxes.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.RegionCode))
                query = query.Where(x => x.RegionCode == filter.RegionCode);

            if (!string.IsNullOrEmpty(filter.TaxName))
                query = query.Where(x => x.TaxName.Contains(filter.TaxName));

            return query;
        }

        #endregion
    }
}
