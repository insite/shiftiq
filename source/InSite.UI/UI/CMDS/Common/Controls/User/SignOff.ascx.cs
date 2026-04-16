using System;
using System.Linq;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.User.Achievements.Controls
{
    public partial class SignOff : BaseUserControl
    {
        public event EventHandler SignedOff;

        public bool CanBeSigned
        {
            get => (bool)ViewState[nameof(CanBeSigned)];
            set => ViewState[nameof(CanBeSigned)] = value;
        }

        private Guid? CredentialId
        {
            get { return (Guid?)ViewState[nameof(CredentialId)]; }
            set { ViewState[nameof(CredentialId)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AchievementSummary.SignedOff += AchievementSummary_SignedOff;
        }

        private void AchievementSummary_SignedOff(object sender, EventArgs e)
        {
            SignedOff?.Invoke(this, new EventArgs());
        }

        public void LoadData(Guid userId, Guid? credentialId, bool isProgram)
        {
            CredentialId = credentialId;

            SelectedAchievement.Visible = credentialId.HasValue;

            if (credentialId.HasValue)
                LoadAchievementInfo(userId, isProgram);

            if (CurrentSessionState.Identity.IsImpersonating)
                AchievementSummary.DisableSignOffButton();
        }

        public void LoadAchievementInfo(VCmdsCredentialAndExperience credential, bool isProgram)
        {
            var lifetime = credential.LifetimeMonths ?? 0;

            SelectedAchievement.Visible = true;
            AchievementTitle.Text = credential.AchievementTitle;

            var achievementDescription = credential.AchievementDescription?.Trim().NullIfEmpty();

            if (isProgram && credential.AchievementLabel == "Certification")
            {
                var programDescription = TryGetProgramDescription(credential, CurrentLanguage);
                if (programDescription != null)
                    achievementDescription = programDescription;
            }

            if (achievementDescription != null)
                AchievementDescription.Text = Markdown.ToHtml(achievementDescription);

            TimeSensitivePanel.Visible = lifetime > 0;

            if (lifetime > 0)
            {
                var quantity = lifetime;

                var units = "Months";

                if (quantity % 12 == 0)
                {
                    quantity = quantity / 12;

                    units = "Years";
                }

                TimeSensitivePanel.InnerHtml = $"Valid for <strong>{units.ToQuantity(quantity)}</strong>";
            }

            StatusPanel.InnerHtml = $"Status: <strong>{credential.CredentialStatus}</strong>";

            ExpiryPanel.Visible = lifetime > 0;

            if (credential.CredentialExpirationExpected.HasValue)
            {
                var expires = credential.CredentialExpirationExpected
                    .FormatDateOnly(CurrentSessionState.Identity.User.TimeZone.Id);

                ExpiryPanel.InnerHtml = $"Expires: <strong>{expires}</strong>";
            }

            EmployeeDownloadPanel.Visible = credential.CredentialIdentifier.HasValue &&
                EmployeeDownloadList.LoadUploads(credential.CredentialIdentifier.Value);

            AchievementSummary.LoadData(credential.UserIdentifier, credential.AchievementIdentifier, CanBeSigned);
        }

        private bool LoadAchievementInfo(Guid userId, bool isProgram)
        {
            var organization = Organization.Identifier;
            var credential = VCmdsCredentialSearch.SelectForTrainingPlan(CredentialId.Value, organization);

            if (credential == null || credential.UserIdentifier != userId)
                return false;

            LoadAchievementInfo(credential, isProgram);

            return false;
        }

        private static string TryGetProgramDescription(VCmdsCredentialAndExperience credential, string language)
        {
            var programs = ProgramSearch.GetPrograms(new TProgramFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                AchievementIdentifiers = new[] { credential.AchievementIdentifier }
            });

            if (programs.IsEmpty())
                return null;

            var program = programs.First();
            var content = ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier, language, new[] { ContentLabel.Summary });
            var summary = content.Summary.Text[language];

            return summary.NullIfEmpty();
        }
    }
}