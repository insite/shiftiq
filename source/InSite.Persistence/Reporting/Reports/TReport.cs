﻿using System;

namespace InSite.Persistence
{
    public class TReport
    {
        public Guid ReportIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? ActionIdentifier { get; set; }

        public string ReportType { get; set; }
        public string ReportTitle { get; set; }
        public string ReportData { get; set; }
        public string ReportDescription { get; set; }

        public DateTimeOffset? Created { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
