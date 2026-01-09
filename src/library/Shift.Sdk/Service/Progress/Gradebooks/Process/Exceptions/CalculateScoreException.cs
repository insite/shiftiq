using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class CalculateScoreException : Exception
    {
        public CalculateScoreException()
        {
        }

        public CalculateScoreException(string message) : base(message)
        {
        }

        public CalculateScoreException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CalculateScoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
