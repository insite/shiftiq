using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountBankQuestionAttachments : Query<int>, IBankQuestionAttachmentCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}