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

        public bool IsCourse { get; set; }

        public Guid AchievementIdentifier { get; set; }

        public Guid? CourseIdentifier { get; set; }

        public int CourseCount { get; set; }

        public string StatusMessageHtml { get; set; }
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

            var achievementIds = credentials.Select(x => x.AchievementIdentifier).Distinct().ToArray();
            var coursesByAchievement = new Dictionary<Guid, List<Guid>>();
            if (achievementIds.Length > 0)
            {
                var courseLinks = CourseSearch.BindCourses(
                    x => new { AchievementId = x.Gradebook.AchievementIdentifier.Value, x.CourseIdentifier },
                    x => x.Gradebook.AchievementIdentifier.HasValue && achievementIds.Contains(x.Gradebook.AchievementIdentifier.Value));

                foreach (var link in courseLinks)
                {
                    if (!coursesByAchievement.TryGetValue(link.AchievementId, out var list))
                    {
                        list = new List<Guid>();
                        coursesByAchievement.Add(link.AchievementId, list);
                    }
                    list.Add(link.CourseIdentifier);
                }
            }

            foreach (var credential in credentials)
            {
                var item = new SearchResultItem();

                var id = credential.CredentialIdentifier;
                item.CredentialIdentifier = id;

                var status = credential.CredentialStatus.ToEnum(CredentialStatus.Undefined);

                var layout = credential.AchievementCertificateLayoutCode;

                item.IsSelfDeclared = credential.AuthorityType == "Self";
                item.AchievementIdentifier = credential.AchievementIdentifier;
                if (coursesByAchievement.TryGetValue(credential.AchievementIdentifier, out var courseIds) && courseIds.Count > 0)
                {
                    item.IsCourse = true;
                    item.CourseCount = courseIds.Count;
                    if (courseIds.Count == 1)
                        item.CourseIdentifier = courseIds[0];
                }

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
                item.StatusMessageHtml = GetStatusMessageHtml(status, credential.AchievementAllowSelfDeclared, item.AchievementIdentifier, item.CourseIdentifier, item.CourseCount);

                items.Add(item);
            }

            return items.ToSearchResult();
        }

        private string GetDateString(DateTimeOffset? date)
        {
            return date.FormatDateOnly(User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language));
        }

        private string GetStatusHtml(CredentialStatus status, bool isSelfDeclared)
            => GetStatusHtml(status, Translate);

        public static string GetStatusHtml(CredentialStatus status, Func<string, string> translate)
        {
            var html = string.Empty;

            switch (status)
            {
                case CredentialStatus.Valid:
                    html = $"<span class='text-success'><i class='fas fa-flag-checkered me-2'></i></span>{translate("Valid")}";
                    break;
                case CredentialStatus.Pending:
                    html = $"<span class='text-warning'><i class='fas fa-hourglass me-2'></i></span>{translate("Pending")}";
                    break;
                case CredentialStatus.Submitted:
                    html = $"<span class='text-info'><i class='fas fa-hourglass-half me-2'></i></span>{translate("Submitted")}";
                    break;
                case CredentialStatus.Expired:
                    html = $"<span class='text-danger'><i class='fas fa-brake-warning me-2'></i></span>{translate("Expired")}";
                    break;
            }

            return html;
        }

        private string GetDeleteLink(SearchResultItem item)
        {
            if (!item.IsSelfDeclared)
                return string.Empty;

            if (Identity.IsAdministrator)
                return string.Empty;

            var html = $"<a title='Delete Certificate' class='text-danger' href='/ui/portal/record/credentials/learners/delete?credential={item.CredentialIdentifier}'><i class='fa-solid fa-trash-alt'></i></a>";

            return html;
        }

        private string GetStatusMessageHtml(CredentialStatus status, bool achievementAllowSelfDeclared, Guid achievementId, Guid? singleCourseId, int courseCount)
        {
            if (status == CredentialStatus.Submitted)
            {
                var pending = System.Web.HttpUtility.HtmlEncode(Translate("Waiting for an administrator to review your certificate"));
                return $"<div class='form-text'>{pending}</div>";
            }

            if (status != CredentialStatus.Expired || achievementAllowSelfDeclared)
                return string.Empty;

            if (courseCount == 1 && singleCourseId.HasValue)
            {
                var label = System.Web.HttpUtility.HtmlEncode(Translate("Take the course"));
                return $"<div class='form-text'><a href='/ui/portal/learning/course/{singleCourseId.Value}'>{label}</a></div>";
            }

            if (courseCount > 1)
            {
                var label = System.Web.HttpUtility.HtmlEncode(Translate("Take a course"));
                return $"<div class='form-text'><a href='/ui/portal/learning/catalogue?achievement={achievementId}'>{label}</a></div>";
            }

            var text = System.Web.HttpUtility.HtmlEncode(Translate("Please contact your administrator with a copy of your renewed certificate"));
            return $"<div class='form-text text-danger'>{text}</div>";
        }

        public static string GetDownloadLink(Guid id, CredentialStatus status, string downloadUrl, string badgeUrl, string layout)
        {
            // A submitted credential is not verified yet, so only the uploaded file is offered.
            // The badge and certificate-layout fallbacks below would render a system-issued
            // certificate for a credential nobody has reviewed.
            if (status == CredentialStatus.Submitted)
            {
                return downloadUrl.IsNotEmpty()
                    ? $"<a title='Download Certificate' target='_blank' href='{downloadUrl}'><i class='fa-solid fa-download'></i></a>"
                    : null;
            }

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

            if (ServiceLocator.Partition.IsE03() || layout.IsNotEmpty())
            {
                var html = $"<a title='Download Certificate' target='_blank' href='/ui/portal/records/credentials/certificate?credential={id}'><i class='fa-solid fa-download'></i></a>";
                return html;
            }

            return null;
        }
    }
}
