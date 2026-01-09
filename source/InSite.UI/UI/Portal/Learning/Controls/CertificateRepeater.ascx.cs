using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Credentials.Utilities;

using Shift.Common;

namespace InSite.UI.Portal.Learning.Controls
{
    public partial class CertificateRepeater : BaseUserControl
    {
        protected CertificateRepeaterItem[] CertificateItems
        {
            get => (CertificateRepeaterItem[])ViewState[nameof(CertificateItems)];
            set => ViewState[nameof(CertificateItems)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemCommand += ItemRepeater_ItemCommand;
        }


        public void BindModelToControl(CertificateRepeaterItem[] items)
        {
            CertificateItems = items;
            Repeater.DataSource = CertificateItems;
            Repeater.DataBind();
            SetVisibility();
        }

        private void SetVisibility()
        {
            Repeater.Visible = Repeater.Items.Count > 0;
            NoResults.Visible = Repeater.Items.Count == 0;
        }

        protected string GetDateString(DateTimeOffset? date)
        {
            if (date != null && date.HasValue)
            {
                return TimeZones.FormatDateOnly(date.Value, User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language));
            }

            return null;
        }

        protected string GetStatusHtml(string credentialStatus)
        {
            var html = new StringBuilder();

            switch (credentialStatus)
            {
                case "Valid":
                    html.AppendLine($"<span class='text-success'><i class='fas fa-flag'></i></span> {Translate("Valid")}");
                    break;
                case "Pending":
                    html.AppendLine($"<span class='text-warning'><i class='fas fa-flag'></i></span> {Translate("Pending")}");
                    break;
                case "Expired":
                    html.Append($"<span class='text-danger'><i class='fas fa-flag'></i></span> {Translate("Expired")}");
                    break;
            }

            return html.ToString();
        }

        protected string GetCertificateDownloadLink()
        {
            if (IsBadgeConfigured())
                return null;

            var dataItem = (CertificateRepeaterItem)Page.GetDataItem();

            return dataItem.CredentialStatus == Shift.Constant.CredentialStatus.Valid.ToString() && (ServiceLocator.Partition.IsE03() || dataItem.CertificationLayoutCode.IsNotEmpty())
                ? $"<a class='btn btn-sm btn-success' href=\"/ui/portal/records/credentials/certificate?credential={dataItem.CredentialIdentifier}&course-name={dataItem.AchievementTitle}\"><i class='far fa-award me-2'></i> Download</a>"
                : null;
        }

        protected bool IsBadgeConfigured()
        {
            var dataItem = (CertificateRepeaterItem)Page.GetDataItem();

            return dataItem.HasBadgeImage == true && dataItem.BadgeUrl.IsNotEmpty();
        }

        private void ItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DownloadBadge")
            {
                var item = CertificateItems[e.Item.ItemIndex];
                if (item.HasBadgeImage != true || item.BadgeUrl.HasNoValue())
                    return;

                var svgBytes = BadgeHelper.GetBadgeSVGFile(item.BadgeUrl, item.CredentialIdentifier.Value);
                if (svgBytes == null || svgBytes.Length == 0)
                    return;

                Response.SendFile("badge.svg", svgBytes, "image/svg+xml");
            }
        }
    }

    [Serializable]
    public class CertificateRepeaterItem
    {
        public string AchievementTitle { get; set; }
        public string AchievementLabel { get; set; }
        public string CertificationLayoutCode { get; set; }
        public string BadgeUrl { get; set; }
        public bool? HasBadgeImage { get; set; }

        public Guid AchievementIdentifier { get; set; }
        public Guid? CredentialIdentifier { get; set; }
        public string CredentialStatus { get; set; }
        public DateTimeOffset? CredentialGranted { get; set; }
        public DateTimeOffset? CredentialExpirationExpected { get; set; }
    }
}