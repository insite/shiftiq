using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Records.Scores.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QProgressFilter>
    {
        public override QProgressFilter Filter
        {
            get
            {
                var filter = new QProgressFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    GradebookTitle = GradebookTitle.Text,
                    GradebookCreatedSince = GradebookCreatedSince.Value,
                    GradebookCreatedBefore = GradebookCreatedBefore.Value,
                    ItemTypes = ItemTypes.ValuesArray,
                    ItemFormat = ItemFormat.Value,
                    ItemName = ItemName.Text,
                    ScorePercentFrom = ScorePercentFrom.ValueAsDecimal / 100m,
                    ScorePercentThru = ScorePercentThru.ValueAsDecimal / 100m,
                    ScoreText = ScoreText.Text,
                    ProgressStatus = ProgressStatus.Value,
                    ScoreComment = ScoreComment.Text,
                    EventTitle = EventTitle.Text,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    AchievementTitle = AchievementTitle.Text,
                    StudentName = LearnerName.Text,
                    EventInstructorIdentifier = EventInstructorIdentifier.ValueAsGuid,
                    StudentEmployerGroupIdentifier = LearnerEmployerGroupIdentifier.Value,
                    StudentEmployerGroupStatusIdentifier = LearnerEmployerGroupStatusId.ValueAsGuid,
                    UserPeriodIdentifier = UserPeriodSelector.Value,
                    GradedSince = GradedSince.Value,
                    GradedBefore = GradedBefore.Value,
                    IsScoreIgnored = IsScoreIgnored.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                GradebookTitle.Text = value.GradebookTitle;
                GradebookCreatedSince.Value = value.GradebookCreatedSince;
                GradebookCreatedBefore.Value = value.GradebookCreatedBefore;
                ItemTypes.Values = value.ItemTypes;
                ItemFormat.Value = value.ItemFormat;
                ItemName.Text = value.ItemName;
                ScorePercentFrom.ValueAsDecimal = value.ScorePercentFrom * 100m;
                ScorePercentThru.ValueAsDecimal = value.ScorePercentThru * 100m;
                ScoreText.Text = value.ScoreText;
                ProgressStatus.Value = value.ProgressStatus;
                ScoreComment.Text = value.ScoreComment;
                EventTitle.Text = value.EventTitle;
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                AchievementTitle.Text = value.AchievementTitle;
                LearnerName.Text = value.StudentName;
                EventInstructorIdentifier.ValueAsGuid = value.EventInstructorIdentifier;
                LearnerEmployerGroupIdentifier.Value = value.StudentEmployerGroupIdentifier;
                LearnerEmployerGroupStatusId.ValueAsGuid = value.StudentEmployerGroupStatusIdentifier;
                UserPeriodSelector.Value = value.UserPeriodIdentifier;
                GradedSince.Value = value.GradedSince;
                GradedBefore.Value = value.GradedBefore;
                IsScoreIgnored.ValueAsBoolean = value.IsScoreIgnored;
            }
        }

        public override void Clear()
        {
            GradebookTitle.Text = null;
            GradebookCreatedSince.Value = null;
            GradebookCreatedBefore.Value = null;
            ItemTypes.ClearSelection();
            ItemFormat.ClearSelection();
            ItemName.Text = null;
            ScorePercentFrom.ValueAsDecimal = null;
            ScorePercentThru.ValueAsDecimal = null;
            ScoreText.Text = null;
            ProgressStatus.Value = null;
            ScoreComment.Text = null;
            EventTitle.Text = null;
            EventScheduledSince.Value = null;
            EventScheduledSince.Value = null;
            AchievementTitle.Text = null;
            LearnerName.Text = null;
            EventInstructorIdentifier.ClearSelection();
            LearnerEmployerGroupIdentifier.Value = null;
            LearnerEmployerGroupStatusId.ClearSelection();
            UserPeriodSelector.Value = null;
            GradedSince.Value = null;
            GradedBefore.Value = null;
            IsScoreIgnored.ValueAsBoolean = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LearnerEmployerGroupStatusId.ListFilter.OrganizationIdentifier = Organization.Identifier;
            LearnerEmployerGroupStatusId.ListFilter.CollectionName = CollectionName.Contacts_Groups_Status_Name;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LearnerEmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

            EventInstructorIdentifier.LoadItems(
                ServiceLocator.EventSearch.GetAttendeeUsers(Organization.OrganizationIdentifier, "Instructor"),
                "UserIdentifier", "UserFullName"
            );
        }
    }
}