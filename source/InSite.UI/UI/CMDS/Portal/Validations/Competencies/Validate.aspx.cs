using System;

using InSite.Application.StandardValidations.Write;
using InSite.Cmds.Controls.Talents.EmployeeCompetencies;
using InSite.Cmds.User.Competencies.Controls;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.User.Competencies.Forms
{
    public partial class Validate : EmployeeCompetencyEditorController, IHasParentLinkParameters
    {
        private void CompetencyAchievements_SignedOff(CompetencyAchievementAndDownloadViewer sender, VCmdsCredential employeeAchievement)
        {
            var achievement = VCmdsAchievementSearch.Select(employeeAchievement.AchievementIdentifier);

            EditorStatus.AddMessage(AlertType.Success, $"Achievement has been signed off: {achievement.AchievementTitle}");
        }

        protected override string SearchRouteAction => "ui/cmds/design/validations/competencies/search";

        protected override bool IsCompetenciesToValidate => true;

        protected override bool MustSaveOnMove =>
            ValidationPanel.Visible && !string.IsNullOrEmpty(ValidatorSelection.SelectedValue);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CompetencyAchievements.SignedOff += CompetencyAchievements_SignedOff;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (CurrentIdentifier == Guid.Empty)
                HttpResponseHelper.Redirect(SearchUrl);

            base.OnLoad(e);

            if (!IsPostBack)
            {
                CheckAllowedToValidate();
                InitPredefinedComments();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            YesButton.ButtonStyle = ValidatorSelection.SelectedIndex == 0
                ? ButtonStyle.Success
                : ButtonStyle.Default;

            NoButton.ButtonStyle = ValidatorSelection.SelectedIndex == 1
                ? ButtonStyle.Success
                : ButtonStyle.Default;

            if (Identity.IsImpersonating)
            {
                YesButton.Enabled = false;
                NoButton.Enabled = false;
            }
        }

        protected override void Save()
        {
            var entity = ServiceLocator.StandardValidationSearch.GetStandardValidation(CurrentIdentifier, UserId);
            var isValidated = StringHelper.Equals(ValidatorSelection.SelectedValue, "Yes");

            if (!isValidated && string.IsNullOrWhiteSpace(ValidatorComment.Text))
                return;

            // If the candidate states that this competency is not applicable in his job, and if the validator agrees with this self-assessment,
            // then the validation status should match the self-assessment status (i.e. Not Applicable).

            var status = !isValidated
                ? ValidationStatuses.NeedsTraining
                : entity.SelfAssessmentStatus == SelfAssessedStatuses.NotApplicable
                    ? ValidationStatuses.NotApplicable
                    : ValidationStatuses.Validated;

            ServiceLocator.SendCommand(new ValidateStandardValidation(entity.StandardValidationIdentifier, UniqueIdentifier.Create(), isValidated, status, ValidatorComment.Text));
        }

        protected override void SetInputValues(StandardValidation info)
        {
            var contact = UserSearch.Select(UserId);
            var competency = CompetencyRepository.Select(info.StandardIdentifier);

            PageHelper.AutoBindHeader(this, null, $"#{competency.Number} ({contact.FullName})");

            Number.Text = competency.Number;
            Summary.Text = competency.Summary;
            Category.Text = GetCategories(competency.StandardIdentifier) ?? "N/A";

            if (!string.IsNullOrEmpty(competency.Knowledge))
                Knowledge.Text = Markdown.ToHtml(competency.Knowledge);
            else
                Knowledge.Text = "-";

            if (!string.IsNullOrEmpty(competency.Skills))
                Skills.Text = Markdown.ToHtml(competency.Skills);
            else
                Skills.Text = "-";

            SelfAssessmentStatus.LoadData(Identity.Organization.Identifier, Identity.Organization.ParentOrganizationIdentifier);
            SelfAssessmentStatus.SelectedValue = info.SelfAssessmentStatus;

            ValidatorComment.Text = null;

            CompetencyAchievements.HideDownloads();
        }

        private void InitPredefinedComments()
        {
            PredefinedComments.DataSource = Comments.Validator;
            PredefinedComments.DataBind();
        }

        private void CheckAllowedToValidate()
        {
            if (!Access.Configure && !Identity.IsInRole(CmdsRole.SuperValidators))
            {
                var entity = UserCompetencyRepository.Select(User.UserIdentifier, CurrentIdentifier);

                var isAllowedToValidate = entity != null && entity.IsValidated;

                ValidationPanel.Visible = isAllowedToValidate;
                NoValidationPanel.Visible = !isAllowedToValidate;

                if (!isAllowedToValidate)
                    NoValidationReason.Text = entity == null
                        ? "you do not have this competency"
                        : "you are not validated in this competency";
            }

            if (Identity.IsImpersonating)
            {
                YesButton.Enabled = false;
                NoButton.Enabled = false;
            }
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/search")
                ? $"userID={UserId}"
                : null;
        }
    }
}
