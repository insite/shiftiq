using System;

namespace InSite.Persistence
{
    public class TimestampModel
    {
        // public ShadowOperation Operation { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }

        private TimestampModel()
        {
        }
    }
}
