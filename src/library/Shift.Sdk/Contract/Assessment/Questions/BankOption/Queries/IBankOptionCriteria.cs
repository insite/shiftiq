using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankOptionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? BankIdentifier { get; set; }
        Guid? CompetencyIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? SetIdentifier { get; set; }

        string OptionText { get; set; }
    }
}