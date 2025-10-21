using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class ChangeStandard : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private BankState Bank
        {
            get
            {
                if (!_isBankLoaded)
                {
                    _bank = ServiceLocator.BankSearch.GetBankState(BankID);
                    _isBankLoaded = true;
                }

                return _bank;
            }
        }

        #endregion

        #region Fields

        private BankState _bank = null;
        private bool _isBankLoaded = false;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BankStandard.AssetSelected += BankStandard_AssetSelected;

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

        private void BankStandard_AssetSelected(object sender, EventArgs e)
        {
            LoadBankStandardAsset();
        }

        private void LoadBankStandardAsset()
        {
            if (!BankStandard.Value.HasValue)
            {
                QuestionSetColumn.Visible = false;
                return;
            }

            var children = StandardSearch.Bind(
                x => new
                {
                    x.StandardIdentifier,
                    x.Code,
                },
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.Parent.StandardIdentifier == BankStandard.Value.Value && x.StandardType == StandardType.Area,
                null, "Sequence");

            if (children.Length == 0)
            {
                QuestionSetColumn.Visible = false;
                return;
            }

            QuestionSetColumn.Visible = true;

            QuestionSetRepeater.DataSource = children.Select(x =>
            {
                var isExist = Bank.Sets.Any(y => y.Standard == x.StandardIdentifier);
                return new
                {
                    Value = x.StandardIdentifier,
                    Text = $"Items for GAC {x.Code}",
                    Enabled = !isExist,
                    Checked = isExist
                };
            });
            QuestionSetRepeater.DataBind();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            if (Bank == null)
                RedirectToSearch();

            SetInputValues(Bank);
        }

        private void Save()
        {
            var identifier = Guid.Empty;
            if (BankStandard.Value.HasValue)
                identifier = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == BankStandard.Value.Value);

            ServiceLocator.SendCommand(new ChangeBankStandard(BankID, identifier));

            if (QuestionSetRepeater.Visible)
            {
                foreach (RepeaterItem item in QuestionSetRepeater.Items)
                {
                    var checkbox = (ICheckBox)item.FindControl("QuestionSetCheckBox");
                    if (!checkbox.Checked || !checkbox.Enabled || !Guid.TryParse(checkbox.Value, out var gacId) || Bank.Sets.Any(x => x.Standard == gacId))
                        continue;

                    ServiceLocator.SendCommand(new AddSet(Bank.Identifier, UniqueIdentifier.Create(), checkbox.Text, gacId));
                }
            }
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(BankState bank)
        {
            var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            var standard = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == bank.Standard);

            BankStandard.Value = standard;

            LoadBankStandardAsset();

            BankDetails.BindBank(bank, false);

            CancelButton.NavigateUrl = GetReaderUrl();
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader()
        {
            var url = GetReaderUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl()
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

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