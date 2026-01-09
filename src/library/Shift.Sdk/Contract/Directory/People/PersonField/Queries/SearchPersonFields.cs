using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchPersonFields : Query<IEnumerable<PersonFieldMatch>>, IPersonFieldCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }

        public int? FieldSequence { get; set; }
    }
}