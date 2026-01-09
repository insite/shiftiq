using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Credentials.Learners.Controls
{
    public class SearchResultItem
    {
        public string AchievementType { get; internal set; }
        public string AchievementTitle { get; internal set; }

        public Guid CredentialIdentifier { get; internal set; }
        public string CredentialIssued { get; internal set; }
        public string CredentialExpiry { get; internal set; }
        public string CredentialStatus { get; internal set; }

        public string DownloadLink { get; set; }
        public string DeleteLink { get; set; }

        public bool HasBadgeImage { get; set; }
        public string BadgeImageUrl { get; set; }

        public bool IsSelfDeclared { get; set; }
    }

    public partial class SearchResults : SearchResultsGridViewController<VCredentialFilter>
    {
        public class ExportDataItem
        {
            public string AchievementTitle { get; set; }
            public string AchievementLabel { get; set; }
            public string CredentialStatus { get; set; }

            public DateTimeOffset? Granted { get; set; }
            public DateTimeOffset? ExpirationExpected { get; set; }
            public DateTimeOffset? Expired { get; set; }
        }

        public override IListSource GetExportData(VCredentialFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<VCredential>().Select(x => new ExportDataItem
            {
                Granted = x.CredentialGranted,
                AchievementTitle = x.AchievementTitle,
                AchievementLabel = x.AchievementLabel,
                CredentialStatus = x.CredentialStatus,
                ExpirationExpected = x.CredentialExpirationExpected,
                Expired = x.CredentialExpired
            }).ToList().ToSearchResult();
        }

        protected override int SelectCount(VCredentialFilter filter)
            => ServiceLocator.AchievementSearch.CountCredentials(filter);

        protected override IListSource SelectData(VCredentialFilter filter)
        {
            var items = new List<SearchResultItem>();

            filter.OrderBy = "AchievementTitle, CredentialGranted desc";

            var credentials = ServiceLocator.AchievementSearch
                .GetCredentials(filter);

            var files = ServiceLocator.FileSearch
                .GetModels(filter.OrganizationIdentifier, credentials.Select(x => x.CredentialIdentifier).ToArray(), null, false);

            foreach (var credential in credentials)
            {
                var item = new SearchResultItem();

                var id = credential.CredentialIdentifier;
                item.CredentialIdentifier = id;

                var status = credential.CredentialStatus.ToEnum(CredentialStatus.Undefined);

                var layout = credential.AchievementCertificateLayoutCode;

                item.IsSelfDeclared = credential.AuthorityType == "Self";

                item.AchievementType = credential.AchievementLabel;
                item.AchievementTitle = credential.AchievementTitle;

                item.CredentialIssued = GetDateString(credential.CredentialGranted);
                item.CredentialExpiry = GetDateString(credential.CredentialExpirationExpected);
                item.CredentialStatus = GetStatusHtml(status, item.IsSelfDeclared);

                item.HasBadgeImage = credential.HasBadgeImage ?? false;
                item.BadgeImageUrl = credential.BadgeImageUrl;

                string fileUrl = null;
                var file = files.FirstOrDefault(x => x.ObjectIdentifier == id);
                if (file != null)
                    fileUrl = ServiceLocator.StorageService.GetFileUrl(file.FileIdentifier, file.FileName, true);

                string badgeUrl = null;
                if (item.HasBadgeImage && credential.BadgeImageUrl.IsNotEmpty())
                    badgeUrl = credential.BadgeImageUrl;

                item.DownloadLink = GetDownloadLink(id, status, fileUrl, badgeUrl, layout);
                item.DeleteLink = GetDeleteLink(item);

                items.Add(item);
            }

            return items.ToSearchResult();
        }

        private string GetDateString(DateTimeOffset? date)
        {
            return date.FormatDateOnly(User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language));
        }

        private string GetStatusHtml(CredentialStatus status, bool isSelfDeclared)
        {
            var html = string.Empty;

            switch (status)
            {
                case CredentialStatus.Valid:
                    html = $"<span class='text-success'><i class='fas fa-flag-checkered me-2'></i></span>{Translate("Valid")}";
                    break;
                case CredentialStatus.Pending:
                    html = $"<span class='text-warning'><i class='fas fa-hourglass me-2'></i></span>{Translate("Pending")}";
                    break;
                case CredentialStatus.Expired:
                    html = $"<span class='text-danger'><i class='fas fa-brake-warning me-2'></i></span>{Translate("Expired")}";
                    break;
            }

            return html;
        }

        private string GetDeleteLink(SearchResultItem item)
        {
            if (!item.IsSelfDeclared)
                return string.Empty;

            var html = $"<a title='Delete Certificate' class='text-danger' href='/ui/portal/record/credentials/learners/delete?credential={item.CredentialIdentifier}'><i class='fa-solid fa-trash-alt'></i></a>";

            return html;
        }

        public static string GetDownloadLink(Guid id, CredentialStatus status, string downloadUrl, string badgeUrl, string layout)
        {
            if (status != CredentialStatus.Valid)
                return null;

            if (downloadUrl.IsNotEmpty())
            {
                var html = $"<a title='Download Certificate' target='_blank' href='{downloadUrl}'><i class='fa-solid fa-download'></i></a>";
                return html;
            }

            if (badgeUrl.IsNotEmpty())
            {
                var html = $"<a title='Download Badge' target='_blank' href='/ui/portal/records/certificates/badge.ashx?credential={id}'><i class='fa-solid fa-download'></i></a>";
                return html;
            }

            if (ServiceLocator.Partition.IsE03() && layout.IsNotEmpty())
            {
                var html = $"<a title='Download Certificate' target='_blank' href='/ui/portal/records/credentials/certificate?credential={id}'><i class='fa-solid fa-download'></i></a>";
                return html;
            }

            return null;
        }
    }
}