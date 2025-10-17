using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class CloseGradebookException : Exception
    {
        public CloseGradebookException()
        {
        }

        public CloseGradebookException(string message) : base(message)
        {
        }

        public CloseGradebookException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CloseGradebookException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
