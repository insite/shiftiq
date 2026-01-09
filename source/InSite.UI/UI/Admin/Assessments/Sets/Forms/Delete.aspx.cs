using System;
using System.Linq;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Sets.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SetID => Guid.Parse(Request["set"]);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            if (!IsPostBack)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                if (bank == null)
                    RedirectToSearch();

                var set = bank.FindSet(SetID);
                if (set == null)
                    RedirectToReader();

                PageHelper.AutoBindHeader(
                    this, 
                    qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

                BankName.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bank.Identifier}\">{bank.Name}</a>";

                SetDetails.BindSet(set);

                QuestionCount.Text = $"{set.Questions.Count:n0}";
                CriterionCount.Text = $"{set.Criteria.Count:n0}";
                SectionCount.Text = $"{set.Criteria.Sum(x => x.Sections.Count):n0}";

                CancelButton.NavigateUrl = GetReaderUrl(set.Identifier);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteSet(BankID, SetID));

            RedirectToReader();
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? set = null)
        {
            var url = GetReaderUrl(set);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? set = null)
        {
            return new ReturnUrl().GetReturnUrl($"set={set}")
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }
    }
}