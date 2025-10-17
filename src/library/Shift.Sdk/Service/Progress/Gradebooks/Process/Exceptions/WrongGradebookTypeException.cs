using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    internal class WrongGradebookTypeException : Exception
    {
        public WrongGradebookTypeException()
        {
        }

        public WrongGradebookTypeException(string message) : base(message)
        {
        }

        public WrongGradebookTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongGradebookTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
