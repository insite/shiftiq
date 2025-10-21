using System;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Delete : AdminBasePage
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCheck.AutoPostBack = true;
            DeleteCheck.CheckedChanged += (x, y) => { DeleteButton.Enabled = DeleteCheck.Checked; };

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                BindModelToControls();
        }

        private void BindModelToControls()
        {
            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete))
                RedirectToSearch();

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            PageHelper.AutoBindHeader(this, null, bank.Name);

            BankDetail.BindBank(bank);

            var summary = ServiceLocator.BankSearch.GetBank(BankID)
                ?? new InSite.Application.Banks.Read.QBank();

            SetCount.Text = $"{summary.SetCount:n0}";
            QuestionCount.Text = $"{summary.QuestionCount:n0}";
            OptionCount.Text = $"{summary.OptionCount:n0}";
            SpecificationCount.Text = $"{summary.SpecCount:n0}";
            FormCount.Text = $"{summary.FormCount:n0}";

            CancelButton.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteBank(BankID));

            Course2Store.ClearCache(Organization.Identifier);

            RedirectToSearch();
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);
    }
}