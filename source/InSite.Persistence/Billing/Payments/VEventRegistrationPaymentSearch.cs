using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class VEventRegistrationPaymentSearch
    {
        private class VEventRegistrationPaymentReadHelper : ReadHelper<VEventRegistrationPayment>
        {
            public static readonly VEventRegistrationPaymentReadHelper Instance = new VEventRegistrationPaymentReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VEventRegistrationPayment>, TResult> func)
            {
                using (var db = new ReportDbContext())
                {
                    var query = db.VEventRegistrationPayments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static int Count(VEventRegistrationPaymentFilter filter) =>
            VEventRegistrationPaymentReadHelper.Instance.Count(
                (IQueryable<VEventRegistrationPayment> query) => query.Filter(filter));

        public static IList<T> Bind<T>(
            Expression<Func<VEventRegistrationPayment, T>> binder,
            VEventRegistrationPaymentFilter filter)
        {
            return VEventRegistrationPaymentReadHelper.Instance.Bind(
                (IQueryable<VEventRegistrationPayment> query) => query.Select(binder),
                (IQueryable<VEventRegistrationPayment> query) => query.Filter(filter),
                filter.Paging, filter.OrderBy, null);
        }
    }
}
