using System;
using System.Runtime.Serialization;

namespace InSite.Persistence
{
    [Serializable]
    internal class SnapshotNotFoundException : Exception
    {
        public SnapshotNotFoundException()
        {
        }

        public SnapshotNotFoundException(string message) : base(message)
        {
        }

        public SnapshotNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SnapshotNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}