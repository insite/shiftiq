using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchBankOptions : Query<IEnumerable<BankOptionMatch>>, IBankOptionCriteria
    {
        public Guid? BankIdentifier { get; set; }
        public Guid? CompetencyIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SetIdentifier { get; set; }

        public string OptionText { get; set; }
    }
}