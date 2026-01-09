using System;
using System.Runtime.Serialization;

namespace Shift.Common
{
    [Serializable]
    internal class DuplicatePivotValueException : Exception
    {
        public DuplicatePivotValueException()
        {
        }

        public DuplicatePivotValueException(string message) : base(message)
        {
        }

        public DuplicatePivotValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicatePivotValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}