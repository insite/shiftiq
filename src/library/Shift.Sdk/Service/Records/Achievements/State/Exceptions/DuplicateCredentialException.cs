using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Records
{
    [Serializable]
    public class DuplicateCredentialException : Exception
    {
        private Guid user;

        public DuplicateCredentialException()
        {
        }

        public DuplicateCredentialException(Guid user)
        {
            this.user = user;
        }

        public DuplicateCredentialException(string message) : base(message)
        {
        }

        public DuplicateCredentialException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateCredentialException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}