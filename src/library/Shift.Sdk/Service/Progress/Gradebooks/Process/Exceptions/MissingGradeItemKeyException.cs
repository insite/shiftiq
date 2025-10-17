using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    internal class MissingGradeItemKeyException : Exception
    {
        private int value;

        public MissingGradeItemKeyException()
        {
        }

        public MissingGradeItemKeyException(int value)
        {
            this.value = value;
        }

        public MissingGradeItemKeyException(string message) : base(message)
        {
        }

        public MissingGradeItemKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingGradeItemKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}