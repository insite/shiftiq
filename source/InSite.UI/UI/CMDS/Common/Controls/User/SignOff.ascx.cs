using System;
using System.Linq;
using System.Text;
using System.Web.UI;

using Humanizer;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.User.Achievements.Controls
{
    public partial class SignOff : UserControl
    {
        public event EventHandler SignedOff;

        public bool CanBeSigned
        {
            get => (bool)ViewState[nameof(CanBeSigned)];
            set => ViewState[nameof(CanBeSigned)] = value;
        }

        private Guid? CredentialIdentifier
        {
            get { return (Guid?)ViewState[nameof(CredentialIdentifier)]; }
            set { ViewState[nameof(CredentialIdentifier)] = value; }
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

        public string CurrentLanguage => CookieTokenModule.Current.Language;

        public void LoadData(Guid userId, Guid? credentialIdentifier)
        {
            CredentialIdentifier = credentialIdentifier;

            SelectedAchievement.Visible = credentialIdentifier.HasValue;

            if (credentialIdentifier.HasValue)
                LoadAchievementInfo(userId);

            if (CurrentSessionState.Identity.IsImpersonating)
                AchievementSummary.DisableSignOffButton();
        }

        public void LoadAchievementInfo(VCmdsCredentialAndExperience credential)
        {
            var lifetime = credential.LifetimeMonths ?? 0;

            SelectedAchievement.Visible = true;
            AchievementTitle.Text = credential.AchievementTitle;

            if (credential.AchievementDescription.HasValue())
                AchievementDescription.Text = Markdown.ToHtml(credential.AchievementDescription);

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

        private bool LoadAchievementInfo(Guid userId)
        {
            var organization = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            var credential = VCmdsCredentialSearch.SelectForTrainingPlan(CredentialIdentifier.Value, organization);

            if (credential == null || credential.UserIdentifier != userId)
                return false;

            LoadAchievementInfo(credential);
            LoadProgramOnlyAchievementSummary(organization, credential.AchievementIdentifier);

            return false;
        }

        private void LoadProgramOnlyAchievementSummary(Guid organizationId, Guid achievementId)
        {
            var tasks = TaskSearch.Select(x=>x.ObjectIdentifier ==  achievementId && x.OrganizationIdentifier == organizationId, y => y.Program).ToList();

            if(tasks == null || tasks.Count == 0) 
                return;

            var summary = new StringBuilder();

            foreach(var task in tasks)
            {
                if (task.Program.ProgramType != "Achievements Only")
                    continue;

                var content = ServiceLocator.ContentSearch.GetBlock(task.ProgramIdentifier, CurrentLanguage);
                summary.Append(content.Summary.GetHtml(CurrentLanguage));
            }

            ProgramOnlyAchievementSummary.Text = summary.ToString();
        }
    }
}