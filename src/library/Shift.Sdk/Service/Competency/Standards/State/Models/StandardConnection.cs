using System;

namespace InSite.Domain.Standards
{
    public class StandardConnection
    {
        public Guid ToStandardId { get; set; }
        public string ConnectionType { get; set; }

        public StandardConnection()
        {
        }

        public StandardConnection(Guid toStandardId, string connectionType)
        {
            ToStandardId = toStandardId;
            ConnectionType = connectionType;
        }

        public StandardConnection Clone()
        {
            return (StandardConnection)MemberwiseClone();
        }
    }
}
