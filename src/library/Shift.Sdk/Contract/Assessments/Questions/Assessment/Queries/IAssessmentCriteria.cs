using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAssessmentCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }

        string FormPublicationStatus { get; set; }
        DateTimeOffset? FormFirstPublishedSince { get; set; }
        DateTimeOffset? FormFirstPublishedBefore { get; set; }
    }
}