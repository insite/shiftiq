using System;

namespace InSite.Persistence
{
    public class TPersonField
    {
        public Guid FieldIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }

        public int? FieldSequence { get; set; }

        public virtual User User { get; set; }
    }
}
