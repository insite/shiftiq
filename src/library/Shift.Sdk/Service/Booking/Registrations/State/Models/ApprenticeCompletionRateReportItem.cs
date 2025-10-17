using System;
using System.Collections.Generic;

namespace InSite.Domain.Registrations
{
    public class ApprenticeCompletionRateReportItem
    {
        public class ClassItem
        {
            public Guid GradebookIdentifier { get; set; }
            public string EventTitle { get; set; }
            public DateTimeOffset? EventScheduledStart { get; set; }
            public DateTimeOffset? EventScheduledEnd { get; set; }
            public decimal Percent { get; set; }
        }

        public Guid UserIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string AchievementDescription { get; set; }
        public int CredentialCount { get; set; }
        public bool IsCompleted { get; set; }

        public List<ClassItem> Classes { get; set; }
    }
}
