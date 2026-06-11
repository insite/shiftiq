using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    internal class DuplicateChangeTypeNamesException : Exception
    {
        public DuplicateChangeTypeNamesException()
        {
        }

        public DuplicateChangeTypeNamesException(string message) : base(message)
        {
        }

        public DuplicateChangeTypeNamesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateChangeTypeNamesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}