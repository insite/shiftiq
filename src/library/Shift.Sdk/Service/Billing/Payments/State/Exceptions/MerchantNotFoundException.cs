using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Payments
{
    [Serializable]
    internal class MerchantNotFoundException : Exception
    {
        public MerchantNotFoundException()
        {
        }

        public MerchantNotFoundException(Guid organization) : base($"There is no merchant registered for this organization ({organization}).")
        {
        }

        public MerchantNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MerchantNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}