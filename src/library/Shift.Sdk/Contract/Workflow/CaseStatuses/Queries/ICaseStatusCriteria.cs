using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseStatusCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationIdentifier { get; set; }
        string CaseTypeContains { get; set; }
        string CaseTypeExact { get; set; }
        string StatusNameContains { get; set; }
        string StatusNameExact { get; set; }
        string StatusCategoryContains { get; set; }
        string StatusCategoryExact { get; set; }
        string ReportCategoryContains { get; set; }
        string ReportCategoryExact { get; set; }
        int? StatusSequenceSince { get; set; }
        int? StatusSequenceBefore { get; set; }
    }
}
