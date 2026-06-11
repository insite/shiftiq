using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    internal class DuplicateGradeItemKeyException : Exception
    {
        private Guid key;

        public DuplicateGradeItemKeyException()
        {
        }

        public DuplicateGradeItemKeyException(Guid key)
        {
            this.key = key;
        }

        public DuplicateGradeItemKeyException(string message) : base(message)
        {
        }

        public DuplicateGradeItemKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateGradeItemKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}