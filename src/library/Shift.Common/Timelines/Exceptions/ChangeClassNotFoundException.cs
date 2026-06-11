using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    internal class ChangeClassNotFoundException : Exception
    {
        public ChangeClassNotFoundException()
        {
        }

        public ChangeClassNotFoundException(string message) : base(message)
        {
        }

        public ChangeClassNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ChangeClassNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}