using System;
using System.Runtime.Serialization;

namespace InSite.Application.Contacts
{
    [Serializable]
    public class GroupException : Exception
    {
        public GroupException()
        {
        }

        public GroupException(string message) : base(message)
        {
        }

        public GroupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GroupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
