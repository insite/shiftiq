using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Payments.Read
{
    public interface IPaymentSearch
    {
        QPayment GetPayment(Guid payment);
        QPayment GetPayment(Guid payment, params Expression<Func<QPayment, object>>[] includes);
        List<QPayment> GetPayments();
        int CountPayments(QPaymentFilter filter);
        List<QPayment> GetPayments(QPaymentFilter filter, params Expression<Func<QPayment, object>>[] includes);
        List<QPayment> GetRecentPayments(QPaymentFilter filter, int take, params Expression<Func<QPayment, object>>[] includes);

        TDiscount GetDiscount(string discountCode);
        int CountDiscounts(TDiscountFilter filter);
        List<TDiscount> GetDiscounts(TDiscountFilter filter);
    }
}
