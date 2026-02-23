using System;

using Shift.Common;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QSurveyFormFilter : Filter
    {
        public string SortByColumn = nameof(QSurveyForm.SurveyFormName);

        public Guid OrganizationIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }
        public Guid? Identifier { get; set; }
        public Guid[] Identifiers { get; set; }
        public string[] CurrentStatus { get; set; }
        public bool? EnableUserConfidentiality { get; set; }
        public bool? IsLocked { get; set; }
        public DateTimeOffset? LastModifiedSince { get; set; }
        public DateTimeOffset? LastModifiedBefore { get; set; }
        public Guid? MessageIdentifier { get; set; }

        public QSurveyFormFilter Clone()
        {
            return (QSurveyFormFilter)MemberwiseClone();
        }
    }
}