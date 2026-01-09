using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    public class AmbiguousChangeHandlerException : Exception
    {
        public AmbiguousChangeHandlerException(string name)
            : base($"You cannot define multiple handlers for the same change ({name}).")
        {
        }

        protected AmbiguousChangeHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}