using System;

using Humanizer;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Lock : AdminBasePage, IHasParentLinkParameters
    {
        private string Command => Request["command"];

        private Guid BankID => Guid.Parse(Request["bank"]);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LockButton.Click += LockButton_Click;
            UnlockButton.Click += UnlockButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search");

            if (!IsPostBack)
            {
                SetControlsVisibility();

                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                if (bank == null)
                    RedirectToSearch();

                var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";
                PageHelper.AutoBindHeader(this, null, title);

                CardHeader.Text = $"{Command.Humanize()} Bank";
                BankDetails.BindBank(bank);

                CancelButton.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}";
            }
        }

        private void SetControlsVisibility()
        {
            LockButton.Visible = Command == "lock";
            UnlockButton.Visible = Command == "unlock";
        }

        private void LockButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new LockBank(BankID));

            RedirectToParent();
        }

        private void UnlockButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new UnlockBank(BankID));

            RedirectToParent();
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToParent() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={BankID}");

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }
    }
}