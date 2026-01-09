using System;
using System.Web.UI;

using Humanizer;

using InSite.Application.Courses.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivitySetupTab : UserControl
    {
        public string ActivityNameInput => string.IsNullOrWhiteSpace(ActivityName.Text) ? "-" : ActivityName.Text;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RequirementConditionChanged.Click += (x, y) => OnRequirementConditionChanged(false);
        }

        private void OnRequirementConditionChanged(bool isInit)
        {
            var isMarkAsDone = RequirementConditionMarkAsDone.Checked;

            DoneSettings.Visible = isMarkAsDone;

            if (isMarkAsDone && !isInit)
            {
                DoneButtonText.Text = QActivity.DefaultDoneButtonText;
                DoneButtonInstructions.Text = QActivity.DefaultDoneButtonInstructions;
                DoneMarkedInstructions.Text = QActivity.DefaultDoneMarkedInstructions;
            }
        }

        public void BindModelToControls(QActivity activity, string validationGroup)
        {
            DoneButtonTextValidator.ValidationGroup = validationGroup;
            DoneButtonInstructionsValidator.ValidationGroup = validationGroup;
            DoneMarkedInstructionsValidator.ValidationGroup = validationGroup;

            var course = CourseSearch.BindActivityFirst(x => x.Module.Unit.CourseIdentifier, x => x.ActivityIdentifier == activity.ActivityIdentifier);

            PrerequisiteList.BindModelToControls(course, PrerequisiteObjectType.Activity, activity.ActivityIdentifier, activity.PrerequisiteDeterminer, validationGroup);

            SetRequirementType(activity);

            switch (activity.RequirementCondition.ToEnum(RequirementType.None))
            {
                case RequirementType.View:
                    RequirementConditionView.Checked = true; break;
                case RequirementType.MarkAsDone:
                    RequirementConditionMarkAsDone.Checked = true; break;
                case RequirementType.ScoreAtLeast:
                    RequirementConditionScoreAtLeast.Checked = true; break;
                case RequirementType.CompleteSurvey:
                    RequirementConditionCompleteSurvey.Checked = true; break;
                case RequirementType.CompleteScorm:
                    RequirementConditionCompleteScorm.Checked = true; break;
            }
            RequirementConditionScoreAtLeast.Visible = activity.ActivityType == "Assessment" || activity.ActivityType == "Quiz";
            RequirementConditionCompleteSurvey.Visible = activity.ActivityType == "Survey";
            RequirementConditionCompleteScorm.Visible = activity.ActivityType == "Link" && activity.ActivityUrlType == "SCORM";
            DoneButtonText.Text = activity.DoneButtonText.IfNullOrEmpty(QActivity.DefaultDoneButtonText);
            DoneButtonInstructions.Text = activity.DoneButtonInstructions.IfNullOrEmpty(QActivity.DefaultDoneButtonInstructions);
            DoneMarkedInstructions.Text = activity.DoneMarkedInstructions.IfNullOrEmpty(QActivity.DefaultDoneMarkedInstructions);

            OnRequirementConditionChanged(true);

            ActivityTypeField.Visible = CurrentSessionState.Identity.IsOperator;

            ActivityType.SelectedValue = activity.ActivityType.Titleize();
            ActivityName.Text = activity.ActivityName;
            ActivityCode.Text = activity.ActivityCode;
            ActivityMinutes.ValueAsInt = activity.ActivityMinutes;
            ActivityIsAdaptive.Checked = activity.ActivityIsAdaptive;
            AuthorName.Text = activity.ActivityAuthorName;
            AuthorDate.Value = activity.ActivityAuthorDate;

            ActivityIdentifier.Text = activity.ActivityIdentifier.ToString();
            ActivityAsset.Text = activity.ActivityAsset.ToString();
        }

        public void BindControlsToModel(QActivity activity)
        {
            PrerequisiteList.SaveChanges();
            activity.PrerequisiteDeterminer = PrerequisiteList.GetPrerequisiteDeterminer();

            if (RequirementConditionView.Checked)
                activity.RequirementCondition = RequirementType.View.GetName();
            else if (RequirementConditionMarkAsDone.Checked)
            {
                activity.RequirementCondition = RequirementType.MarkAsDone.GetName();
                activity.DoneButtonText = DoneButtonText.Text.NullIf(QActivity.DefaultDoneButtonText);
                activity.DoneButtonInstructions = DoneButtonInstructions.Text.NullIf(QActivity.DefaultDoneButtonInstructions);
                activity.DoneMarkedInstructions = DoneMarkedInstructions.Text.NullIf(QActivity.DefaultDoneMarkedInstructions);
            }
            else if (RequirementConditionScoreAtLeast.Checked)
                activity.RequirementCondition = RequirementType.ScoreAtLeast.GetName();
            else if (RequirementConditionCompleteSurvey.Checked)
                activity.RequirementCondition = RequirementType.CompleteSurvey.GetName();
            else if (RequirementConditionCompleteScorm.Checked)
                activity.RequirementCondition = RequirementType.CompleteScorm.GetName();

            activity.ActivityType = ActivityType.SelectedValue;
            activity.ActivityName = ActivityName.Text;
            activity.ActivityCode = ActivityCode.Text;
            activity.ActivityMinutes = ActivityMinutes.ValueAsInt;
            activity.ActivityIsAdaptive = ActivityIsAdaptive.Checked;
            activity.ActivityAuthorName = AuthorName.Text;
            activity.ActivityAuthorDate = AuthorDate.Value;

            if (string.IsNullOrWhiteSpace(activity.ActivityName))
                activity.ActivityName = "-";
        }


        private void SetRequirementType(QActivity activity)
        {
            if (activity.RequirementCondition != null)
                return;

            switch (activity.ActivityType)
            {
                case "Assessment":
                case "Quiz":
                    activity.RequirementCondition = RequirementType.ScoreAtLeast.ToString();
                    break;

                case "Survey":
                    activity.RequirementCondition = RequirementType.CompleteSurvey.ToString();
                    break;

                default:
                    activity.RequirementCondition = RequirementType.View.ToString();
                    break;
            }
        }
    }
}