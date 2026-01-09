using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Achievements.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Programs
{
    public partial class ModifyAchievement : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/modify-achievement";

        public static string GetNavigateUrl(Guid programId) => NavigateUrl + "?id=" + programId;

        public static void Redirect(Guid programId) => HttpResponseHelper.Redirect(GetNavigateUrl(programId));

        private Guid? ProgramId => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementIdentifier.AutoPostBack = true;
            AchievementIdentifier.ValueChanged += (x, y) => BindModelToControlsForAchievement(AchievementIdentifier.Value, false);
            AchievementCreateButton.Click += (x, y) => BindModelToControlsForAchievement(null, true);

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #region Methods (open)

        private void Open()
        {
            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, program.ProgramName);

            AchievementIdentifier.Value = program.AchievementIdentifier;

            BindModelToControlsForAchievement(program.AchievementIdentifier, false);

            CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramId.Value);
        }

        public void BindModelToControlsForAchievement(Guid? achievementId, bool forceNew)
        {
            AchievementFields.Visible = forceNew;
            AchievementIdentifierField.Visible = !forceNew;
            SaveButton.Text = "Update Achievement";

            if (forceNew)
            {
                AchievementName.Text = "New Achievement";
                AchievementExpiration.SetExpiration();
                AchievementLabel.Text = string.Empty;
                AchievementLayout.Value = null;
                SaveButton.Text = "Add Achievement";
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
            AchievementExpiration.SetExpiration(achievement);
            AchievementLayout.Value = achievement.CertificateLayoutCode;

            AchievementFields.Visible = true;
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                return;

            if (!AchievementIdentifierField.Visible)
                Create(program);
            else
                Modify(program);

            Outline.Redirect(program.ProgramIdentifier);
        }

        private void Modify(TProgram program)
        {
            if (AchievementIdentifier.HasValue)
            {
                var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value.Value);

                if (achievement == null)
                    return;

                if (!achievement.AchievementIsEnabled)
                {
                    EditorStatus.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
                    return;
                }

                if (achievement.AchievementLabel != AchievementLabel.Text || achievement.AchievementTitle != AchievementName.Text)
                    ServiceLocator.SendCommand(new DescribeAchievement(
                        achievement.AchievementIdentifier, AchievementLabel.Text, AchievementName.Text, achievement.AchievementDescription, false));

                if (achievement.CertificateLayoutCode != AchievementLayout.Value)
                    ServiceLocator.SendCommand(new ChangeCertificateLayout(
                        achievement.AchievementIdentifier, AchievementLayout.Value));

                ServiceLocator.SendCommand(new ChangeAchievementExpiry(AchievementIdentifier.Value.Value, AchievementExpiration.GetExpiration()));
            }

            if (program.AchievementIdentifier == null && AchievementIdentifier.Value == null)
                return;

            if (program.AchievementIdentifier == null && AchievementIdentifier.Value != null)
                AssignAchievementToProgram(AchievementIdentifier.Value.Value, program);

            else if (program.AchievementIdentifier != null && AchievementIdentifier.Value == null)
            {
                program.AchievementIdentifier = null;
                program.AchievementWhenChange = null;
                program.AchievementWhenGrade = null;
                program.AchievementThenCommand = null;
                program.AchievementElseCommand = null;
                program.AchievementFixedDate = null;
                ProgramStore.Update(program, User.Identifier);
            }

            else
            {
                program.AchievementIdentifier = AchievementIdentifier.Value.Value;
                ProgramStore.Update(program, User.Identifier);
            }

            CheckProgramCompletion(program);
            BindModelToControlsForAchievement(AchievementIdentifier.Value, false);

            EditorStatus.AddMessage(AlertType.Success, "The achievement have been updated");
        }

        private void Create(TProgram program)
        {
            var id = UniqueIdentifier.Create();
            var expiration = AchievementExpiration.GetExpiration();

            ServiceLocator.SendCommand(new Application.Achievements.Write.CreateAchievement(
                id, Organization.OrganizationIdentifier, AchievementLabel.Text, AchievementName.Text, null, expiration, null));

            var layout = AchievementLayout.Value;
            if (layout.IsNotEmpty())
                ServiceLocator.SendCommand(new ChangeCertificateLayout(id, layout));

            AssignAchievementToProgram(id, program);
            CheckProgramCompletion(program);
            BindModelToControlsForAchievement(AchievementIdentifier.Value = id, false);

            EditorStatus.AddMessage(AlertType.Success, "The achievement have been created");
        }

        private void AssignAchievementToProgram(Guid achievement, TProgram program)
        {
            program.AchievementIdentifier = achievement;
            program.AchievementWhenChange = TriggerCauseChange.Changed.ToString();
            program.AchievementWhenGrade = TriggerCauseGrade.Pass.ToString();
            program.AchievementThenCommand = TriggerEffectCommand.Grant.ToString();
            program.AchievementElseCommand = TriggerEffectCommand.Void.ToString();

            ProgramStore.Update(program, User.Identifier);
        }

        private static void CheckProgramCompletion(TProgram program)
        {
            var enrollments = ProgramSearch1
                .GetProgramUsers(new VProgramEnrollmentFilter
                {
                    ProgramIdentifier = program.ProgramIdentifier,
                    OrganizationIdentifier = program.OrganizationIdentifier
                })
                .Select(x => x.UserIdentifier)
                .ToList();

            foreach (var userIdentifier in enrollments)
            {
                if ((program.CompletionTaskIdentifier.HasValue && ServiceLocator.ProgramSearch.IsTaskCompletedByLearner(program.CompletionTaskIdentifier.Value, userIdentifier)) ||
                    ServiceLocator.ProgramSearch.IsProgramFullyCompletedByLearner(program.ProgramIdentifier, userIdentifier))
                {
                    if (program.AchievementIdentifier.HasValue)
                        ProgramHelper.SendGrantCommands(TriggerEffectCommand.Grant, CurrentSessionState.Identity.Organization.Identifier, program.AchievementIdentifier.Value, userIdentifier);

                    ProgramStore.InsertEnrollment(Organization.Identifier, program.ProgramIdentifier, userIdentifier, User.Identifier, DateTimeOffset.UtcNow);
                }
            }
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramId}"
                : null;
        }

        #endregion
    }
}