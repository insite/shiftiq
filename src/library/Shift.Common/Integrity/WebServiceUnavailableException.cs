using System;
using System.Runtime.Serialization;

namespace Shift.Common
{
    [Serializable]
    public class WebServiceUnavailableException : Exception
    {
        public WebServiceUnavailableException()
        {
        }

        public WebServiceUnavailableException(string message) : base(message)
        {
        }

        public WebServiceUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebServiceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
