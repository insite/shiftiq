using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankQuestionAttachmentCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
    }
}