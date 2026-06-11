using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class DuplicateGradeItemCodeException : Exception
    {
        public DuplicateGradeItemCodeException()
        {
        }

        public DuplicateGradeItemCodeException(string message) : base(message)
        {
        }

        public DuplicateGradeItemCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateGradeItemCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}