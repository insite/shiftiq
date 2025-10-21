using System;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Specifications.Forms
{
    public partial class Rename : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SpecificationID => Guid.Parse(Request["spec"]);

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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader(SpecificationID);
        }

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var spec = bank.FindSpecification(SpecificationID);
            if (spec == null)
                RedirectToReader();

            var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            BankDetails.BindBank(bank);
            SpecificationDetails.BindSpec(spec, false);

            SpecificationType.SelectedValue = spec.Type.GetName();
            SpecificationTypeHelp.InnerText = SpecHelper.GetDescription(spec.Type);

            SpecificationName.Text = spec.Name;

            CancelButton.NavigateUrl = GetReaderUrl(SpecificationID);
        }

        private void Save()
        {
            var inputName = SpecificationName.Text;
            var inputType = SpecificationType.SelectedValue.ToEnum<SpecificationType>();

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var spec = bank.FindSpecification(SpecificationID);

            if (spec.Name != inputName)
                ServiceLocator.SendCommand(new RenameSpecification(BankID, SpecificationID, inputName));

            if (spec.Type != inputType)
                ServiceLocator.SendCommand(new RetypeSpecification(BankID, SpecificationID, inputType));

            Course2Store.ClearCache(Organization.Identifier);

            RedirectToReader(SpecificationID);
        }

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
    }
}