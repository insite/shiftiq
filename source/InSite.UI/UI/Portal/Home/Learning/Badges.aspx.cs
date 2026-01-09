using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.UI.Admin.Records.Credentials.Utilities;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Home.Learning
{
    public partial class Badges : PortalBasePage
    {
        #region Classes

        private class SearchDataItem
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid CredentialIdentifier { get; set; }

            public string UserEmail { get; set; }
            public string UserFullName { get; set; }
            public string AchievementLabel { get; set; }
            public string AchievementTitle { get; set; }
            public string CredentialStatus { get; set; }
            public decimal? CredentialGrantedScore { get; set; }


            public DateTimeOffset? CredentialGranted { get; set; }
            public DateTimeOffset? CredentialExpired { get; set; }
            public DateTimeOffset? CredentialExpirationExpected { get; set; }

            public string AchievementCertificateLayoutCode { get; set; }

            public bool? HasBadgeImage { get; set; }
            public string BadgeImageUrl { get; internal set; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BadgeRepeater.ItemCommand += BadgeRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/learning/dashboard/home");
            PortalMaster.RenderHelpContent(null);
            PortalMaster.HideBreadcrumbsAndTitle();

            PageHelper.AutoBindHeader(this);

            LoadBadges();
        }

        private void BadgeRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DownloadBadge")
            {
                var parts = ((string)e.CommandArgument).Split(';');
                var credentialId = Guid.Parse(parts[0]);
                var badgeImageUrl = parts[1];

                var svgBytes = BadgeHelper.GetBadgeSVGFile(badgeImageUrl, credentialId);
                if (svgBytes == null || svgBytes.Length == 0)
                    return;

                Response.SendFile("badge.svg", svgBytes, "image/svg+xml");
            }
        }

        private void LoadBadges()
        {
            var filter = new VCredentialFilter
            {
                CredentialStatus = CredentialStatus.Valid.ToString(),
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifier = User.Identifier,
            };

            var list = ServiceLocator.AchievementSearch
                .GetCredentials(filter)
                .Select(x => new SearchDataItem
                {
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    UserIdentifier = x.UserIdentifier,
                    UserEmail = x.UserEmail,
                    UserFullName = x.UserFullName,
                    AchievementLabel = x.AchievementLabel,
                    AchievementTitle = x.AchievementTitle,
                    CredentialStatus = x.CredentialStatus,
                    CredentialGranted = x.CredentialGranted,
                    CredentialExpirationExpected = x.CredentialExpirationExpected,
                    CredentialGrantedScore = x.CredentialGrantedScore,
                    CredentialExpired = x.CredentialExpired,
                    CredentialIdentifier = x.CredentialIdentifier,
                    AchievementCertificateLayoutCode = x.AchievementCertificateLayoutCode,
                    HasBadgeImage = x.HasBadgeImage,
                    BadgeImageUrl = x.BadgeImageUrl
                })
                .Where(x => x.HasBadgeImage == true)
                .OrderBy(x => x.AchievementLabel)
                .ThenBy(x => x.AchievementTitle)
                .ToList();

            NoBadges.Visible = list.Count == 0;

            BadgeRepeater.Visible = list.Count > 0;
            BadgeRepeater.DataSource = list;
            BadgeRepeater.DataBind();
        }

        protected string GetExpiryHtml()
        {
            var item = (SearchDataItem)Page.GetDataItem();
            if (!item.CredentialExpirationExpected.HasValue)
                return string.Empty;
            return "Expiry on " + LocalizeDate(item.CredentialExpirationExpected.Value) + " (" + Shift.Common.Humanizer.Humanize(item.CredentialExpirationExpected.Value) + ")";
        }

        protected string GetGrantedHtml()
        {
            var item = (SearchDataItem)Page.GetDataItem();
            if (!item.CredentialGranted.HasValue)
                return string.Empty;
            return "Granted on " + LocalizeDate(item.CredentialGranted.Value) + " (" + Shift.Common.Humanizer.Humanize(item.CredentialGranted.Value) + ")";
        }

        protected string GetShareHtml()
        {
            var item = (SearchDataItem)Page.GetDataItem();
            if (item.CredentialStatus != "Valid")
                return string.Empty;

            var brand = ServiceLocator.AppSettings.Release.Brand;

            return $@"
<div class='btn-group' role='group'>
  <button type='button' class='btn btn-sm btn-outline-secondary dropdown-toggle' data-bs-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>
    <i class='fas fa-share me-1'></i>Share
  </button>
  <div class='dropdown-menu'>
    <a target='_blank' href='{Lobby.CeritifcateVerify.GenerateEmailContent(brand, item.CredentialIdentifier, item.AchievementTitle, Request)}' class='dropdown-item'><i class='fas fa-paper-plane me-1'></i>Email</a>
    <a href='javascript:void(0)' class='dropdown-item' onclick='navigator.clipboard.writeText(""{Lobby.CeritifcateVerify.GenerateVerificationLink(item.CredentialIdentifier, Request, false)}"");'><span style='margin-left: -3px;margin-right: -2px;'><svg width='20' height='20' viewBox='0 0 24 24'><path fill='#5a5b75' fill-rule='evenodd' clip-rule='evenodd' d='M3.57 14.67c0-.57.13-1.11.38-1.6l.02-.02v-.02l.02-.02c0-.02 0-.02.02-.02.12-.26.3-.52.57-.8L7.78 9v-.02l.01-.02c.44-.41.91-.7 1.44-.85a4.87 4.87 0 0 0-1.19 2.36A5.04 5.04 0 0 0 8 11.6L6.04 13.6c-.19.19-.32.4-.38.65a2 2 0 0 0 0 .9c.08.2.2.4.38.57l1.29 1.31c.27.28.62.42 1.03.42.42 0 .78-.14 1.06-.42l1.23-1.25.79-.78 1.15-1.16c.08-.09.19-.22.28-.4.1-.2.15-.42.15-.67 0-.16-.02-.3-.06-.45l-.02-.02v-.02l-.07-.14s0-.03-.04-.06l-.06-.13-.02-.02c0-.02 0-.03-.02-.05a.6.6 0 0 0-.14-.16l-.48-.5c0-.04.02-.1.04-.15l.06-.12 1.17-1.14.09-.09.56.57c.02.04.08.1.16.18l.05.04.03.06.04.05.03.04.04.06.1.14.02.02c0 .02.01.03.03.04l.1.2v.02c.1.16.2.38.3.68a1 1 0 0 1 .04.25 3.2 3.2 0 0 1 .02 1.33 3.49 3.49 0 0 1-.95 1.87l-.66.67-.97.97-1.56 1.57a3.4 3.4 0 0 1-2.47 1.02c-.97 0-1.8-.34-2.49-1.03l-1.3-1.3a3.55 3.55 0 0 1-1-2.51v-.01h-.02v.02zm5.39-3.43c0-.19.02-.4.07-.63.13-.74.44-1.37.95-1.87l.66-.67.97-.98 1.56-1.56c.68-.69 1.5-1.03 2.47-1.03.97 0 1.8.34 2.48 1.02l1.3 1.32a3.48 3.48 0 0 1 1 2.48c0 .58-.11 1.11-.37 1.6l-.02.02v.02l-.02.04c-.14.27-.35.54-.6.8L16.23 15l-.01.02-.01.02c-.44.42-.92.7-1.43.83a4.55 4.55 0 0 0 1.23-3.52L18 10.38c.18-.21.3-.42.35-.65a2.03 2.03 0 0 0-.01-.9 1.96 1.96 0 0 0-.36-.58l-1.3-1.3a1.49 1.49 0 0 0-1.06-.42c-.42 0-.77.14-1.06.4l-1.2 1.27-.8.8-1.16 1.15c-.08.08-.18.21-.29.4a1.66 1.66 0 0 0-.08 1.12l.02.03v.02l.06.14s.01.03.05.06l.06.13.02.02.01.02.01.02c.05.08.1.13.14.16l.47.5c0 .04-.02.09-.04.15l-.06.12-1.15 1.15-.1.08-.56-.56a2.3 2.3 0 0 0-.18-.19c-.02-.01-.02-.03-.02-.04l-.02-.02a.37.37 0 0 1-.1-.12c-.03-.03-.05-.04-.05-.06l-.1-.15-.02-.02-.02-.04-.08-.17v-.02a5.1 5.1 0 0 1-.28-.69 1.03 1.03 0 0 1-.04-.26c-.06-.23-.1-.46-.1-.7v.01z'></path></svg></span> Copy Link</a> 
  </div>
</div>";
        }
    }
}