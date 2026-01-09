using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QGradebookFilter>
    {
        public override QGradebookFilter Filter
        {
            get
            {
                var filter = new QGradebookFilter
                {
                    GradebookTypes = new[] { GradebookType.Scores.ToString(), GradebookType.Standards.ToString(), GradebookType.ScoresAndStandards.ToString() },
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    GradebookTitle = GradebookTitle.Text,
                    GradebookCreatedSince = GradebookCreatedSince.Value,
                    GradebookCreatedBefore = GradebookCreatedBefore.Value,
                    GradebookPeriodIdentifier = GradebookPeriodIdentiffier.Value,
                    EventTitle = EventTitle.Text,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    AchievementIdentifier = AchievementIdentifier.ValueAsGuid,
                    StandardIdentifier = StandardIdentifier.Value,
                    EventInstructorIdentifier = Instructor.ValueAsGuid,
                    IsLocked = IsLocked.ValueAsBoolean,
                    CourseName = CourseName.Text,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                GradebookTitle.Text = value.GradebookTitle;
                GradebookCreatedSince.Value = value.GradebookCreatedSince;
                GradebookCreatedBefore.Value = value.GradebookCreatedBefore;
                GradebookPeriodIdentiffier.Value = value.GradebookPeriodIdentifier;
                EventTitle.Text = value.EventTitle;
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                AchievementIdentifier.ValueAsGuid = value.AchievementIdentifier;
                StandardIdentifier.Value = value.StandardIdentifier;
                IsLocked.ValueAsBoolean = value.IsLocked;
                Instructor.ValueAsGuid = Filter.EventInstructorIdentifier;
                CourseName.Text = value.CourseName;
            }
        }

        public override void Clear()
        {
            GradebookTitle.Text = null;
            GradebookCreatedSince.Value = null;
            GradebookCreatedBefore.Value = null;
            GradebookPeriodIdentiffier.Value = null;
            EventTitle.Text = null;
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            AchievementIdentifier.ValueAsGuid = null;
            StandardIdentifier.Value = null;
            Instructor.ClearSelection();
            IsLocked.ClearSelection();
            CourseName.Text = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            StandardIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            StandardIdentifier.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Framework };

            Instructor.LoadItems(
                ServiceLocator.EventSearch.GetAttendeeUsers(Organization.OrganizationIdentifier, "Instructor"),
                "UserIdentifier", "UserFullName"
            );
        }
    }
}