using System;
using System.Runtime.Serialization;

namespace InSite.Persistence
{
    [Serializable]
    public class MissingPersonException : Exception
    {
        public MissingPersonException()
        {
        }

        public MissingPersonException(string user) 
            : base($"The user account {user} is not registered under any organization account.")
        {
        }

        public MissingPersonException(string user, string organization)
            : base($"The user account {user} is not registered under the organization account {organization}.")
        {
        }

        public MissingPersonException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        protected MissingPersonException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}