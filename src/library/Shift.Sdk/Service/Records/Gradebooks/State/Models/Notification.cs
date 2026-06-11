using System;

namespace InSite.Domain.Records
{
    [Serializable]
    public class Notification
    {
        public string Change { get; set; }
        public Guid? Message { get; set; }
    }
}