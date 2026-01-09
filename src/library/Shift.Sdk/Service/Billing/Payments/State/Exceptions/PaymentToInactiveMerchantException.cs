using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Payments
{
    [Serializable]
    internal class PaymentToInactiveMerchantException : Exception
    {
        public PaymentToInactiveMerchantException()
        {
        }

        public PaymentToInactiveMerchantException(string message) : base(message)
        {
        }

        public PaymentToInactiveMerchantException(Guid organization, Guid payment)
            : base($"This payment ({payment}) cannot be submitted to an inactive merchant account ({organization}).")
        {
        }

        public PaymentToInactiveMerchantException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PaymentToInactiveMerchantException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}