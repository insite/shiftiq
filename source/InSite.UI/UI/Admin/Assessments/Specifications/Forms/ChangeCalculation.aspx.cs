using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Specifications.Forms
{
    public partial class ChangeCalculation : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SpecID => Guid.Parse(Request.QueryString["spec"]);

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

            RedirectToReader(SpecID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var spec = bank.FindSpecification(SpecID);
            if (spec == null)
                RedirectToReader();

            SetInputValues(spec);
        }

        private void Save()
        {
            var calc = new ScoreCalculation();

            CalculationDetails.GetInputValues(calc);

            ServiceLocator.SendCommand(new ChangeSpecificationCalculation(BankID, SpecID, calc));

            Course2Store.ClearCache(Organization.Identifier);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Specification spec)
        {
            var title =
                $"{(spec.Bank.Content.Title?.Default).IfNullOrEmpty(spec.Bank.Name)} <span class=\"form-text\">Asset #{spec.Bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            BankDetails.BindBank(spec.Bank);
            SpecificationDetails.BindSpec(spec, true, true, false);
            CalculationDetails.SetInputValues(spec.Calculation);

            CancelButton.NavigateUrl = GetReaderUrl(spec.Identifier);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? spec = null)
        {
            var url = GetReaderUrl(spec);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? spec = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (spec.HasValue)
                url += $"&spec={spec.Value}";

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