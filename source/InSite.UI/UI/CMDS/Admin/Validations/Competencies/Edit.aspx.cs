using System;
using System.Collections.Generic;

using InSite.Application.StandardValidations.Write;
using InSite.Cmds.Controls.Talents.EmployeeCompetencies;
using InSite.Cmds.User.Competencies.Controls;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Actions.Talent.Employee.Competency.Admin
{
    public partial class Edit : EmployeeCompetencyEditorController
    {
        protected override string SearchRouteAction => "ui/cmds/admin/validations/competencies/search";

        private void CompetencyAchievements_SignedOff(CompetencyAchievementAndDownloadViewer sender, VCmdsCredential credential)
        {
            var achievement = VCmdsAchievementSearch.Select(credential.AchievementIdentifier);

            EditorStatus.AddMessage(AlertType.Success, string.Format("Achievement '{0}' have been signed off.", achievement.AchievementTitle));
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CompetencyAchievements.SignedOff += CompetencyAchievements_SignedOff;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (CurrentIdentifier == Guid.Empty)
                Response.Redirect(@"/ui/cmds/admin/validations/competencies/search");

            base.OnLoad(e);

            if (!IsPostBack)
                InitPredefinedComments();
        }

        protected override void Save()
        {
            var entity = ServiceLocator.StandardValidationSearch.GetStandardValidation(CurrentIdentifier, UserId);

            ServiceLocator.SendCommand(new ModifyStandardValidationStatus(
                entity.StandardValidationIdentifier, UniqueIdentifier.Create(),
                IsValidated.Checked, SelfAssessmentStatus.SelectedValue, ValidationStatus.Value, ValidationComment.Text));
        }

        protected override void SetInputValues(StandardValidation info)
        {
            var contact = UserSearch.Select(UserId);
            var competency = CompetencyRepository.Select(info.StandardIdentifier);

            PageHelper.AutoBindHeader(this, null, $"#{competency.Number} ({contact.FullName})");

            SelfAssessmentQuestion.Text = $"For this competency, I, {contact.FullName} have the required knowledge, skills, and experience to work independently.";

            NumberOld.Text = !string.IsNullOrEmpty(competency.NumberOld) ? competency.NumberOld : "N/A";
            Number.Text = competency.Number;
            Summary.Text = competency.Summary;
            Category.Text = Category.Text = GetCategories(competency.StandardIdentifier) ?? "N/A";

            if (!string.IsNullOrEmpty(competency.Knowledge))
                Knowledge.Text = competency.Knowledge.Replace("\r", "").Replace("\n", "<br/>");

            if (!string.IsNullOrEmpty(competency.Skills))
                Skills.Text = competency.Skills.Replace("\r", "").Replace("\n", "<br/>");

            ValidationStatusLabel.Text = info.ValidationStatus.IfNullOrEmpty("N/A");
            SelfAssessmentDate.Text = info.SelfAssessmentDate.Format(User.TimeZone, nullValue: "N/A");
            ValidationDate.Text = info.ValidationDate.Format(User.TimeZone, nullValue: "N/A");
            ExpirationDate.Text = info.Expired.Format(User.TimeZone, nullValue: "N/A");

            ValidationStatus.Value = info.ValidationStatus;
            ValidationComment.Text = info.ValidationComment;
            SelfAssessmentStatus.SelectedValue = info.SelfAssessmentStatus;
            IsValidated.Checked = info.IsValidated;

            ValidatorField.Visible = info.ValidatorUserIdentifier.HasValue;

            if (info.ValidatorUserIdentifier.HasValue)
            {
                var validator = UserSearch.Select(info.ValidatorUserIdentifier.Value);
                ValidatorName.Text = validator.FullName;
            }
        }

        private void InitPredefinedComments()
        {
            var comments = new List<string> { "The status of this competency was imported from the previous database." };
            PredefinedComments.DataSource = comments;
            PredefinedComments.DataBind();
        }
    }
}
