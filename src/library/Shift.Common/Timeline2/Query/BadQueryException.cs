using System;
using System.Runtime.Serialization;

namespace Shift.Common
{
    [Serializable]
    public class BadQueryException : Exception
    {
        public BadQueryException()
        {
        }

        public BadQueryException(string message) : base(message)
        {
        }

        public BadQueryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadQueryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}