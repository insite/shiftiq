using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    public class UnhandledChangeException : Exception
    {
        public UnhandledChangeException(string name)
            : base($"You must register at least one handler for this change ({name}).")
        {
        }

        protected UnhandledChangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}