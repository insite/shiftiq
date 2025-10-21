using System.Linq;

using InSite.Application.Payments.Read;
using InSite.Domain.Payments;

using Shift.Constant;

namespace InSite.Persistence
{
    public class PaymentStore : IPaymentStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public void InsertPayment(PaymentImported e)
        {
            using (var db = CreateContext())
            {
                var payment = new QPayment
                {
                    OrganizationIdentifier = e.Tenant,
                    InvoiceIdentifier = e.Invoice,
                    PaymentIdentifier = e.Payment,
                    PaymentStatus = e.Status.ToString(),
                    PaymentStarted = e.Started,
                    PaymentApproved = e.Approved,
                    PaymentAmount = e.Amount,
                    CustomerIP = e.CustomerIP,
                    CreatedBy = e.CreatedBy,
                    TransactionId = e.TransactionId,
                    ResultCode = e.ResultCode,
                    ResultMessage = e.ResultMessage
                };

                db.QPayments.Add(payment);

                if (e.Approved.HasValue)
                {
                    var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == e.Invoice);
                    invoice.InvoiceStatus = InvoiceStatus.Paid.ToString();
                    invoice.InvoicePaid = e.Approved;
                }

                db.SaveChanges();
            }
        }

        public void InsertPayment(PaymentStarted e)
        {
            using (var db = CreateContext())
            {
                var payment = new QPayment
                {
                    OrganizationIdentifier = e.OriginOrganization,
                    InvoiceIdentifier = e.Invoice,
                    PaymentIdentifier = e.Payment,

                    PaymentStatus = PaymentStatus.Started.ToString(),
                    PaymentStarted = e.ChangeTime,

                    PaymentAmount = e.Request.Amount,
                    CustomerIP = e.Request.CustomerIP,

                    CardNumber = e.Request.Card.Number,
                    CardholderName = e.Request.Card.Name,

                    CreatedBy = e.OriginUser
                };
                db.QPayments.Add(payment);
                db.SaveChanges();
            }
        }

        public void UpdatePayment(PaymentCompleted e)
        {
            using (var db = CreateContext())
            {
                var payment = db.QPayments.Single(x => x.PaymentIdentifier == e.Payment);
                payment.PaymentStatus = PaymentStatus.Completed.ToString();

                if (e.Response.Approved)
                {
                    payment.PaymentApproved = e.ChangeTime;

                    var invoice = db.QInvoices.Single(x => x.InvoiceIdentifier == payment.InvoiceIdentifier);
                    invoice.InvoiceStatus = InvoiceStatus.Paid.ToString();
                    invoice.InvoicePaid = e.ChangeTime;
                }
                else
                    payment.PaymentDeclined = e.ChangeTime;

                payment.TransactionId = e.Response.TransactionIdentifier;
                payment.ResultCode = e.Response.AuthCode;
                payment.ResultMessage = e.Response.Message;

                db.SaveChanges();
            }
        }

        public void UpdatePayment(PaymentAborted e)
        {
            using (var db = CreateContext())
            {
                var payment = db.QPayments.Single(x => x.PaymentIdentifier == e.Payment);
                payment.PaymentStatus = PaymentStatus.Aborted.ToString();

                payment.PaymentAborted = e.ChangeTime;

                payment.ResultCode = e.Response.Code;
                payment.ResultMessage = e.Response.Message;

                db.SaveChanges();
            }
        }

        public void UpdatePayment(PaymentCreatedByModified e)
        {
            using (var db = CreateContext())
            {
                var payment = db.QPayments.Single(x => x.PaymentIdentifier == e.Payment);
                
                payment.CreatedBy = e.CreatedBy;

                db.SaveChanges();
            }
        }

        public void InsertDiscount(TDiscount discount)
        {
            using (var db = CreateContext())
            {
                db.TDiscounts.Add(discount);
                db.SaveChanges();
            }
        }

        public void UpdateDiscount(TDiscount discount)
        {
            using (var db = CreateContext())
            {
                var entity = db.TDiscounts.Single(x => x.DiscountCode == discount.DiscountCode);

                entity.DiscountPercent = discount.DiscountPercent;
                entity.DiscountDescription = discount.DiscountDescription;

                db.SaveChanges();
            }
        }

        public void DeleteDiscount(string discountCode)
        {
            using (var db = CreateContext())
            {
                var entities = db.TDiscounts.Where(x => x.DiscountCode == discountCode);

                db.TDiscounts.RemoveRange(entities);
                db.SaveChanges();
            }
        }
    }
}
