using System;
using System.Runtime.Serialization;

namespace Shift.Common
{
    [Serializable]
    internal class InvalidPivotKeyException : Exception
    {
        public InvalidPivotKeyException()
        {
        }

        public InvalidPivotKeyException(string message) : base(message)
        {
        }

        public InvalidPivotKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPivotKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}