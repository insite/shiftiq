using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Registrations
{
    [Serializable]
    public class UnscheduledRegistrationException : Exception
    {
        public UnscheduledRegistrationException()
        {
        }

        public UnscheduledRegistrationException(string message) : base(message)
        {
        }

        public UnscheduledRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnscheduledRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}