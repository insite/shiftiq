using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Records.Outcomes.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QGradebookCompetencyValidationFilter>
    {
        public override QGradebookCompetencyValidationFilter Filter
        {
            get
            {
                var filter = new QGradebookCompetencyValidationFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    GradebookTitle = GradebookTitle.Text,
                    GradebookCreatedSince = GradebookCreatedSince.Value,
                    GradebookCreatedBefore = GradebookCreatedBefore.Value,
                    PointsFrom = PointsFrom.ValueAsDecimal,
                    PointsThru = PointsThru.ValueAsDecimal,
                    EventTitle = EventTitle.Text,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    AchievementTitle = AchievementTitle.Text,
                    StudentName = StudentName.Text,
                    EventInstructorIdentifier = EventInstructorIdentifier.ValueAsGuid,
                    StudentEmployerGroupIdentifier = StudentEmployerGroupIdentifier.Value,
                    CompetencyIdentifier = CompetencySelector.Value,
                    GradebookPeriodIdentifier = GradebookPeriodIdentifier.Value,
                    UserPeriodIdentifier = UserPeriodIdentifier.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                GradebookTitle.Text = value.GradebookTitle;
                GradebookCreatedSince.Value = value.GradebookCreatedSince;
                GradebookCreatedBefore.Value = value.GradebookCreatedBefore;

                PointsFrom.ValueAsDecimal = value.PointsFrom;
                PointsThru.ValueAsDecimal = value.PointsThru;
                EventTitle.Text = value.EventTitle;
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                AchievementTitle.Text = value.AchievementTitle;
                StudentName.Text = value.StudentName;

                EventInstructorIdentifier.ValueAsGuid = value.EventInstructorIdentifier;
                StudentEmployerGroupIdentifier.Value = value.StudentEmployerGroupIdentifier;
                CompetencySelector.Value = value.CompetencyIdentifier;
                GradebookPeriodIdentifier.Value = value.GradebookPeriodIdentifier;
                UserPeriodIdentifier.Value = value.UserPeriodIdentifier;
            }
        }

        public override void Clear()
        {
            GradebookTitle.Text = null;
            GradebookCreatedSince.Value = null;
            GradebookCreatedBefore.Value = null;
            PointsFrom.ValueAsDecimal = null;
            PointsThru.ValueAsDecimal = null;
            EventTitle.Text = null;
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            AchievementTitle.Text = null;
            StudentName.Text = null;
            EventInstructorIdentifier.ValueAsGuid = null;
            StudentEmployerGroupIdentifier.Value = null;
            CompetencySelector.Value = null;
            GradebookPeriodIdentifier.Value = null;
            UserPeriodIdentifier.Value = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                StudentEmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

                EventInstructorIdentifier.LoadItems(
                    ServiceLocator.EventSearch.GetAttendeeUsers(Organization.OrganizationIdentifier, "Instructor"),
                    "UserIdentifier", "UserFullName"
                );

                CompetencySelector.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
                CompetencySelector.Filter.StandardTypes = new string[] { Shift.Constant.StandardType.Competency };
                CompetencySelector.Value = null;
            }
        }
    }
}