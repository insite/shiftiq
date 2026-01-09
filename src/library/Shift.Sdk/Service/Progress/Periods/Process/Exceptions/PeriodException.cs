using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class PeriodException : Exception
    {
        public PeriodException()
        {
        }

        public PeriodException(string message) : base(message)
        {
        }

        public PeriodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PeriodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
