using System;

namespace Shift.Contract
{
    public class ModifyPersonField
    {
        public Guid FieldId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }

        public int? FieldSequence { get; set; }
    }
}