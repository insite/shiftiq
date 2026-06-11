using System;
using System.Runtime.Serialization;

namespace InSite.Persistence
{
    [Serializable]
    internal class CommandNotFoundException : Exception
    {
        public CommandNotFoundException(string message) : base(message)
        {
        }

        protected CommandNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}