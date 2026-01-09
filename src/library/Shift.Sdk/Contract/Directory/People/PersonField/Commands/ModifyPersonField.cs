using System;

namespace Shift.Contract
{
    public class ModifyPersonField
    {
        public Guid FieldIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }

        public int? FieldSequence { get; set; }
    }
}