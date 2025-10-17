using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class JournalSetupException : Exception
    {
        public JournalSetupException()
        {
        }

        public JournalSetupException(string message) : base(message)
        {
        }

        public JournalSetupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JournalSetupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
