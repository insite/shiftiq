using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    internal class InvalidGradebookException : Exception
    {
        public InvalidGradebookException()
        {
        }

        public InvalidGradebookException(string message) : base(message)
        {
        }

        public InvalidGradebookException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidGradebookException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}