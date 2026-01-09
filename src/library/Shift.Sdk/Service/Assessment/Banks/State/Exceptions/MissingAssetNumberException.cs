using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Banks
{
    [Serializable]
    internal class MissingAssetNumberException : Exception
    {
        public MissingAssetNumberException()
        {
        }

        public MissingAssetNumberException(string message) : base(message)
        {
        }

        public MissingAssetNumberException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingAssetNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}