using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Surveys.Sessions
{
    [Serializable]
    internal class AnswerNotFoundException : Exception
    {
        private Guid question;

        public AnswerNotFoundException()
        {
        }

        public AnswerNotFoundException(Guid question)
        {
            this.question = question;
        }

        public AnswerNotFoundException(string message) : base(message)
        {
        }

        public AnswerNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AnswerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}