using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCaseStatuses : Query<IEnumerable<CaseStatusMatch>>, ICaseStatusCriteria
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
