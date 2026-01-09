using System;
using System.Runtime.Serialization;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class ToolkitSettings
    {
        public AccountSettings Accounts { get; set; }
        public AchievementSettings Achievements { get; set; }
        public AssessmentSettings Assessments { get; set; }
        public ContactSettings Contacts { get; set; }
        public EventSettings Events { get; set; }
        public GradebookSettings Gradebooks { get; set; }
        public IssueSettings Issues { get; set; }
        public NCSHASettings NCSHA { get; set; }
        public PlatformSettings Platform { get; set; }
        public SalesSettings Sales { get; set; }
        public SiteSettings Sites { get; set; }
        public StandardSettings Standards { get; set; }
        public SurveySettings Surveys { get; set; }
        public PortalSettings Portal { get; set; }

        public ToolkitSettings()
        {
            Accounts = new AccountSettings();
            Achievements = new AchievementSettings();
            Assessments = new AssessmentSettings();
            Contacts = new ContactSettings();
            Events = new EventSettings();
            Gradebooks = new GradebookSettings();
            Issues = new IssueSettings();
            NCSHA = new NCSHASettings();
            Platform = new PlatformSettings();
            Sales = new SalesSettings();
            Sites = new SiteSettings();
            Standards = new StandardSettings();
            Surveys = new SurveySettings();
            Portal = new PortalSettings();
        }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            if (Portal == null)
                Portal = new PortalSettings();
        }
    }
}
