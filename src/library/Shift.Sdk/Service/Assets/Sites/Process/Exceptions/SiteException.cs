using System;
using System.Runtime.Serialization;

namespace InSite.Application.Sites
{
    [Serializable]
    public class SiteException : Exception
    {
        public SiteException()
        {
        }

        public SiteException(string message) : base(message)
        {
        }

        public SiteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SiteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
