using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Records
{
    [Serializable]
    internal class UnsupportedUnitException : Exception
    {
        public UnsupportedUnitException()
        {
        }

        public UnsupportedUnitException(string message) : base(message)
        {
        }

        public UnsupportedUnitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedUnitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}