using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Attachments.Forms
{
    public partial class History : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? BankID => Guid.TryParse(Request["bank"], out var value) ? value : (Guid?)null;

        private Guid? AttachmentID => Guid.TryParse(Request["attachment"], out var value) ? value : (Guid?)null;

        private bool ShowAllVersions => Request.QueryString["version"] == "all";

        #endregion

        #region Initialization and Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && !Open())
                RedirectToSearch();
        }

        #endregion

        #region Database operations

        private bool Open()
        {
            if (!BankID.HasValue || !AttachmentID.HasValue)
                return false;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID.Value);
            if (bank == null)
                return false;

            var attachment = bank.FindAttachment(AttachmentID.Value);
            if (attachment == null)
                RedirectBack();

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            CancelButton.NavigateUrl = GetBackUrl();

            if (!ShowAllVersions)
            {
                var changes = AttachmentHelper.GetChanges(BankID.Value, attachment.Identifier);

                ChangeRepeater.LoadData(bank.Identifier, changes);

                return true;
            }

            ChangeRepeater.ShowAssetNumber = true;

            var bankChanges = AttachmentHelper.GetChanges(BankID.Value);
            var entityMapping = attachment.EnumerateAllVersions().ToDictionary(x => x.Identifier);
            var assetMapping = new Dictionary<int, string>();
            var attachmentChanges = new List<IChange>();

            foreach (var e in bankChanges)
            {
                var ids = AttachmentHelper.GetAttachmentIdentifier(e).ToArray();
                var asset = string.Empty;
                var isValid = true;

                foreach (var id in ids)
                {
                    if (!entityMapping.TryGetValue(id, out var entity))
                    {
                        isValid = false;
                        break;
                    }

                    if (asset.Length != 0)
                        asset += "<br/>";

                    asset += $"{entity.Asset}.{entity.AssetVersion}";
                }

                if (!isValid)
                    continue;

                attachmentChanges.Add(e);
                assetMapping.Add(e.AggregateVersion, asset);
            }

            var items = Admin.Reports.Changes.Controls.ChangeRepeater.DataItem.FromChanges(attachmentChanges, User.TimeZone);

            foreach (var i in items)
                i.AssetNumber = assetMapping[i.Version];

            ChangeRepeater.LoadData(bank.Identifier, items);

            return true;
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search");

        private void RedirectBack()
        {
            var url = GetBackUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetBackUrl()
        {
            var url = new ReturnUrl().GetReturnUrl();

            if (url == null)
            {
                url = $"/ui/admin/assessments/banks/outline?bank={BankID}";
                if (AttachmentID.HasValue)
                    url += $"&attachment={AttachmentID}";
            }

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}