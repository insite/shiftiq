using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    public class MissingAggregateIdentifierException : Exception
    {
        public MissingAggregateIdentifierException(Type aggregateType, Type changeType)
            : base($"The aggregate identifier is missing from both the aggregate instance ({aggregateType.FullName}) and the event instance ({changeType.FullName}).")
        {
        }

        protected MissingAggregateIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}