using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Messages
{
    [Serializable]
    public class MessageNotFoundException : Exception
    {
        public MessageNotFoundException()
        {
        }

        public MessageNotFoundException(string message) : base(message)
        {
        }

        public MessageNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MessageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
