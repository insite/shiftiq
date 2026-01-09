using System;
using System.Linq;
using System.Net;

using Shift.Common.Timeline.Changes;

using InSite.Application.Gateways.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Domain.Integrations.Bambora;
using InSite.Domain.Payments;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Payments.Read
{
    /// <summary>
    /// Implements the process manager for Payment events. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class PaymentChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IInvoiceSearch _invoices;
        private readonly IPaymentSearch _payments;
        private readonly IApiRequestLogger _apiRequestLogger;

        public PaymentChangeProcessor(
            ICommander commander,
            IChangeQueue publisher,
            IInvoiceSearch invoices,
            IPaymentSearch payments,
            IApiRequestLogger apiRequestLogger
            )
        {
            _commander = commander;
            _invoices = invoices;
            _payments = payments;
            _apiRequestLogger = apiRequestLogger;

            publisher.Subscribe<PaymentStarted>(Handle);
            publisher.Subscribe<PaymentCompleted>(Handle);
            publisher.Subscribe<PaymentAborted>(Handle);
        }

        private void Handle(PaymentStarted e)
        {
            // Bambora API Docs: https://dev.na.bambora.com/docs/references/payment_APIs/v1-0-5

            var body = BamboraPaymentRequest.Create(e.Request);
            var json = JsonConvert.SerializeObject(body);

            var bodyToSave = BamboraPaymentRequest.Create(e.Request);
            bodyToSave.card.number = MaskedCreditCard.MaskNumber(bodyToSave.card.number);
            bodyToSave.card.cvd = null;

            var jsonToSave = JsonConvert.SerializeObject(bodyToSave);

            var client = new IntegrationClient(HttpVerb.POST, IntegrationType.Bambora, e.OriginUser, e.OriginOrganization, _apiRequestLogger);

            var response = client.Request("payments", json, jsonToSave);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                SendCompletePayment(e, response);
            }
            else
            {
                SendAbortPayment(e, response);
            }
        }

        private void SendCompletePayment(PaymentStarted e, IntegrationResponse response)
        {
            var responseObject = JsonConvert.DeserializeObject<BamboraPaymentResponse>(response.Content);

            _commander.Send(new CompletePayment(e.AggregateIdentifier, e.Payment,
                new PaymentResponse
                {
                    TransactionIdentifier = responseObject.id,
                    Approved = responseObject.approved == 1,
                    AuthorizingMerchantId = responseObject.authorizing_merchant_id.ToString(),
                    MessageId = responseObject.message_id,
                    Message = responseObject.message,
                    AuthCode = responseObject.auth_code,
                    Created = responseObject.created,
                    OrderNumber = responseObject.order_number,
                    Type = responseObject.type,
                    RiskScore = (int)responseObject.risk_score,
                    Amount = responseObject.amount,
                    PaymentMethod = responseObject.payment_method,
                }
            ));
        }

        private void SendAbortPayment(PaymentStarted e, IntegrationResponse response)
        {
            var responseObject = JsonConvert.DeserializeObject<BamboraErrorResponse>(response.Content);

            _commander.Send(new AbortPayment(e.AggregateIdentifier, e.Payment,
                new ErrorResponse
                {
                    Code = responseObject.code.ToString(),
                    Category = responseObject.category.ToString(),
                    Message = responseObject.message,
                    Details = responseObject.details != null
                        ? responseObject.details.Select(x => new Detail { Field = x.field, Message = x.message }).ToArray()
                        : null
                }
            ));
        }

        private void Handle(PaymentCompleted e)
        {
            if (e.Response.Approved)
            {
                var payment = _payments.GetPayment(e.Payment);
                var paymentSum = _payments
                    .GetPayments(new QPaymentFilter { InvoiceIdentifier = payment.InvoiceIdentifier, Approved = true })
                    .Sum(x => x.PaymentAmount);

                var invoice = _invoices.GetInvoice(payment.InvoiceIdentifier);
                if (paymentSum >= invoice.InvoiceAmount)
                    _commander.Send(new PayInvoice(payment.InvoiceIdentifier, DateTimeOffset.UtcNow));
            }
        }

        private void Handle(PaymentAborted e)
        {
            var payment = _payments.GetPayment(e.Payment);

            _commander.Send(new FailInvoicePayment(payment.InvoiceIdentifier));
        }
    }
}