using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectBankQuestionAttachments : Query<IEnumerable<BankQuestionAttachmentModel>>, IBankQuestionAttachmentCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}