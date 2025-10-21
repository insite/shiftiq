using System;
using System.Linq;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Criteria.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid CriterionID => Guid.Parse(Request["criterion"]);

        private Guid SpecificationID
        {
            get => (Guid)ViewState[nameof(SpecificationID)];
            set => ViewState[nameof(SpecificationID)] = value;
        }

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

        private void DeleteButton_Click(object sender, EventArgs e) => DeleteCriteria();

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var criterion = bank.FindCriterion(CriterionID);
            if (criterion == null)
                RedirectToBankReader();

            if (!bank.IsAdvanced)
                RedirectToBankReader();

            SpecificationID = criterion.Specification.Identifier;

            PageHelper.AutoBindHeader(this, null, $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            BankName.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bank.Identifier}\">{bank.Name}</a>";
            SpecificationName.Text = criterion.Specification.Name;

            var sectionCount = 0;
            var fieldCount = 0;

            foreach (var section in criterion.Sections)
            {
                sectionCount++;
                fieldCount += section.Fields.Count;
            }

            SetName.Text = criterion.ToString();
            SetWeight.Text = criterion.SetWeight.ToString("n2");
            QuestionLimit.Text = criterion.QuestionLimit.ToString("n0") + " of " + criterion.Sets.SelectMany(x => x.Questions).Count().ToString("n0");

            var hasBasicFilter = !string.IsNullOrEmpty(criterion.TagFilter);
            var hasAdvancedFilter = !hasBasicFilter && criterion.PivotFilter != null && !criterion.PivotFilter.IsEmpty;

            FilterType.Text = hasBasicFilter ? "Filter with Question Tags" : hasAdvancedFilter ? "Filter with Pivot Table" : "Include All Questions";

            SectionCount.Text = sectionCount.ToString("n0");
            FieldCount.Text = fieldCount.ToString("n0");

            SetRepeater.DataSource = criterion.Sets;
            SetRepeater.DataBind();

            StandardRepeater.DataSource = criterion.Sets;
            StandardRepeater.DataBind();

            CancelButton.NavigateUrl = GetReaderUrl(null, CriterionID);
        }

        private void DeleteCriteria()
        {
            ServiceLocator.SendCommand(new DeleteCriterion(BankID, CriterionID));

            RedirectToBankReader(SpecificationID);
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToBankReader(Guid? specificationId = null, Guid? sieveId = null)
        {
            var url = GetReaderUrl(specificationId, sieveId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? specificationId = null, Guid? sieveId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (sieveId.HasValue)
                url += $"&sieve={sieveId.Value}";
            else if (specificationId.HasValue)
                url += $"&spec={specificationId.Value}";
            else
                url += "&panel=specifications";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }

        #endregion
    }
}