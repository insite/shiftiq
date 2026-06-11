using System;

namespace InSite.Application.Logs.Read
{
    public class MostRecentChange
    {
        public Guid BankIdentifier { get; set; }
        public string BankName { get; set; }
        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
    }
}