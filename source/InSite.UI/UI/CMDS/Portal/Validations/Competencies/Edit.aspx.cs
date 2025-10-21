using System;

using InSite.Application.StandardValidations.Write;
using InSite.Cmds.Controls.Talents.EmployeeCompetencies;
using InSite.Cmds.User.Competencies.Controls;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Actions.Talent.Employee.Competency.Assessment
{
    public partial class Edit : EmployeeCompetencyEditorController
    {
        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            bool isSelfAssessmentEnabled = UserId == User.UserIdentifier;

            NextButton.Enabled = isSelfAssessmentEnabled;
            PrevButton.Enabled = isSelfAssessmentEnabled;
            SubmitForValidation.Enabled = isSelfAssessmentEnabled;

            if (!isSelfAssessmentEnabled || Identity.IsImpersonating)
            {
                SelfAssessmentStatus.Enabled = false;
                WarningPanel.Visible = false;
            }
        }

        protected override string SearchRouteAction => "ui/cmds/portal/validations/competencies/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubmitForValidation.Click += SubmitForValidation_Click;
            CompetencyAchievements.SignedOff += CompetencyAchievements_SignedOff;
            CompetencyAchievements.ModuleQuizCompletedChanged += CompetencyAchievements_ModuleQuizCompletedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (CurrentIdentifier == Guid.Empty)
                Response.Redirect("/ui/cmds/portal/validations/competencies/search");

            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                SetFocusAndSelect();
        }

        private void SubmitForValidation_Click(Object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            Response.Redirect("/ui/cmds/portal/validations/competencies/submit");
        }

        private void CompetencyAchievements_SignedOff(CompetencyAchievementAndDownloadViewer sender, VCmdsCredential employeeAchievement)
        {
            var achievement = VCmdsAchievementSearch.Select(employeeAchievement.AchievementIdentifier);

            EditorStatus.AddMessage(AlertType.Success, string.Format("Achievement '{0}' have been signed off.", achievement.AchievementTitle));
        }

        private void CompetencyAchievements_ModuleQuizCompletedChanged(Object sender, EventArgs e)
        {
            if (CheckSelfAssessmentEnabled())
            {
                EditorStatus.AddMessage(AlertType.Success, "You may now self-assess on this competency");
            }
            else
            {
                EditorStatus.AddMessage(AlertType.Error, "You may not self-assess until successfully completing the module quiz requirement for this competency");
            }
        }

        protected override void Open()
        {
            base.Open();

            CheckSelfAssessmentEnabled();
        }

        protected override void Save()
        {
            var status = SelfAssessmentStatus.SelectedValue;
            if (status.IsEmpty())
                return;

            var entity = ServiceLocator.StandardValidationSearch.GetStandardValidation(CurrentIdentifier, UserId);

            ServiceLocator.SendCommand(new SelfValidateStandardValidation(entity.StandardValidationIdentifier, UniqueIdentifier.Create(), status));
        }

        private bool CheckSelfAssessmentEnabled()
        {
            SelfAssessmentAllowedStatusPanel.Visible = false;

            if (UserId != User.UserIdentifier)
                return false;

            var isSelfAssessmentEnabled = true;

            CompetencyAchievements.SetQuizCheckbox(false, false);

            if (CompetencyAchievements.HasAchievements)
            {
                var enableLearningModuleDependencyOnSelfAssessment = false;

                if (enableLearningModuleDependencyOnSelfAssessment)
                {
                    var employeeCompetency = UserCompetencyRepository.Select(UserId, CurrentIdentifier);

                    isSelfAssessmentEnabled = employeeCompetency.IsModuleQuizCompleted;

                    SelfAssessmentAllowedStatusPanel.Visible = true;
                    SelfAssessmentPanel.Visible = employeeCompetency.IsModuleQuizCompleted;

                    SelfAssessmentAllowedStatus.Text = employeeCompetency.IsModuleQuizCompleted
                        ? "You may now self-assess on this competency"
                        : "You may not self-assess until successfully completing the module quiz requirement for this competency";

                    CompetencyAchievements.SetQuizCheckbox(true, employeeCompetency.IsModuleQuizCompleted);
                }
            }

            NextButton.Enabled = isSelfAssessmentEnabled;
            PrevButton.Enabled = isSelfAssessmentEnabled;
            SubmitForValidation.Enabled = isSelfAssessmentEnabled;

            return isSelfAssessmentEnabled;
        }

        protected override void SetInputValues(StandardValidation info)
        {
            var contact = UserSearch.Select(UserId);
            var competency = CompetencyRepository.Select(info.StandardIdentifier);

            PageHelper.AutoBindHeader(this, null, $"#{competency.Number} ({contact.FullName})");

            SelfAssessmentQuestion.Text = $"For this competency, I, {contact.FullName} have the required knowledge, skills, and experience to work independently.";

            Number.Text = competency.Number;
            Summary.Text = competency.Summary;
            Category.Text = GetCategories(competency.StandardIdentifier) ?? "N/A";

            if (!String.IsNullOrEmpty(competency.Knowledge))
                Knowledge.Text = competency.Knowledge.Replace("\r", "").Replace("\n", "<br/>");

            if (!String.IsNullOrEmpty(competency.Skills))
                Skills.Text = competency.Skills.Replace("\r", "").Replace("\n", "<br/>");

            StatusLabel.Text = String.IsNullOrEmpty(info.ValidationStatus) ? "N/A" : info.ValidationStatus;

            SelfAssessmentStatus.LoadData(Identity.Organization.Identifier, Identity.Organization.ParentOrganizationIdentifier);
            SelfAssessmentStatus.SelectedValue = info.SelfAssessmentStatus;

            CompetencyAchievements.HideDownloads();
        }

        private void SetFocusAndSelect()
        {
            var script = @"
                function setFocusAndScrollUp()
                {
                    const ctrl = document.getElementById('" + FocusButton.ClientID + @"');
                    ctrl.focus();

                    setTimeout('window.scrollTo(0,0)', 0);
                }

                window.onload = setFocusAndScrollUp;
                ";

            Page.ClientScript.RegisterClientScriptBlock(typeof(Edit), "SetFocusAndSelect", script, true);
        }

    }
}