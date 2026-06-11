using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Banks
{
    [Serializable]
    internal class DuplicateQuestionException : Exception
    {
        public DuplicateQuestionException()
        {
        }

        public DuplicateQuestionException(string message) : base(message)
        {
        }

        public DuplicateQuestionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateQuestionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}