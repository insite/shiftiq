using System;
using System.Linq;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Specifications.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SpecificationID => Guid.Parse(Request["spec"]);

        #endregion

        #region Initialization and loading

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
                Open();
        }

        #endregion

        #region Event handlers

        private void DeleteButton_Click(object sender, EventArgs e) => DeleteSpecification();

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            if (!bank.IsAdvanced)
                RedirectToReader();

            var spec = bank.FindSpecification(SpecificationID);
            if (spec == null)
                RedirectToReader();

            PageHelper.AutoBindHeader(
                this, 
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            var allForms = spec.EnumerateAllForms().ToArray();
            var sectionCount = 0;
            var fieldCount = 0;

            foreach (var form in allForms)
            {
                sectionCount += form.Sections.Count;
                foreach (var section in form.Sections)
                    fieldCount += section.Fields.Count;
            }

            BankName.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bank.Identifier}\">{bank.Name}</a>";
            SpecificationDetails.BindSpec(spec);

            CriterionCount.Text = spec.Criteria.Count.ToString("n0");
            FormCount.Text = allForms.Length.ToString("n0");
            SectionCount.Text = sectionCount.ToString("n0");
            FieldCount.Text = fieldCount.ToString("n0");

            CancelButton.NavigateUrl = GetReaderUrl(SpecificationID);
        }

        private void DeleteSpecification()
        {
            ServiceLocator.SendCommand(new DeleteSpecification(BankID, SpecificationID));

            Course2Store.ClearCache(Organization.Identifier);

            RedirectToReader();
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? specificationId = null)
        {
            var url = GetReaderUrl(specificationId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? specificationId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (specificationId.HasValue)
                url += $"&spec={specificationId.Value}";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }

        #endregion
    }
}