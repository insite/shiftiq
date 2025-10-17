using System;
using System.Runtime.Serialization;

namespace Shift.Common
{
    [Serializable]
    public class BlockContentControlNotFound : Exception
    {
        public BlockContentControlNotFound() { }

        public BlockContentControlNotFound(string message) : base(message) { }

        public BlockContentControlNotFound(string message, Exception innerException) : base(message, innerException) { }

        protected BlockContentControlNotFound(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class PageContentControlNotFound : Exception
    {
        public PageContentControlNotFound() { }

        public PageContentControlNotFound(string message) : base(message) { }

        public PageContentControlNotFound(string message, Exception innerException) : base(message, innerException) { }

        protected PageContentControlNotFound(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}