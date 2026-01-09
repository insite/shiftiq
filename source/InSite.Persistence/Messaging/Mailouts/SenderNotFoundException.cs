using System;
using System.Runtime.Serialization;

namespace InSite.Persistence
{
    [Serializable]
    internal class SenderNotFoundException : Exception
    {
        public SenderNotFoundException()
        {
        }

        public SenderNotFoundException(Guid senderId)
            : this($"Sender not found: {senderId}")
        {
        }

        public SenderNotFoundException(string message) : base(message)
        {
        }

        public SenderNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SenderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}