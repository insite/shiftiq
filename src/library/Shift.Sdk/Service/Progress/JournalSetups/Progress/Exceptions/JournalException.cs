using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class JournalException : Exception
    {
        public JournalException()
        {
        }

        public JournalException(string message) : base(message)
        {
        }

        public JournalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JournalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
