using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class ProgressNotFoundException : Exception
    {
        public ProgressNotFoundException()
        {
        }

        public ProgressNotFoundException(string message) : base(message)
        {
        }

        public ProgressNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProgressNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}