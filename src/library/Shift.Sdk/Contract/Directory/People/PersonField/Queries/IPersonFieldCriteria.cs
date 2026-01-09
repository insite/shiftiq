using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonFieldCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        string FieldName { get; set; }
        string FieldValue { get; set; }

        int? FieldSequence { get; set; }
    }
}