using System;
using System.Runtime.Serialization;

namespace Shift.Common
{
    [Serializable]
    public class WebServiceFailureException : Exception
    {
        public WebServiceFailureException()
        {
        }

        public WebServiceFailureException(string message) : base(message)
        {
        }

        public WebServiceFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebServiceFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
