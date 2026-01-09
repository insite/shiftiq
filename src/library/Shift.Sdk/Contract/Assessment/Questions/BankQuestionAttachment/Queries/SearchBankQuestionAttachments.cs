using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchBankQuestionAttachments : Query<IEnumerable<BankQuestionAttachmentMatch>>, IBankQuestionAttachmentCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}