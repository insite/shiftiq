using System;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyWorkflowConfiguration
    {
        public Guid? AdministratorUserIdentifier { get; set; }
        public Guid? OwnerUserIdentifier { get; set; }
        public Guid? IssueStatusIdentifier { get; set; }

        public string IssueType { get; set; }
    }
}
