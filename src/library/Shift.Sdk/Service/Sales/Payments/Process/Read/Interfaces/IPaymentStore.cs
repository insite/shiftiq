using InSite.Domain.Payments;

namespace InSite.Application.Payments.Read
{
    public interface IPaymentStore
    {
        void InsertPayment(PaymentImported e);
        void InsertPayment(PaymentStarted e);
        void UpdatePayment(PaymentCompleted e);
        void UpdatePayment(PaymentAborted e);
        void UpdatePayment(PaymentCreatedByModified e);

        void InsertDiscount(TDiscount discount);
        void UpdateDiscount(TDiscount discount);
        void DeleteDiscount(string discountCode);
    }
}
