using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Payments.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class PaymentSearch : IPaymentSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public PaymentSearch()
        {

        }

        #region Payments

        public QPayment GetPayment(Guid payment)
        {
            using (var db = CreateContext())
            {
                return db.QPayments.FirstOrDefault(x => x.PaymentIdentifier == payment);
            }
        }

        public QPayment GetPayment(Guid payment, params Expression<Func<QPayment, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPayments.ApplyIncludes(includes)
                    .FirstOrDefault(x => x.PaymentIdentifier == payment);
            }
        }

        public List<QPayment> GetPayments()
        {
            using (var db = CreateContext())
            {
                return db.QPayments.ToList();
            }
        }

        public int CountPayments(QPaymentFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<QPayment> GetPayments(QPaymentFilter filter, params Expression<Func<QPayment, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db, includes);

                query = !string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(filter.OrderBy)
                    : query.OrderBy(x => x.PaymentStarted);

                return query
                    .ApplyPaging(filter.Paging)
                    .ToList();
            }
        }

        public List<QPayment> GetRecentPayments(QPaymentFilter filter, int take, params Expression<Func<QPayment, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db, includes)
                    .OrderByDescending(x => x.PaymentStarted)
                    .Take(take)
                    .ToList();
            }
        }

        private static IQueryable<QPayment> CreateQuery(QPaymentFilter filter, InternalDbContext db, params Expression<Func<QPayment, object>>[] includes)
        {
            var query = db.QPayments.ApplyIncludes(includes);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.InvoiceIdentifier.HasValue)
                query = query.Where(x => x.InvoiceIdentifier == filter.InvoiceIdentifier);

            if (filter.CreatedBy.HasValue)
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);

            if (filter.PaymentStatus.HasValue())
                query = query.Where(x => x.PaymentStatus.Contains(filter.PaymentStatus));

            if (filter.CustomerName.HasValue())
                query = query.Where(x => x.CreatedByUser.UserFullName.Contains(filter.CustomerName));

            if (filter.CustomerEmail.HasValue())
                query = query.Where(x => x.CreatedByUser.UserEmail.Contains(filter.CustomerEmail));

            if (filter.Approved.HasValue && filter.Approved.Value)
                query = query.Where(x => x.PaymentApproved.HasValue);

            if (filter.Approved.HasValue && !filter.Approved.Value)
                query = query.Where(x => x.PaymentDeclined.HasValue);

            if (filter.MinAmount.HasValue)
                query = query.Where(x => x.PaymentAmount >= filter.MinAmount);

            if (filter.MaxAmount.HasValue)
                query = query.Where(x => x.PaymentAmount <= filter.MaxAmount);

            if (filter.PaymentAbortedSince.HasValue)
                query = query.Where(x => x.PaymentAborted >= filter.PaymentAbortedSince.Value);

            if (filter.PaymentAbortedBefore.HasValue)
                query = query.Where(x => x.PaymentAborted < filter.PaymentAbortedBefore.Value);

            if (filter.PaymentApprovedSince.HasValue)
                query = query.Where(x => x.PaymentApproved >= filter.PaymentApprovedSince.Value);

            if (filter.PaymentApprovedBefore.HasValue)
                query = query.Where(x => x.PaymentApproved < filter.PaymentApprovedBefore.Value);

            if (filter.PaymentDeclinedSince.HasValue)
                query = query.Where(x => x.PaymentDeclined >= filter.PaymentDeclinedSince.Value);

            if (filter.PaymentDeclinedBefore.HasValue)
                query = query.Where(x => x.PaymentDeclined < filter.PaymentDeclinedBefore.Value);

            if (filter.PaymentStartedSince.HasValue)
                query = query.Where(x => x.PaymentStarted >= filter.PaymentStartedSince.Value);

            if (filter.PaymentStartedBefore.HasValue)
                query = query.Where(x => x.PaymentStarted < filter.PaymentStartedBefore.Value);

            if (filter.ExcludeBrokenReferences)
                query = query.Where(x => x.CreatedByUser.UserFullName != null && x.CreatedInvoice.InvoiceStatus != null);

            if (filter.InvoiceNumber.HasValue)
                query = query.Where(x => x.CreatedInvoice.InvoiceNumber == filter.InvoiceNumber.Value);

            if (filter.TransactionIdentifier.HasValue())
                query = query.Where(x => x.TransactionId.Contains(filter.TransactionIdentifier));

            if(filter.CustomerEmployer.HasValue())
                query = query.Where(x => x.CreatedInvoice.CustomerEmployer.Contains(filter.CustomerEmployer));

            if (filter.ProductIdentifier.HasValue)
                query = query.Where(x => x.CreatedInvoice.InvoiceItems.Any(ii => ii.ProductIdentifier == filter.ProductIdentifier.Value));

            return query;
        }

        #endregion

        #region Discounts

        public TDiscount GetDiscount(string discountCode)
        {
            using (var db = CreateContext())
            {
                return db.TDiscounts.FirstOrDefault(x => x.DiscountCode == discountCode);
            }
        }

        public int CountDiscounts(TDiscountFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<TDiscount> GetDiscounts(TDiscountFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.DiscountCode)
                    .ApplyPaging(filter.Paging)
                    .ToList();
            }
        }

        private static IQueryable<TDiscount> CreateQuery(TDiscountFilter filter, InternalDbContext db)
        {
            var query = db.TDiscounts.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.DiscountCode.HasValue())
                query = query.Where(x => x.DiscountCode.Contains(filter.DiscountCode));

            if (filter.DiscountDescription.HasValue())
                query = query.Where(x => x.DiscountDescription.Contains(filter.DiscountDescription));

            return query;
        }

        #endregion
    }
}
