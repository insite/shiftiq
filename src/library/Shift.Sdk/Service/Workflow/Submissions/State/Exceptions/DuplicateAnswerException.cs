using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Surveys.Sessions
{
    [Serializable]
    internal class DuplicateAnswerException : Exception
    {
        private Guid question;

        public DuplicateAnswerException()
        {
        }

        public DuplicateAnswerException(Guid question)
        {
            this.question = question;
        }

        public DuplicateAnswerException(string message) : base(message)
        {
        }

        public DuplicateAnswerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateAnswerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}