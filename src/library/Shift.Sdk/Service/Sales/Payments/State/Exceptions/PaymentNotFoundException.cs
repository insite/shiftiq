using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Payments
{
    [Serializable]
    public class PaymentNotFoundException : Exception
    {
        public PaymentNotFoundException()
        {
        }

        public PaymentNotFoundException(Guid payment) : base($"There is no payment transaction started with this identifier ({payment}).")
        {
        }

        public PaymentNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PaymentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}