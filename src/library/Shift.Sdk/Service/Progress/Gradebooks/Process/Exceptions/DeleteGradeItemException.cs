using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    internal class DeleteGradeItemException : Exception
    {
        public DeleteGradeItemException()
        {
        }

        public DeleteGradeItemException(string message) : base(message)
        {
        }

        public DeleteGradeItemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeleteGradeItemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
