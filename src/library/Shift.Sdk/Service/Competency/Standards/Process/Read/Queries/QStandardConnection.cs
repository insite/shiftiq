using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardConnection
    {
        public Guid FromStandardIdentifier { get; set; }
        public Guid ToStandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string ConnectionType { get; set; }
    }
}
