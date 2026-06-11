using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Glossaries
{
    [Serializable]
    internal class DuplicateGlossaryTermException : Exception
    {
        private Guid aggregateIdentifier;
        private string term;

        public DuplicateGlossaryTermException()
        {
        }

        public DuplicateGlossaryTermException(string message) : base(message)
        {
        }

        public DuplicateGlossaryTermException(Guid aggregateIdentifier, string term)
        {
            this.aggregateIdentifier = aggregateIdentifier;
            this.term = term;
        }

        public DuplicateGlossaryTermException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateGlossaryTermException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}