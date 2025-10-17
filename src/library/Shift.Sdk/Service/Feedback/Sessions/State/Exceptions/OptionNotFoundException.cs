using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Surveys.Sessions
{
    [Serializable]
    internal class OptionNotFoundException : Exception
    {
        private Guid _option;

        public OptionNotFoundException()
        {
        }

        public OptionNotFoundException(Guid option)
        {
            _option = option;
        }

        public OptionNotFoundException(string message) : base(message)
        {
        }

        public OptionNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OptionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}