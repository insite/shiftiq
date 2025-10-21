using System;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Rename : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search");


            if (!IsPostBack)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);

                if (bank == null)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search");
                    return;
                }

                var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

                PageHelper.AutoBindHeader(this, null, title);

                Name.Value = bank.Name;
                BankDetails.BindBank(bank, true, false);

                CancelButton.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var bank = ServiceLocator.BankSearch.GetBank(BankID);
            if (bank.BankName != Name.Value)
                ServiceLocator.SendCommand(new RenameBank(BankID, Name.Value));

            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={BankID}");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }
    }
}