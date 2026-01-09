using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCaseStatuses : Query<IEnumerable<CaseStatusModel>>, ICaseStatusCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string CaseTypeContains { get; set; }
        public string CaseTypeExact { get; set; }
        public string StatusNameContains { get; set; }
        public string StatusNameExact { get; set; }
        public string StatusCategoryContains { get; set; }
        public string StatusCategoryExact { get; set; }
        public string ReportCategoryContains { get; set; }
        public string ReportCategoryExact { get; set; }
        public int? StatusSequenceSince { get; set; }
        public int? StatusSequenceBefore { get; set; }
    }
}
