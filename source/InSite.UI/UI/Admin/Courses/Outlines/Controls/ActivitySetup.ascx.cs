using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Courses.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.CourseObjects;
using InSite.Domain.Courses;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivitySetup : BaseUserControl
    {
        public Guid CourseIdentifier
        {
            get => (Guid)ViewState[nameof(CourseIdentifier)];
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        public Guid ActivityIdentifier
        {
            get => (Guid)ViewState[nameof(ActivityIdentifier)];
            set => ViewState[nameof(ActivityIdentifier)] = value;
        }

        public Guid? GradebookIdentifier
        {
            get => (Guid?)ViewState[nameof(GradebookIdentifier)];
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GradeItemIdentifier.AutoPostBack = true;
            GradeItemIdentifier.ValueChanged += (x, y) => { BindModelToControlsForGradebook(GradeItemIdentifier.ValueAsGuid, false); };
            GradebookCreateButton.Click += (x, y) => { BindModelToControlsForGradebook(null, true); };

            AchievementIdentifier.AutoPostBack = true;
            AchievementIdentifier.ValueChanged += (x, y) => { BindModelToControlsForAchievement(AchievementIdentifier.Value, false); };
            AchievementCreateButton.Click += (x, y) => { BindModelToControlsForAchievement(null, true); };

            ActivitySaveButton.Click += ActivitySaveButton_Click;
            ActivityCancelButton.NavigateUrl = Request.RawUrl;

            GradeItemCodeValidator.ServerValidate += GradeItemCodeValidator_ServerValidate;
            UniqueGradeItem.ServerValidate += UniqueGradeItem_ServerValidate;
        }

        private void ActivitySaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                var activity = ServiceLocator.CourseSearch.GetActivity(ActivityIdentifier);
                var course = CourseSearch.SelectCourse(CourseIdentifier);

                ActivitySetupTab.BindControlsToModel(activity);

                SaveChangesToGradeItem(course, activity);
                SaveChangesToAchievement(course, activity);
                SaveChangesToCompetencies(course, activity);

                Course2Store.UpdateActivity(CourseIdentifier, activity, null);

                HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "panel", "activity", "tab", "activity"));
            }
            catch (Application.Records.AchievementException ex)
            {
                ActivitySetupAlert.AddMessage(AlertType.Error, $"Modifications are not permitted while the achievement is locked. Please unlock it before making any changes. {ex.Message}");
            }
            catch (Application.Records.DuplicateGradeItemCodeException ex)
            {
                ActivitySetupAlert.AddMessage(AlertType.Error, $"Another grade item in the course gradebook already uses this code: {ex.Message}");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                if (ex.Message.StartsWith("Cannot insert duplicate key row in object 'courses.QActivity' with unique index 'IX_TActivity_GradeItem'"))
                    ActivitySetupAlert.AddMessage(AlertType.Error, $"This grade item is already assigned to another activity in this course.");
                ActivitySetupAlert.AddMessage(AlertType.Error, $"Another grade item in the course gradebook already uses this code: {ex.Message}");
            }
        }

        private void GradeItemCodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var gradeItem = GradeItemIdentifier.ValueAsGuid.HasValue
                ? ServiceLocator.RecordSearch.GetGradeItem(GradeItemIdentifier.ValueAsGuid.Value)
                : null;

            if (gradeItem == null)
                return;

            args.IsValid = ServiceLocator.RecordSearch.IsGradeItemCodeUniqe(gradeItem.GradebookIdentifier, gradeItem.GradeItemIdentifier, GradeItemCode.Text);

            ((BaseValidator)source).ErrorMessage = $"Another grade item in the course gradebook already uses this code {GradeItemCode.Text}";
        }

        private void UniqueGradeItem_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (GradeItemIdentifier.ValueAsGuid == null)
                return;

            var activity = ServiceLocator.CourseSearch.GetActivityByGradeItem(GradeItemIdentifier.ValueAsGuid.Value);

            args.IsValid = activity == null || activity.ActivityIdentifier == ActivityIdentifier;
        }

        private void SaveChangesToGradeItem(QCourse course, QActivity activity)
        {
            if (!course.GradebookIdentifier.HasValue)
                return;

            var isNew = !GradeItemIdentifierField.Visible;
            var format = GradeItemFormat.SelectedValue.ToEnum<GradeItemFormat>();

            if (isNew)
            {
                activity.GradeItemIdentifier = UniqueIdentifier.Create();
                ServiceLocator.SendCommand(new Application.Gradebooks.Write.AddGradeItem(course.GradebookIdentifier.Value, activity.GradeItemIdentifier.Value,
                    GradeItemCode.Text, GradeItemName.Text, null, true,
                    format, GradeItemType.Score, GradeItemWeighting.Equally, null,
                    null));

                return;
            }

            var gradeItem = GradeItemIdentifier.ValueAsGuid.HasValue
                ? ServiceLocator.RecordSearch.GetGradeItem(GradeItemIdentifier.ValueAsGuid.Value)
                : null;

            if (gradeItem == null)
            {
                activity.GradeItemIdentifier = null;
                return;
            }

            activity.GradeItemIdentifier = gradeItem.GradeItemIdentifier;

            var type = gradeItem.GradeItemType.ToEnum<GradeItemType>();
            var weighting = gradeItem.GradeItemWeighting.ToEnum<GradeItemWeighting>();

            if (GradeItemName.Text != gradeItem.GradeItemName
                || GradeItemCode.Text != gradeItem.GradeItemCode
                || GradeItemFormat.SelectedValue != gradeItem.GradeItemType)
                ServiceLocator.SendCommand(new Application.Gradebooks.Write.ChangeGradeItem(gradeItem.GradebookIdentifier, gradeItem.GradeItemIdentifier,
                    GradeItemCode.Text, GradeItemName.Text, gradeItem.GradeItemShortName, gradeItem.GradeItemIsReported,
                    format, type, weighting,
                    gradeItem.ParentGradeItemIdentifier));

            var gradeItemPassPercent = GradeItemPassPercent.ValueAsDecimal / 100m;
            if (gradeItem.GradeItemPassPercent != gradeItemPassPercent)
                ServiceLocator.SendCommand(new Application.Gradebooks.Write.ChangeGradeItemPassPercent(
                    gradeItem.GradebookIdentifier, gradeItem.GradeItemIdentifier, gradeItemPassPercent));
        }

        private void SaveChangesToAchievement(QCourse course, QActivity activity)
        {
            if (!course.GradebookIdentifier.HasValue || !activity.GradeItemIdentifier.HasValue)
                return;

            var isNew = !AchievementIdentifierField.Visible;

            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(course.GradebookIdentifier.Value);
            var gradeitem = gradebook.FindItem(activity.GradeItemIdentifier.Value);

            if (gradebook == null || gradeitem == null)
                return;

            if (isNew)
                CreateAchievement(gradebook.Identifier, gradeitem.Identifier);
            else
                ModifyAchievement(gradebook.Identifier, gradeitem);
        }

        private void SaveChangesToCompetencies(QCourse course, QActivity activity)
        {
            if (!course.FrameworkStandardIdentifier.HasValue)
                return;

            if (!CompetenciesTab.IsSelected)
                return;

            var selectedCompetencies = CompetenciesSelector.SelectedCompetencies;

            {
                var framework = InSite.Admin.Courses.Activities.Controls.CompetenciesSelector.LoadCompetencies(activity.ActivityIdentifier, course.FrameworkStandardIdentifier.Value)
                    .FirstOrDefault(x => x.StandardIdentifier == course.FrameworkStandardIdentifier.Value);

                if (framework != null)
                {
                    var competencies = framework.EnumerateChildrenFlatten().ToList();

                    var identifiers = competencies
                        .Where(x => x.StandardType == StandardType.Competency)
                        .Select(x => x.StandardIdentifier)
                        .ToArray();

                    Course2Store.DeleteActivityCompetencies(CourseIdentifier, activity.ActivityIdentifier, identifiers);
                }
            }
            {
                var competencies = new List<ActivityCompetency>();

                foreach (var id in selectedCompetencies)
                {
                    competencies.Add(new ActivityCompetency
                    {
                        CompetencyStandardIdentifier = id,
                        CompetencyCode = StandardSearch.GetPathCode(id)
                    });
                }

                if (competencies.Count > 0)
                    Course2Store.Insert(CourseIdentifier, ActivityIdentifier, competencies.ToArray());
            }
        }

        private void ModifyAchievement(Guid gradebook, Domain.Records.GradeItem gradeitem)
        {
            if (AchievementIdentifier.HasValue)
            {
                var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value.Value);

                if (achievement == null || !achievement.AchievementIsEnabled)
                    throw new Application.Records.AchievementException(achievement.AchievementTitle);

                if (achievement.AchievementLabel != AchievementLabel.Text || achievement.AchievementTitle != AchievementName.Text)
                    ServiceLocator.SendCommand(new Application.Achievements.Write.DescribeAchievement(
                        achievement.AchievementIdentifier, AchievementLabel.Text, AchievementName.Text, achievement.AchievementDescription, achievement.AchievementAllowSelfDeclared));

                if (achievement.CertificateLayoutCode != AchievementLayout.Value)
                    ServiceLocator.SendCommand(new Application.Achievements.Write.ChangeCertificateLayout(
                        achievement.AchievementIdentifier, AchievementLayout.Value));
            }

            if (gradeitem.Achievement == null && AchievementIdentifier.Value == null)
                return;

            if (gradeitem.Achievement == null && AchievementIdentifier.Value != null)
                AssignAchievementToGradeItem(AchievementIdentifier.Value.Value, gradebook, gradeitem.Identifier);

            else if (gradeitem.Achievement != null && AchievementIdentifier.Value == null)
                ServiceLocator.SendCommand(new Application.Gradebooks.Write.ChangeGradeItemAchievement(gradebook, gradeitem.Identifier, null));

            else
            {
                gradeitem.Achievement.Achievement = AchievementIdentifier.Value.Value;
                ServiceLocator.SendCommand(new Application.Gradebooks.Write.ChangeGradeItemAchievement(gradebook, gradeitem.Identifier, gradeitem.Achievement));
            }
        }

        private void CreateAchievement(Guid gradebook, Guid gradeitem)
        {
            if (string.IsNullOrWhiteSpace(AchievementLabel.Text))
                return;

            var id = UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new Application.Achievements.Write.CreateAchievement(
                id, Organization.OrganizationIdentifier, AchievementLabel.Text, AchievementName.Text, null, null, null));

            AssignAchievementToGradeItem(id, gradebook, gradeitem);
        }

        private void AssignAchievementToGradeItem(Guid achievement, Guid gradebook, Guid gradeitem)
        {
            var itemAchievement = new Domain.Records.GradeItemAchievement
            {
                Achievement = achievement,
                WhenChange = TriggerCauseChange.Changed,
                WhenGrade = TriggerCauseGrade.Pass,
                ThenCommand = TriggerEffectCommand.Grant,
                ElseCommand = TriggerEffectCommand.Void
            };

            ServiceLocator.SendCommand(new Application.Gradebooks.Write.ChangeGradeItemAchievement(
                gradebook, gradeitem, itemAchievement));
        }

        public void BindModelToControls(Course course, QActivity activity)
        {
            CourseIdentifier = course.Identifier;
            ActivityIdentifier = activity.ActivityIdentifier;
            GradebookIdentifier = course.Gradebook?.Identifier;

            ActivitySetupTab.BindModelToControls(activity, ActivitySaveButton.ValidationGroup);

            GradeItemIdentifier.GradebookIdentifier = course.Gradebook?.Identifier ?? Guid.Empty;
            GradeItemIdentifier.RefreshData();

            BindModelToControlsForGradebook(activity.GradeItemIdentifier, false);

            CompetenciesSelector.BindModelToControls(CourseIdentifier, ActivityIdentifier, course.Framework);
            PrivacySettingsGroups.LoadData(ActivityIdentifier, "Activity");
        }

        public void BindModelToControlsForGradebook(Guid? gradeItemId, bool forceNew)
        {
            GradeItemFields.Visible = false;
            AchievementPanel.Visible = false;

            if (!GradebookIdentifier.HasValue)
                return;

            if (forceNew)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier.Value);
                var activity = CourseSearch.SelectActivity(ActivityIdentifier, x => x.Module.Unit);

                if (gradebook == null || activity == null)
                    return;

                var isAssessment = activity.ActivityType == "Assessment";
                var format = isAssessment ? Shift.Constant.GradeItemFormat.Percent : Shift.Constant.GradeItemFormat.Boolean;
                var code = string.Empty;
                var percent = (decimal?)null;

                if (isAssessment && activity.AssessmentFormIdentifier.HasValue)
                {
                    var form = ServiceLocator.BankSearch.GetForm(activity.AssessmentFormIdentifier.Value);
                    if (form != null && form.FormPassingScore.HasValue)
                        percent = form.FormPassingScore.Value * 100m;
                }

                {
                    var unitSequence = 1;
                    var moduleSequence = 1;
                    var activitySequence = activity.ActivitySequence;
                    var subSequence = 0;

                    if (activity.Module != null)
                    {
                        moduleSequence = activity.Module.ModuleSequence;
                        if (activity.Module.Unit != null)
                            unitSequence = activity.Module.Unit.UnitSequence;
                    }

                    code = $"{unitSequence}.{moduleSequence}.{activitySequence}";

                    while (gradebook.ContainsCode(code))
                        code = $"{unitSequence}.{moduleSequence}.{activitySequence}.{++subSequence}";
                }

                GradeItemIdentifierField.Visible = false;
                GradeItemFields.Visible = true;
                GradeItemName.Text = activity.ActivityName;
                GradeItemCode.Text = code;
                GradeItemFormat.SelectedValue = format.GetName();
                GradeItemPassPercent.ValueAsDecimal = percent;

                return;
            }

            GradeItemIdentifier.GradebookIdentifier = GradebookIdentifier.Value;
            GradeItemIdentifier.RefreshData();
            GradeItemIdentifier.ValueAsGuid = gradeItemId;

            GradebookOutlineLink.Visible = true;
            GradebookOutlineLink.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}";

            if (gradeItemId.HasValue)
            {
                var gradeItem = ServiceLocator.RecordSearch.GetGradeItem(gradeItemId.Value);
                if (gradeItem != null)
                {
                    GradeItemName.Text = gradeItem.GradeItemName;
                    GradeItemCode.Text = gradeItem.GradeItemCode;
                    GradeItemFormat.SelectedValue = gradeItem.GradeItemFormat;

                    GradeItemPassPercent.ValueAsDecimal = gradeItem.GradeItemPassPercent * 100m;

                    AchievementIdentifier.Value = gradeItem.AchievementIdentifier;

                    GradeItemFields.Visible = true;
                    AchievementPanel.Visible = true;

                    BindModelToControlsForAchievement(gradeItem.AchievementIdentifier, false);
                }
            }
        }

        public void BindModelToControlsForAchievement(Guid? achievementId, bool forceNew)
        {
            AchievementFields.Visible = false;

            if (forceNew)
            {
                AchievementIdentifierField.Visible = false;
                AchievementFields.Visible = true;
                AchievementName.Text = "New Achievement";
                AchievementLabel.Text = string.Empty;
                return;
            }

            var achievement = achievementId.HasValue
                ? ServiceLocator.AchievementSearch.GetAchievement(achievementId.Value)
                : null;

            if (achievement == null)
                return;

            AchievementOutlineLink.NavigateUrl = $"/ui/admin/records/achievements/outline?id={achievementId}";
            AchievementName.Text = achievement.AchievementTitle;
            AchievementLabel.Text = achievement.AchievementLabel;
            AchievementExpiration.Text = achievement.ExpirationToString();
            AchievementLayout.Value = achievement.CertificateLayoutCode;

            AchievementFields.Visible = true;
        }
    }
}