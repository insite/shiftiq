using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Snapshots
{
    [Serializable]
    internal class SerializationFailedException : Exception
    {
        public SerializationFailedException()
        {
        }

        public SerializationFailedException(string message) : base(message)
        {
        }

        public SerializationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SerializationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}