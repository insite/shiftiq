using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class LockedGradebookException : Exception
    {
        public LockedGradebookException()
        {
        }

        public LockedGradebookException(string message) : base(message)
        {
        }

        public LockedGradebookException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LockedGradebookException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
