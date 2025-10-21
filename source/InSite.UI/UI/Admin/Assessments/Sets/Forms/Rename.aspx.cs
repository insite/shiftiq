using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Sets.Forms
{
    public partial class Rename : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SetID => Guid.Parse(Request["set"]);

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
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                if (bank == null)
                    RedirectToSearch();

                var set = bank.FindSet(SetID);
                if (set == null)
                    RedirectToReader();

                var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

                PageHelper.AutoBindHeader(this, null, title);

                BankDetails.BindBank(bank);
                SetDetails.BindSet(set, false);

                SetName.Text = set.Name;

                CancelButton.NavigateUrl = GetReaderUrl(set.Identifier);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var set = bank.FindSet(SetID);

            ServiceLocator.SendCommand(new RenameSet(bank.Identifier, set.Identifier, SetName.Text));

            RedirectToReader(SetID);
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? setId = null)
        {
            var url = GetReaderUrl(setId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? setId = null)
        {
            return new ReturnUrl().GetReturnUrl($"set={setId}")
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }
    }
}