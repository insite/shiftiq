using System;
using System.Linq;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request["form"]);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
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

                var form = bank.FindForm(FormID);
                if (form == null)
                    RedirectToReader();

                PageHelper.AutoBindHeader(
                    this, 
                    qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

                FormDetails.BindForm(form, BankID, bank.IsAdvanced);
                SectionCount.Text = $"{form.Sections.Count:n0}";
                FieldCount.Text = $"{form.Sections.Sum(x => x.Fields.Count):n0}";

                CancelButton.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}&form={form.Identifier}";
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteForm(BankID, FormID));

            Course2Store.ClearCache(Organization.Identifier);

            RedirectToReader();
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? form = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (form.HasValue)
                url += $"&form={form.Value}";

            HttpResponseHelper.Redirect(url, true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }
    }
}