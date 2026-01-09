using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Records.Credentials.Utilities;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Achievements.Credentials.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VCredentialFilter>
    {
        #region Classes 

        public class SearchDataItem
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementTag { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public string PersonCode { get; set; }
            public Guid CredentialIdentifier { get; set; }
            public string CredentialStatus { get; set; }
            public DateTimeOffset? CredentialGranted { get; set; }
            public string CredentialExpiryHtml { get; set; }
            public string CredentialExpiryDate { get; set; }
            public DateTimeOffset? CredentialRevoked { get; set; }
            public Guid? EmployerGroupIdentifier { get; set; }
            public string EmployerGroupName { get; set; }
            public string EmployerGroupStatus { get; set; }
            public string EmployerGroupRegion { get; set; }
            public string AchievementCertificateLayoutCode { get; set; }
            public bool? HasBadgeImage { get; set; }
            public string BadgeImageUrl { get; set; }
        }

        public class ExportDataItem
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementTag { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public string PersonCode { get; set; }
            public Guid CredentialIdentifier { get; set; }
            public string CredentialStatus { get; set; }
            public DateTimeOffset? CredentialGranted { get; set; }
            public string CredentialExpiry { get; set; }
            public DateTimeOffset? CredentialRevoked { get; set; }
            public Guid? EmployerGroupIdentifier { get; set; }
            public string EmployerGroupName { get; set; }
            public string EmployerGroupStatus { get; set; }
            public string EmployerGroupRegion { get; set; }
            public string AchievementCertificateLayoutCode { get; set; }
            public bool? HasBadgeImage { get; set; }

        }

        #endregion  

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var personCodeColumn = Grid.Columns.FindByName("PersonCode");
            if (personCodeColumn == null)
                return;

            personCodeColumn.HeaderText = LabelHelper.GetLabelContentText("Person Code");
        }

        #endregion

        #region Event handlers

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DownloadBadge")
                return;

            var row = GridViewExtensions.GetRow(e);
            var credentialId = Grid.GetDataKey<Guid>(row, "CredentialIdentifier");

            var item = ServiceLocator.AchievementSearch.GetCredential(credentialId);
            if (item.HasBadgeImage != true || item.BadgeImageUrl.HasNoValue())
                return;

            var svgBytes = BadgeHelper.GetBadgeSVGFile(item.BadgeImageUrl, item.CredentialIdentifier);
            if (svgBytes == null || svgBytes.Length == 0)
                return;

            Response.SendFile("badge.svg", svgBytes, "image/svg+xml");
        }

        #endregion

        #region Loading Data

        protected override int SelectCount(VCredentialFilter filter)
        {
            return ServiceLocator.AchievementSearch.CountCredentials(filter);
        }

        protected override IListSource SelectData(VCredentialFilter filter)
        {
            return ServiceLocator.AchievementSearch
                .GetCredentials(filter)
                .Select(x => new SearchDataItem
                {
                    CredentialIdentifier = x.CredentialIdentifier,
                    UserIdentifier = x.UserIdentifier,
                    UserFullName = x.UserFullName,
                    UserEmail = x.UserEmail,
                    AchievementIdentifier = x.AchievementIdentifier,
                    AchievementTitle = x.AchievementTitle,
                    CredentialStatus = x.CredentialStatus,
                    CredentialGranted = x.CredentialGranted,
                    CredentialRevoked = x.CredentialRevoked,
                    EmployerGroupIdentifier = x.OriginalEmployerGroupIdentifier ?? x.EmployerGroupIdentifier,
                    EmployerGroupName = x.OriginalEmployerGroupName ?? x.EmployerGroupName,
                    EmployerGroupStatus = x.OriginalEmployerGroupIdentifier.HasValue ? x.OriginalEmployerGroupStatus : x.EmployerGroupStatus,
                    EmployerGroupRegion = x.OriginalEmployerGroupIdentifier.HasValue ? x.OriginalEmployerGroupRegion : x.EmployerGroupRegion,
                    AchievementCertificateLayoutCode = x.AchievementCertificateLayoutCode,
                    BadgeImageUrl = x.BadgeImageUrl,
                    HasBadgeImage = x.HasBadgeImage,
                    PersonCode = x.PersonCode,
                    AchievementTag = x.AchievementLabel,
                    CredentialExpiryDate = GetCredentialExpiryDate(x),
                    CredentialExpiryHtml = GetCredentialExpiryHtml(x)
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Export Data
        public override IListSource GetExportData(VCredentialFilter filter, bool empty)
        {
            var query = SelectData(filter).GetList().Cast<SearchDataItem>().AsQueryable();

            if (empty)
                query = query.Take(0);

            return query
                .Select(x => new ExportDataItem
                {
                    AchievementIdentifier = x.AchievementIdentifier,
                    AchievementTitle = x.AchievementTitle,
                    AchievementTag = x.AchievementTag,
                    UserIdentifier = x.UserIdentifier,
                    UserFullName = x.UserFullName,
                    UserEmail = x.UserEmail,
                    PersonCode = x.PersonCode,
                    CredentialIdentifier = x.CredentialIdentifier,
                    CredentialStatus = x.CredentialStatus,
                    CredentialGranted = x.CredentialGranted,
                    CredentialExpiry = x.CredentialExpiryDate,
                    CredentialRevoked = x.CredentialRevoked,
                    EmployerGroupIdentifier = x.EmployerGroupIdentifier,
                    EmployerGroupName = x.EmployerGroupName,
                    EmployerGroupStatus = x.EmployerGroupStatus,
                    EmployerGroupRegion = x.EmployerGroupRegion,
                    AchievementCertificateLayoutCode = x.AchievementCertificateLayoutCode,
                    HasBadgeImage = x.HasBadgeImage

                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Helper Methods

        protected string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            if (when == null)
                return string.Empty;
            return TimeZones.Format(when.Value, User.TimeZone, true, true);
        }

        private string GetCredentialExpiryHtml(VCredential item)
        {
            return Achievements.Controls.CredentialGrid.GetCredentialExpiry(item);
        }

        private string GetCredentialExpiryDate(VCredential item)
        {
            return Achievements.Controls.CredentialGrid.GetCredentialExpiryDate(item, "yyyy-MM-dd");
        }

        public string IsInQueue(string flag)
        {
            if (flag == "visible") return "none";
            return "inline";
        }

        public string IsInQueueDisplay(string flag)
        {
            if (flag != "visible") return "none";
            return "inline";
        }

        protected bool IsCredentialDownloadable()
        {
            if (IsBadgeConfigured())
                return false;

            var dataItem = (SearchDataItem)Page.GetDataItem();

            return dataItem.CredentialStatus == CredentialStatus.Valid.ToString()
                && (ServiceLocator.Partition.IsE03() || dataItem.AchievementCertificateLayoutCode.IsNotEmpty());
        }

        protected bool IsBadgeConfigured()
        {
            var dataItem = (SearchDataItem)Page.GetDataItem();

            return dataItem.HasBadgeImage == true && dataItem.BadgeImageUrl.IsNotEmpty();
        }

        #endregion
    }
}