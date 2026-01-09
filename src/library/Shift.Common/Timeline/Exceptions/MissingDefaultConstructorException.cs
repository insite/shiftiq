using System;
using System.Runtime.Serialization;

namespace Shift.Common.Timeline.Exceptions
{
    [Serializable]
    public class MissingDefaultConstructorException : Exception
    {
        public MissingDefaultConstructorException(Type type)
            : base($"This class has no default constructor ({type.FullName}).")
        {
        }

        protected MissingDefaultConstructorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}