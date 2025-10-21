using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class LinkToAchievement : BlockControl, IBlockControl
    {
        private static readonly string[] _contentLabels = new[] { "Heading" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer block, string hook = null)
        {
            if (hook.IsEmpty())
                return;

            var target = GetText(block, "Heading");

            var achievement = BindAchievement(hook, target);

            if (achievement != null)
                BindCredential(achievement, hook);
        }

        private QAchievement BindAchievement(string hook, string target)
        {
            QAchievement achievement = null;

            if (hook.IsEmpty())
            {
                BlockHeading.Attributes["class"] = "text-danger";
                BlockHeading.InnerText = Translate($"Error: Missing Achievement Hook");
                BlockDescription.InnerHtml = Markdown.ToHtml(Translate("The integration hook for this ") + "**Link to Achievement** " + Translate("content block has not been input by the author of this web page."));
            }
            else
            {
                achievement = ServiceLocator.AchievementSearch.GetAchievement(Organization.Identifier, hook);

                if (achievement != null)
                {
                    BlockHeading.InnerText = target;
                    BlockDescription.InnerHtml = achievement.AchievementDescription;
                }
                else
                {
                    BlockHeading.Attributes["class"] = "text-danger";
                    BlockHeading.InnerText = $"{Translate("Error: Achievement Not Found")}: {hook}";
                    BlockDescription.InnerText = Translate("This achievement is not yet added to your library, or there is a typo in the achievement hook.");
                }
            }

            return achievement;
        }

        private void BindCredential(QAchievement achievement, string hook)
        {
            var credential = ServiceLocator.AchievementSearch.GetCredential(achievement.AchievementIdentifier, User.UserIdentifier);

            if (credential != null)
            {
                PendingLabel.Visible = credential.CredentialStatus == "Pending";
                GrantedLabel.Visible = credential.CredentialStatus == "Valid";
                ExpiredLabel.Visible = credential.CredentialStatus == "Expired";

                if (credential.CredentialGranted.HasValue)
                    BlockDateGranted.Text = "Granted " + TimeZones.FormatDateOnly(credential.CredentialGranted.Value, User.TimeZone);

                if (credential.CredentialExpired.HasValue)
                    BlockDateExpired.Text = "Expires " + TimeZones.FormatDateOnly(credential.CredentialExpired.Value, User.TimeZone);

                BlockDownloadUrl.HRef = GetDownloadCertificateUrl(credential);
                BlockDownloadUrl.Attributes["class"] = "btn btn-sm btn-success";
            }
            else
            {
                PendingLabel.Visible = true;
                GrantedLabel.Visible = false;
                ExpiredLabel.Visible = false;
            }
        }

        private string GetDownloadCertificateUrl(VCredential credential)
        {
            if (credential == null)
                return null;

            if (credential.CredentialStatus == CredentialStatus.Valid.ToString())
                return $"/ui/portal/records/credentials/certificate?achievement={credential.AchievementIdentifier}&user={User.UserIdentifier}";

            return null;
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(3, typeof(LinkToAchievement), "Link to Achievement", "~/UI/Layout/Common/Controls/LinkToAchievement.ascx", _contentLabels);
        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}