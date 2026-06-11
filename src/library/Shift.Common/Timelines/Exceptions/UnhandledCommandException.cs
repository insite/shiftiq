using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    public class UnhandledCommandException : Exception
    {
        public UnhandledCommandException(string name)
            : base($"There is no handler registered for this command ({name}). One handler (and only one handler) must be registered.")
        {
        }

        public UnhandledCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnhandledCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}