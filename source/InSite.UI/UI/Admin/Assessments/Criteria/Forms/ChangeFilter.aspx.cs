using System;
using System.Web.UI;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Criteria.Forms
{
    public partial class ChangeFilter : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid CriterionID => Guid.TryParse(Request.QueryString["criterion"], out var value) ? value : Guid.Empty;

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

            try
            {
                Save();

                RedirectToReader(CriterionID);
            }
            catch (ApplicationError apperr)
            {
                EditorStatus.AddMessage(AlertType.Error, apperr.Message);
            }
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
            {
                RedirectToSearch();
                return;
            }

            var form = bank.FindCriterion(CriterionID);
            if (form == null)
                RedirectToReader();

            SetInputValues(form);
        }

        private void Save()
        {
            var values = CriterionInput.GetInputValues();

            if (values.FilterType == CriterionFilterType.Tag)
            {
                QuestionDisplayFilter.Validate(values.BasicFilter);

                ServiceLocator.SendCommand(new ChangeCriterionFilter(BankID, CriterionID, values.SetWeight, values.QuestionLimit, values.BasicFilter, null));
            }
            else if (values.FilterType == CriterionFilterType.Pivot)
            {
                ServiceLocator.SendCommand(new ChangeCriterionFilter(BankID, CriterionID, values.SetWeight, values.QuestionLimit, null, CriterionInput.RequirementsTable));
            }
            else
            {
                ServiceLocator.SendCommand(new ChangeCriterionFilter(BankID, CriterionID, values.SetWeight, values.QuestionLimit, null, null));
            }
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Criterion sieve)
        {
            var bank = sieve.Specification.Bank;

            SpecificationName.Text = sieve.Specification.Name;
            SetRepeater.DataSource = sieve.Sets;
            SetRepeater.DataBind();

            var title =
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            CriterionInput.SetInputValues(sieve);

            CancelButton.NavigateUrl = GetReaderUrl(CriterionID);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? sieveId = null)
        {
            var url = GetReaderUrl(sieveId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? sieveId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (sieveId.HasValue)
                url += $"&sieve={sieveId.Value}";

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