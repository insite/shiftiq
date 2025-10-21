using System;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Sets.Forms
{
    public partial class ChangeStandard : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SetID => Guid.Parse(Request.QueryString["set"]);

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader(SetID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var set = bank.FindSet(SetID);
            if (set == null)
                RedirectToReader();

            SetInputValues(set);
        }

        private void Save()
        {
            var standard = !string.IsNullOrEmpty(StandardAssetID.SelectedValue)
                ? Guid.Parse(StandardAssetID.SelectedValue) : Guid.Empty;

            ServiceLocator.SendCommand(new ChangeSetStandard(BankID, SetID, standard));
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Set set)
        {
            var title = $"{(set.Bank.Content.Title?.Default).IfNullOrEmpty(set.Bank.Name)} <span class=\"form-text\">Asset #{set.Bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            BankDetails.BindBank(set.Bank);
            SetDetails.BindSet(set, true, false);

            var children = StandardSearch.Bind(
                x => new
                {
                    x.StandardIdentifier,
                    x.StandardType,
                    x.Code,
                    x.AssetNumber,
                    Title = (CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title)
                        ?? CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Summary)).Replace("\r", "").Replace("\n", ""),
                    x.ContentTitle
                },
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.Parent.StandardIdentifier == set.Bank.Standard,
                "AssetNumber,ContentTitle");

            StandardAssetID.Items.Clear();

            foreach (var item in children)
            {
                StandardAssetID.Items.Add(new System.Web.UI.WebControls.ListItem
                {
                    Value = item.StandardIdentifier.ToString(),
                    Text = (string.IsNullOrEmpty(item.Code) ? string.Empty : $"{item.Code}: ") + $"{item.Title} <span class='form-text'>{item.StandardType} Asset #{item.AssetNumber}</span>",
                    Selected = item.StandardIdentifier == set.Standard
                });
            }

            var downstream = StandardContainmentSearch.Bind(
                x => new
                {
                    x.Child.StandardIdentifier,
                    x.Child.StandardType,
                    x.Child.Code,
                    x.Child.AssetNumber,
                    Title = x.Child.ContentTitle,
                    x.Child.ContentTitle
                },
                x => x.Child.OrganizationIdentifier == Organization.OrganizationIdentifier && x.Parent.StandardIdentifier == set.Bank.Standard,
                "AssetNumber,ContentTitle");

            foreach (var item in downstream)
            {
                StandardAssetID.Items.Add(new System.Web.UI.WebControls.ListItem
                {
                    Value = item.StandardIdentifier.ToString(),
                    Text = (string.IsNullOrEmpty(item.Code) ? string.Empty : $"{item.Code}: ") + $"{item.Title} <span class='form-text'>{item.StandardType} Asset #{item.AssetNumber}</span>",
                    Selected = item.StandardIdentifier == set.Standard
                });
            }

            if (children.Length + downstream.Count == 0)
            {
                SaveButton.Visible = false;
                EmptyMessage.Visible = true;
            }

            CancelButton.NavigateUrl = GetReaderUrl(SetID);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? setId = null)
        {
            var url = GetReaderUrl(setId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? setId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (setId.HasValue)
                url += $"&set={setId.Value}";

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