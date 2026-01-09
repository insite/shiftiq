using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountPersonFields : Query<int>, IPersonFieldCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }

        public int? FieldSequence { get; set; }
    }
}