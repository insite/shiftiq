using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    public class UnorderedChangesException : Exception
    {
        public UnorderedChangesException(Guid aggregate)
            : base($"The changes for this aggregate are not in the expected order ({aggregate}).")
        {
        }

        protected UnorderedChangesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}