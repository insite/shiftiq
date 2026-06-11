using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Records
{
    [Serializable]
    internal class ChangeDisabledAchievementException : Exception
    {
        public ChangeDisabledAchievementException()
        {
        }

        public ChangeDisabledAchievementException(string message) : base(message)
        {
        }

        public ChangeDisabledAchievementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ChangeDisabledAchievementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}