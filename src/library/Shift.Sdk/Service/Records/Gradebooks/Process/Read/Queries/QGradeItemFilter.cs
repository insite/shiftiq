using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QGradeItemFilter : Filter
    {
        public Guid? AchievementIdentifier { get; set; }
        public Guid? AssessmentFormIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public Guid[] GradebookIdentifiers { get; set; }
        public Guid? ParentGradeItemIdentifier { get; set; }
        public string ItemFormat { get; set; }
        public Guid[] GradeItemIdentifiers { get; set; }
    }
}
