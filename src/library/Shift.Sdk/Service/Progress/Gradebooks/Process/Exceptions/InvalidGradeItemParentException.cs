using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    internal class InvalidGradeItemParentException : Exception
    {
        public InvalidGradeItemParentException()
        {
        }

        public InvalidGradeItemParentException(string message) : base(message)
        {
        }

        public InvalidGradeItemParentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidGradeItemParentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
