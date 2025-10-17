using System;

namespace Shift.Sdk.UI
{
    public class PersonName
    {
        public Guid OrganizationId { get; set; }
        public Guid PersonId { get; set; }
        public Guid UserId { get; set; }

        public string First { get; set; }
        public string Last { get; set; }

        public string FullPerson { get; set; }
        public string FullUser { get; set; }
    }
}