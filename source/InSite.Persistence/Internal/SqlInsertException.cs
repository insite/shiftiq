using System;
using System.Runtime.Serialization;

namespace InSite.Persistence
{
    [Serializable]
    internal class SqlInsertException : Exception
    {
        public SqlInsertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SqlInsertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}