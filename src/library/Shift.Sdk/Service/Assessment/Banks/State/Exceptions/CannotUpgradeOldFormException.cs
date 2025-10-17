using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Banks
{
    [Serializable]
    public class CannotUpgradeOldFormException : Exception
    {
        public CannotUpgradeOldFormException()
        {
        }

        public CannotUpgradeOldFormException(string message) : base(message)
        {
        }

        public CannotUpgradeOldFormException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotUpgradeOldFormException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}