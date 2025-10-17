using System;
using System.Runtime.Serialization;

namespace InSite.Application.Records
{
    [Serializable]
    public class AchievementException : Exception
    {
        public AchievementException()
        {
        }

        public AchievementException(string message) : base(message)
        {
        }

        public AchievementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AchievementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
