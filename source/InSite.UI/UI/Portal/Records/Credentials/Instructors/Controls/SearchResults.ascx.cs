using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Credentials.Instructors.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VCredentialFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public string EmployerGroupName { get; set; }

            public string AchievementTitle { get; set; }
            public string AchievementLabel { get; set; }
            public string CredentialStatus { get; set; }

            public DateTimeOffset? ExpirationExpected { get; set; }
            public DateTimeOffset? Granted { get; set; }
            public DateTimeOffset? Expired { get; set; }
        }

        private class SearchDataItem
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid CredentialIdentifier { get; set; }

            public string UserEmail { get; set; }
            public string UserFullName { get; set; }
            public string EmployerGroupName { get; set; }
            public string AchievementLabel { get; set; }
            public string AchievementTitle { get; set; }
            public string CredentialStatus { get; set; }

            public string AchievementCertificateLayoutCode { get; set; }
            public string BadgeImageUrl { get; set; }
            public string DownloadLink { get; set; }
            public bool HasBadgeImage { get; set; }

            public decimal? CredentialGrantedScore { get; set; }

            public DateTimeOffset? CredentialGranted { get; set; }
            public DateTimeOffset? CredentialExpired { get; set; }
            public DateTimeOffset? CredentialExpirationExpected { get; set; }
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        #endregion

        #region Event handlers

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Expire":
                    var credentialIdentifier = Guid.Parse(e.CommandArgument as string);
                    var command = new ExpireCredential(credentialIdentifier, DateTimeOffset.UtcNow);
                    ServiceLocator.SendCommand(command);
                    RefreshGrid();
                    break;
            }
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(VCredentialFilter filter)
            => ServiceLocator.AchievementSearch.CountCredentials(filter);

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(VCredentialFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<SearchDataItem>().Select(x => new ExportDataItem
            {
                Granted = x.CredentialGranted,
                UserName = x.UserFullName,
                UserEmail = x.UserEmail,
                EmployerGroupName = x.EmployerGroupName,
                AchievementTitle = x.AchievementTitle,
                AchievementLabel = x.AchievementLabel,
                CredentialStatus = x.CredentialStatus,
                ExpirationExpected = x.CredentialExpirationExpected,
                Expired = x.CredentialExpired
            }).ToList().ToSearchResult();
        }

        protected override IListSource SelectData(VCredentialFilter filter)
        {
            filter.OrderBy = $"{nameof(VCredential.CredentialGranted)} desc, {nameof(VCredential.UserFullName)}";

            var items = ServiceLocator.AchievementSearch
                .GetCredentials(filter)
                .Select(x => new SearchDataItem
                {
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    UserIdentifier = x.UserIdentifier,
                    UserEmail = x.UserEmail,
                    UserFullName = x.UserFullName,
                    EmployerGroupName = x.EmployerGroupName,
                    AchievementLabel = x.AchievementLabel,
                    AchievementTitle = x.AchievementTitle,
                    CredentialStatus = x.CredentialStatus,
                    CredentialGranted = x.CredentialGranted,
                    CredentialExpirationExpected = x.CredentialExpirationExpected,
                    CredentialGrantedScore = x.CredentialGrantedScore,
                    CredentialExpired = x.CredentialExpired,
                    CredentialIdentifier = x.CredentialIdentifier,

                    AchievementCertificateLayoutCode = x.AchievementCertificateLayoutCode,
                    BadgeImageUrl = x.BadgeImageUrl,
                    HasBadgeImage = x.HasBadgeImage ?? false

                }).ToList();

            var files = ServiceLocator.FileSearch
                .GetModels(filter.OrganizationIdentifier, items.Select(x => x.CredentialIdentifier).ToArray(), null, false);

            foreach (var item in items)
            {
                var status = item.CredentialStatus.ToEnum(CredentialStatus.Undefined);

                string fileUrl = null;
                var file = files.FirstOrDefault(x => x.ObjectIdentifier == item.CredentialIdentifier);
                if (file != null)
                    fileUrl = ServiceLocator.StorageService.GetFileUrl(file.FileIdentifier, file.FileName, true);

                string badgeUrl = null;
                if (item.HasBadgeImage && item.BadgeImageUrl.IsNotEmpty())
                    badgeUrl = item.BadgeImageUrl;

                var layout = item.AchievementCertificateLayoutCode;

                item.DownloadLink = InSite.UI.Portal.Records.Credentials.Learners.Controls.SearchResults
                    .GetDownloadLink(item.CredentialIdentifier, status, fileUrl, badgeUrl, layout);
            }

            return items.ToSearchResult();

        }
        #endregion

        #region Methods (render)
        protected static string GetDateString(DateTimeOffset? date) =>
            date.Format(User.TimeZone, true, true, false,
                CultureInfo.GetCultureInfo(CurrentSessionState.Identity.Language), nullValue: string.Empty);

        protected string GetCredentialExpiry(object item)
        {
            return item != null
                ? GetCredentialExpiry((SearchDataItem)item)
                : null;
        }

        private string GetCredentialExpiry(SearchDataItem credential)
        {
            var expected = credential.CredentialExpirationExpected;
            var actual = credential.CredentialExpired;
            var now = DateTimeOffset.UtcNow;
            var culture = CultureInfo.GetCultureInfo(CurrentSessionState.Identity.Language);

            if (expected.HasValue && expected.Value > now)
            {
                var age = TimeZones.Format(expected.Value, User.TimeZone, true, false, false, culture);
                var span = expected.Value - now;
                var days = span.TotalDays;

                if (days > 90)
                    return $"<div>{age}</div><div class='badge bg-success'>" +
                        $"{span.Humanize(1, true, culture, TimeUnit.Month)} {Translate("from now")}</div>";
                else
                    return $"<div>{age}</div><div class='badge bg-warning'>" +
                        $"{expected.Value.Humanize(null, culture)}</div>";
            }
            else if (actual.HasValue)
            {
                var age = TimeZones.Format(actual.Value, User.TimeZone, true);

                return $"<div>{age}</div><div class='badge bg-danger'>" +
                    $"{actual.Value.Humanize(null, culture)}</div>";
            }

            return null;
        }

        #endregion
    }
}